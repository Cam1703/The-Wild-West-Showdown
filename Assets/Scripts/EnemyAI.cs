using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // --- REFERENCIAS ---
    [SerializeField] UIManager uiManager;

    // --- CONFIGURACI�N DE DIFICULTAD ---
    [SerializeField] float minReactionTime = 0.3f;
    [SerializeField] float maxReactionTime = 1.5f;

    private float currentRoundReactionTime = 0f; // Guarda el tiempo de esta ronda

    void Start()
    {
        // Busca UIManager si no est� asignado
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
    }

    // Inicia la simulaci�n de escritura del enemigo para esta ronda
    public float StartEnemyTurn(string prompt)
    {
        // Calcula y guarda el tiempo total para esta ronda
        currentRoundReactionTime = Random.Range(minReactionTime, maxReactionTime);
        Debug.Log($"Enemigo reaccionar� en: {currentRoundReactionTime:F2}s con prompt: {prompt}");

        // Inicia la corutina que simula la escritura
        StartCoroutine(SimulateTypingCoroutine(prompt, currentRoundReactionTime));

        // Devuelve el tiempo total calculado para que GameManager lo use en la comparaci�n
        return currentRoundReactionTime;
    }

    // Coroutine que simula al enemigo "escribiendo" el prompt
    private IEnumerator SimulateTypingCoroutine(string targetPrompt, float totalTime)
    {
        if (uiManager == null || targetPrompt.Length == 0) yield break; // Salir si no hay UI o prompt

        string simulatedInput = "";
        uiManager.UpdateEnemyRealtimeInput(simulatedInput); // Muestra vac�o al inicio

        // Calcula el tiempo por car�cter
        float timePerChar = totalTime / targetPrompt.Length;

        // Espera y a�ade un car�cter a la vez
        for (int i = 0; i < targetPrompt.Length; i++)
        {
            // Espera el tiempo calculado para "escribir" el siguiente car�cter
            yield return new WaitForSeconds(timePerChar);

            // A�ade el siguiente car�cter del prompt al input simulado
            simulatedInput += targetPrompt[i];

            // Actualiza la UI con el input simulado actual
            uiManager.UpdateEnemyRealtimeInput(simulatedInput);
        }

        Debug.Log($"Enemigo termin� de simular escritura para: {targetPrompt}");
    }

    // Detiene todas las corutinas en este script (�til si se reinicia la ronda bruscamente)
    public void StopSimulation()
    {
        StopAllCoroutines();
        // Opcional: Limpiar el texto de input del enemigo inmediatamente
        // if(uiManager != null) uiManager.UpdateEnemyRealtimeInput("");
    }
}
