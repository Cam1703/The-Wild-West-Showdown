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

    // --- CONFIGURACIÓN ---
    [SerializeField] int startLives = 3;
    [SerializeField]  float countdownTime = 3f;
    [SerializeField] float delayAfterPrompt = 0.5f;
    [SerializeField] float delayBetweenRounds = 2f;

    // --- ESTADO ---
    public enum GameState { Idle, Countdown, ShowingPrompt, WaitingInput, Comparing, RoundEnd, GameOver }
    private GameState currentState = GameState.Idle;

    private int playerLives;
    private int enemyLives;
    private string currentPrompt;
    private float playerTime;
    private float enemyTime;
    private bool playerFinished = false;

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        // Reinicia las vidas y limpia la UI
        playerLives = startLives;
        enemyLives = startLives;
        uiManager.UpdateLives(playerLives, enemyLives);
        uiManager.ShowMessage("");
        uiManager.ClearRoundDisplay();
        enemyAI.StopSimulation(); // Detiene simulaciones anteriores si las hubiera

        // Asegúrate de que el estado inicial sea Idle
        currentState = GameState.Idle;

        // Inicia la primera ronda después de un pequeño retraso para garantizar que la UI esté lista
        StartCoroutine(StartFirstRound());
    }

    private IEnumerator StartFirstRound()
    {
        yield return null; // Espera un frame para asegurarte de que todo esté inicializado
        StartCoroutine(RoundCoroutine());
    }

    private IEnumerator RoundCoroutine()
    {
        currentState = GameState.Countdown;
        yield return StartCoroutine(uiManager.CountdownCoroutine(countdownTime));

        currentState = GameState.ShowingPrompt;
        currentPrompt = promptGenerator.GeneratePrompt();
        uiManager.ShowPrompt(currentPrompt); // UIManager ahora también activa los campos de input real-time
        //uiManager.ShowMessage("Ready?");

        yield return new WaitForSeconds(delayAfterPrompt);

        currentState = GameState.WaitingInput;
        //uiManager.ShowMessage("GO");
        playerFinished = false;
        playerInput.StartListening(currentPrompt);
        // Inicia la simulación del enemigo y guarda su tiempo total planeado
        enemyTime = enemyAI.StartEnemyTurn(currentPrompt);

        // Espera hasta que el jugador termine
        while (!playerFinished)
        {
            yield return null;
        }
        // En este punto, el jugador ha terminado. La corutina del enemigo puede seguir ejecutándose.

        currentState = GameState.Comparing;

        // --- MOSTRAR TIEMPOS FINALES ---
        // Esperamos un poquito para asegurar que la corutina del enemigo termine visualmente
        // (Idealmente, la comparación real usa el 'enemyTime' guardado, no espera la simulación)
        // yield return new WaitForSeconds(0.1f); // Pausa opcional si quieres que se vea el final de la simulación

        uiManager.ShowTimes(playerTime, enemyTime); // Muestra los tiempos calculados

        // Determina el ganador (usa los tiempos calculados, no depende de cuándo termina la *animación* del enemigo)
        bool playerWinsRound = playerTime <= enemyTime;
        string roundResultMessage;

        if (playerWinsRound)
        {
            enemyLives--;
            roundResultMessage = "You won the round";
        }
        else
        {
            playerLives--;
            roundResultMessage = "You lost the round";
        }

        uiManager.ShowMessage(roundResultMessage);
        // playerInput.StopListening(); // Se llama desde PlayerFinishedRound
        uiManager.UpdateLives(playerLives, enemyLives);

        // Comprueba fin de juego
        if (playerLives <= 0 || enemyLives <= 0)
        {
            currentState = GameState.GameOver;
            enemyAI.StopSimulation(); // Detiene simulación del enemigo al final
            string finalMessage = playerLives > 0 ? "You have won the duel" : "You have lost the duel";
            uiManager.ShowMessage(finalMessage, 5f);
            yield return new WaitForSeconds(5f);
            StartGame(); // Reinicia
        }
        else
        {
            currentState = GameState.RoundEnd;
            yield return new WaitForSeconds(delayBetweenRounds);
            StartCoroutine(RoundCoroutine());
        }
    }

    // Llamado por PlayerInput
    public void PlayerFinishedRound(float time)
    {
        // Asegura que solo procesamos esto una vez y en el estado correcto
        if (currentState == GameState.WaitingInput && !playerFinished)
        {
            playerTime = time;
            playerFinished = true;
            playerInput.StopListening(); // Detiene la escucha del jugador
            Debug.Log($"Jugador terminó en {time:F2}s");
            // No detenemos la simulación del enemigo aquí, dejamos que termine visualmente.
        }
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }
}
