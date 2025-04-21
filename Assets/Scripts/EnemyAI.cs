using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // --- REFERENCIAS ---
    [SerializeField] UIManager uiManager;
    private GameManager gameManager; 

    // --- CONFIGURACI�N DE DIFICULTAD ---
    [SerializeField] float minReactionTime = 0.3f;
    [SerializeField] float maxReactionTime = 1.5f;

    //[SerializeField] bool useErrorChance = false; // Si se quiere usar la posibilidad de error
    //[SerializeField] float errorChance = 0.05f; 
    //[SerializeField] float errorPauseTime = 0.5f; 

    private float currentRoundTotalTime = 0f; // Guarda el tiempo total planeado para esta ronda
    private Coroutine simulationCoroutine; // Para poder detenerla espec�ficamente

    void Start()
    {
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
    }

    // Inicia la simulaci�n de escritura del enemigo para esta ronda
    public float StartEnemyTurn(string prompt, GameManager gm)
    {
        gameManager = gm; // Guarda la referencia

        // Calcula y guarda el tiempo total para esta ronda
        currentRoundTotalTime = Random.Range(minReactionTime, maxReactionTime);
        //Debug.Log($"Enemigo intentar� terminar en: {currentRoundTotalTime:F2}s (Prompt: {prompt})");

        // Detiene cualquier simulaci�n anterior por si acaso
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
            Debug.LogError("EnemyAI: Faltan referencias (UIManager o GameManager) o prompt vac�o.");
            yield break; // Salir si algo falta
        }

        string simulatedInput = "";
        uiManager.UpdateEnemyRealtimeInput(simulatedInput); // Muestra vac�o al inicio

        // Calcula el tiempo por car�cter (aproximado)
        float timePerChar = (targetPrompt.Length > 0) ? totalTime / targetPrompt.Length : 0;
        float timeElapsed = 0f;

        // Espera y a�ade un car�cter a la vez
        for (int i = 0; i < targetPrompt.Length; i++)
        {
            // Calcula cu�nto esperar para este car�cter
            float waitTime = timePerChar;

            // Opcional: A�adir peque�a variaci�n aleatoria al tiempo por caracter
            waitTime += Random.Range(-timePerChar * 0.1f, timePerChar * 0.1f);
            waitTime = Mathf.Max(0.01f, waitTime); // Asegura un tiempo m�nimo

            // Espera simulando el tipeo
            yield return new WaitForSeconds(waitTime);
            timeElapsed += waitTime;

            // TO-DO: Simular error
            

            // A�ade el siguiente car�cter del prompt al input simulado
            simulatedInput += targetPrompt[i];

            // Actualiza la UI con el input simulado actual
            uiManager.UpdateEnemyRealtimeInput(simulatedInput);
        }

        // Asegura esperar el tiempo total restante si el c�lculo por car�cter fue impreciso
        float remainingTime = totalTime - timeElapsed;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        //Debug.Log($"Enemigo termin� simulaci�n visual para: {targetPrompt}. Tiempo total simulado ~{totalTime:F2}s");
        // --- AQU� PODR�A IR UNA ANIMACI�N DEL ENEMIGO TERMINANDO ---
        // uiManager.PlayEnemyFinishedTypingAnimation();

        // Notifica a GameManager que ha terminado, pasando el tiempo PRECALCULADO
        if (gameManager != null)
        {
            gameManager.EnemyFinishedRound(currentRoundTotalTime);
        }
        simulationCoroutine = null; // Resetea la referencia a la corutina
    }

    // Detiene la corutina de simulaci�n actual
    private void StopSimulationInternal()
    {
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
            Debug.Log("EnemyAI: Simulaci�n detenida.");
        }
        // Limpia el texto de input del enemigo inmediatamente al detenerse
        if (uiManager != null)
        {
            uiManager.UpdateEnemyRealtimeInput("");
        }
    }

    // M�todo p�blico para detener la simulaci�n desde fuera (GameManager)
    public void StopSimulation()
    {
        StopSimulationInternal();
    }


}