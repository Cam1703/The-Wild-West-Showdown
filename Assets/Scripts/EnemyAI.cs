using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // --- REFERENCIAS ---
    [SerializeField] UIManager uiManager;
    private GameManager gameManager; 

    // --- CONFIGURACIÓN DE DIFICULTAD ---
    [SerializeField] float minReactionTime = 0.3f;
    [SerializeField] float maxReactionTime = 1.5f;

    //[SerializeField] bool useErrorChance = false; // Si se quiere usar la posibilidad de error
    //[SerializeField] float errorChance = 0.05f; 
    //[SerializeField] float errorPauseTime = 0.5f; 

    private float currentRoundTotalTime = 0f; // Guarda el tiempo total planeado para esta ronda
    private Coroutine simulationCoroutine; // Para poder detenerla específicamente

    void Start()
    {
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
    }

    // Inicia la simulación de escritura del enemigo para esta ronda
    public float StartEnemyTurn(string prompt, GameManager gm)
    {
        gameManager = gm; // Guarda la referencia

        // Calcula y guarda el tiempo total para esta ronda
        currentRoundTotalTime = Random.Range(minReactionTime, maxReactionTime);
        //Debug.Log($"Enemigo intentará terminar en: {currentRoundTotalTime:F2}s (Prompt: {prompt})");

        // Detiene cualquier simulación anterior por si acaso
        StopSimulationInternal();

        // Inicia la corutina que simula la escritura
        simulationCoroutine = StartCoroutine(SimulateTypingCoroutine(prompt, currentRoundTotalTime));

        // Devuelve el tiempo total calculado para que GameManager lo use inmediatamente
        return currentRoundTotalTime;
    }

    // Coroutine que simula al enemigo "escribiendo" el prompt
    private IEnumerator SimulateTypingCoroutine(string targetPrompt, float totalTime)
    {
        if (uiManager == null || targetPrompt.Length == 0 || gameManager == null)
        {
            Debug.LogError("EnemyAI: Faltan referencias (UIManager o GameManager) o prompt vacío.");
            yield break; // Salir si algo falta
        }

        string simulatedInput = "";
        uiManager.UpdateEnemyRealtimeInput(simulatedInput); // Muestra vacío al inicio

        // Calcula el tiempo por carácter (aproximado)
        float timePerChar = (targetPrompt.Length > 0) ? totalTime / targetPrompt.Length : 0;
        float timeElapsed = 0f;

        // Espera y añade un carácter a la vez
        for (int i = 0; i < targetPrompt.Length; i++)
        {
            // Calcula cuánto esperar para este carácter
            float waitTime = timePerChar;

            // Opcional: Añadir pequeña variación aleatoria al tiempo por caracter
            waitTime += Random.Range(-timePerChar * 0.1f, timePerChar * 0.1f);
            waitTime = Mathf.Max(0.01f, waitTime); // Asegura un tiempo mínimo

            // Espera simulando el tipeo
            yield return new WaitForSeconds(waitTime);
            timeElapsed += waitTime;

            // TO-DO: Simular error
            

            // Añade el siguiente carácter del prompt al input simulado
            simulatedInput += targetPrompt[i];

            // Actualiza la UI con el input simulado actual
            uiManager.UpdateEnemyRealtimeInput(simulatedInput);
        }

        // Asegura esperar el tiempo total restante si el cálculo por carácter fue impreciso
        float remainingTime = totalTime - timeElapsed;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        //Debug.Log($"Enemigo terminó simulación visual para: {targetPrompt}. Tiempo total simulado ~{totalTime:F2}s");
        // --- AQUÍ PODRÍA IR UNA ANIMACIÓN DEL ENEMIGO TERMINANDO ---
        // uiManager.PlayEnemyFinishedTypingAnimation();

        // Notifica a GameManager que ha terminado, pasando el tiempo PRECALCULADO
        if (gameManager != null)
        {
            gameManager.EnemyFinishedRound(currentRoundTotalTime);
        }
        simulationCoroutine = null; // Resetea la referencia a la corutina
    }

    // Detiene la corutina de simulación actual
    private void StopSimulationInternal()
    {
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
            Debug.Log("EnemyAI: Simulación detenida.");
        }
        // Limpia el texto de input del enemigo inmediatamente al detenerse
        if (uiManager != null)
        {
            uiManager.UpdateEnemyRealtimeInput("");
        }
    }

    // Método público para detener la simulación desde fuera (GameManager)
    public void StopSimulation()
    {
        StopSimulationInternal();
    }


}