using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // --- REFERENCIAS ---
    [SerializeField] UIManager uiManager;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] EnemyAI enemyAI;
    [SerializeField] PromptGenerator promptGenerator;
    [SerializeField] SceneManager sceneManager; 

    // --- CONFIGURACI�N ---
    [SerializeField] int startLives = 3;
    [SerializeField] float countdownTime = 3f;
    [SerializeField] float delayAfterPrompt = 0.5f;
    [SerializeField] float delayBetweenRounds = 2f;

    // --- ESTADO ---
    public enum GameState { Idle, Countdown, ShowingPrompt, WaitingInput, Comparing, RoundEnd, GameOver }
    private GameState currentState = GameState.Idle;

    private int playerLives;
    private int enemyLives;
    private string currentPrompt;
    private float playerTime = float.MaxValue; // Inicializa a un valor alto
    private float enemyTime = float.MaxValue;  // Inicializa a un valor alto
    private bool playerFinished = false;
    private bool enemyFinished = false;

    void Start()
    {
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        if (playerInput == null) playerInput = FindObjectOfType<PlayerInput>();
        if (enemyAI == null) enemyAI = FindObjectOfType<EnemyAI>();
        if (promptGenerator == null) promptGenerator = FindObjectOfType<PromptGenerator>();
        if (sceneManager == null) sceneManager = FindObjectOfType<SceneManager>(); 

        StartGame();
    }

    public void StartGame()
    {
        playerLives = startLives;
        enemyLives = startLives;
        uiManager.UpdateLives(playerLives, enemyLives);
        uiManager.ShowMessage("");
        uiManager.ClearRoundDisplay(); // Limpia prompts y tiempos anteriores
        enemyAI.StopSimulation(); // Detiene simulaciones anteriores

        currentState = GameState.Idle;
        StartCoroutine(StartFirstRound());
    }

    private IEnumerator StartFirstRound()
    {
        yield return null; // Espera un frame
        StartCoroutine(RoundCoroutine());
    }

    private IEnumerator RoundCoroutine()
    {
        // --- Preparaci�n de Ronda ---
        currentState = GameState.Idle; // Estado intermedio antes de cuenta atr�s
        uiManager.ClearRoundDisplay(); // Limpia input/tiempos de ronda anterior
        playerFinished = false;
        enemyFinished = false;
        playerTime = float.MaxValue; // Resetea tiempos
        enemyTime = float.MaxValue;

        // --- Cuenta Atr�s ---
        currentState = GameState.Countdown;
        yield return StartCoroutine(uiManager.CountdownCoroutine(countdownTime));
        // --- AQU� PODR�A IR UNA ANIMACI�N DE "START" / "GO" ---
        // uiManager.PlayStartAnimation();

        // --- Mostrar Prompt ---
        currentState = GameState.ShowingPrompt;
        currentPrompt = promptGenerator.GeneratePrompt();
        uiManager.ShowPrompt(currentPrompt);

        yield return new WaitForSeconds(delayAfterPrompt);

        // --- Esperando Input ---
        currentState = GameState.WaitingInput;

        // Iniciar escucha del jugador y simulaci�n del enemigo
        playerInput.StartListening(currentPrompt);
        enemyTime = enemyAI.StartEnemyTurn(currentPrompt, this); // Pasa referencia a GameManager

        // Espera hasta que ALGUIEN termine
        while (!playerFinished && !enemyFinished)
        {
            yield return null; // Espera al siguiente frame
        }

        // --- Comparaci�n y Fin de Ronda ---
        currentState = GameState.Comparing;

        // Determinar qui�n termin� primero y detener al otro
        bool playerWinsRound = playerFinished && playerTime <= enemyTime; // Player termin� y fue m�s r�pido (o igual)
        bool enemyWinsRound = enemyFinished && enemyTime < playerTime;   // Enemy termin� y fue estrictamente m�s r�pido


        if (playerFinished && !enemyFinished) // Jugador termin� primero
        {
            enemyAI.StopSimulation(); // Detiene la simulaci�n visual del enemigo
            // enemyTime se mantiene como el valor precalculado
            playerWinsRound = true; // Aseguramos que player gana si termin� primero
            enemyWinsRound = false;
            //Debug.Log($"Ronda terminada por JUGADOR en {playerTime:F2}s. Enemigo detenido.");
        }
        else if (enemyFinished && !playerFinished) // Enemigo termin� primero
        {
            playerInput.StopListening(); // Detiene la escucha del jugador
            // playerTime se mantiene como float.MaxValue o el tiempo parcial si se quisiera registrar
            enemyWinsRound = true; // Aseguramos que enemigo gana si termin� primero
            playerWinsRound = false;
            //Debug.Log($"Ronda terminada por ENEMIGO en {enemyTime:F2}s. Jugador detenido.");
        }
        else if (playerFinished && enemyFinished) // Ambos terminaron en el mismo frame (raro, pero posible)
        {
            // La l�gica inicial de comparaci�n (playerTime <= enemyTime) decide el empate
            //Debug.Log($"Ronda terminada por AMBOS casi al mismo tiempo. P: {playerTime:F2}s, E: {enemyTime:F2}s");
        }


        // --- Mostrar Resultado de la Ronda ---
        string roundResultMessage;
        float winningTime = 0f;

        if (playerWinsRound)
        {
            enemyLives--;
            winningTime = playerTime;
            roundResultMessage = $"You won! ({winningTime:F2}s)";
            uiManager.DisplayRoundResult(winningTime, true);
            AnimationManager.Instance.PlayPlayerShoot();
            if (enemyLives > 0)
            {
                AnimationManager.Instance.PlayEnemyDamaged();
            }
            else
            {
                AnimationManager.Instance.PlayEnemyDeath();
            }
        }
        else // Incluye el caso en que el enemigo gane (enemyWinsRound == true)
        {
            playerLives--;
            winningTime = enemyTime;
            roundResultMessage = $"Enemy won ({winningTime:F2}s)";
            uiManager.DisplayRoundResult(winningTime, false); // Muestra tiempo del enemigo como ganador
            AnimationManager.Instance.PlayEnemyShoot();
            if(playerLives > 0)
            {
                AnimationManager.Instance.PlayPlayerDamaged();
            }
            else
            {
                AnimationManager.Instance.PlayPlayerDeath();
            }
        }

        uiManager.ShowMessage(roundResultMessage, delayBetweenRounds - 0.5f); // Muestra mensaje por un tiempo
        uiManager.UpdateLives(playerLives, enemyLives);

        // --- Comprobar Fin de Juego ---
        if (playerLives <= 0 || enemyLives <= 0)
        {
            currentState = GameState.GameOver;
            enemyAI.StopSimulation(); // Asegura que la simulaci�n del enemigo est� detenida
            playerInput.StopListening(); // Asegura que el input del jugador est� detenido

            string finalMessage = playerLives > 0 ? "You have won the duel!" : "You have lost the duel!";
            // --- AQU� PODR�A IR UNA ANIMACI�N DE FIN DE JUEGO (VICTORIA/DERROTA) ---
            // if (playerLives > 0) uiManager.PlayPlayerWinGameAnimation();
            // else uiManager.PlayEnemyWinGameAnimation();

            uiManager.ShowMessage(finalMessage, 10f); // Muestra mensaje final m�s tiempo
            
            

            yield return new WaitForSeconds(10f); // Espera antes de cambiar de escena

            if (sceneManager != null)
            {
                sceneManager.ChangeScene("Menu"); // O usa UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            }
            else
            {
                Debug.LogWarning("SceneManager no asignado. No se puede cambiar de escena.");
            }
        }
        else
        {
            // --- Iniciar Siguiente Ronda ---
            currentState = GameState.RoundEnd;
            yield return new WaitForSeconds(delayBetweenRounds);
            StartCoroutine(RoundCoroutine()); // Llama recursivamente para la siguiente ronda
        }
    }

    // Llamado por PlayerInput cuando completa el prompt
    public void PlayerFinishedRound(float time)
    {
        // Solo procesa si estamos esperando input y el jugador no ha terminado ya
        if (currentState == GameState.WaitingInput && !playerFinished)
        {
            playerTime = time;
            playerFinished = true;
            //Debug.Log($"PlayerInput notific� finalizaci�n en {time:F2}s");
        }
    }

    // Llamado por EnemyAI cuando completa la simulaci�n
    public void EnemyFinishedRound(float time)
    {
        // Solo procesa si estamos esperando input y el enemigo no ha terminado ya
        if (currentState == GameState.WaitingInput && !enemyFinished)
        {
            // Usamos el tiempo precalculado que nos pasa EnemyAI
            enemyTime = time;
            enemyFinished = true;
            Debug.Log($"EnemyAI notific� finalizaci�n en {time:F2}s");
        }
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }
}