using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // --- REFERENCIAS ---
    [SerializeField] GameManager gameManager;
    [SerializeField] UIManager uiManager; // Añade referencia a UIManager

    // --- ESTADO DEL INPUT ---
    private string targetPrompt = "";
    private string currentInput = "";
    private bool isListening = false;
    private float startTime;

    void Start()
    {
        // Intenta encontrar referencias si no están asignadas
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>(); // Busca UIManager
    }

    void Update()
    {
        if (isListening)
        {
            bool inputChanged = false; // Bandera para saber si actualizar UI

            foreach (char c in Input.inputString)
            {
                if (char.IsLetterOrDigit(c))
                {
                    // Comprueba si añadir el carácter excedería la longitud del prompt
                    if (currentInput.Length < targetPrompt.Length)
                    {
                        currentInput += char.ToUpper(c); // Convierte a mayúsculas para comparar fácil
                        inputChanged = true;

                        // Comprueba si la entrada actual coincide con el inicio del prompt
                        if (targetPrompt.StartsWith(currentInput, System.StringComparison.OrdinalIgnoreCase))
                        {
                            // Si la entrada coincide completamente
                            if (currentInput.Length == targetPrompt.Length)
                            {
                                float timeTaken = Time.time - startTime;
                                gameManager.PlayerFinishedRound(timeTaken);
                                // isListening = false; // GameManager llama a StopListening
                            }
                        }
                        else
                        {
                            // Error de tipeo
                            Debug.Log("Error de tipeo!");
                            currentInput = ""; // Reinicia input
                        }
                    }
                }
            }

            // Actualiza la UI solo si el input cambió en este frame
            if (inputChanged && uiManager != null)
            {
                uiManager.UpdatePlayerRealtimeInput(currentInput);
            }
        }
    }

    public void StartListening(string prompt)
    {
        targetPrompt = prompt.ToUpper(); // Guarda prompt en mayúsculas
        currentInput = "";
        isListening = true;
        startTime = Time.time;
        if (uiManager != null) uiManager.UpdatePlayerRealtimeInput(currentInput); // Muestra input vacío inicial
    }

    public void StopListening()
    {
        isListening = false;
        // No limpiamos currentInput aquí para que se vea el resultado final
        Debug.Log("Input detenido.");
    }

    // Añade un método para obtener el input final si es necesario fuera de este script
    public string GetFinalInput()
    {
        return currentInput;
    }
}
