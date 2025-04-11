using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // --- REFERENCIAS ---
    [SerializeField] GameManager gameManager;
    [SerializeField] UIManager uiManager;

    // --- ESTADO DEL INPUT ---
    private string targetPrompt = "";
    private string currentInput = "";
    private bool isListening = false;
    private float startTime;

    void Start()
    {
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        if (isListening)
        {
            bool inputChanged = false;

            foreach (char c in Input.inputString)
            {

                if (char.IsLetterOrDigit(c)) 
                {
                    if (currentInput.Length < targetPrompt.Length)
                    {
                        // Compara directamente con el carácter esperado en mayúsculas
                        char expectedChar = targetPrompt[currentInput.Length];
                        char inputCharUpper = char.ToUpper(c);

                        if (inputCharUpper == expectedChar)
                        {
                            // Correcto
                            currentInput += inputCharUpper;
                            inputChanged = true;

                            // Comprueba si la entrada coincide completamente
                            if (currentInput.Length == targetPrompt.Length)
                            {
                                float timeTaken = Time.time - startTime;
                                // --- AQUÍ PODRÍA IR UNA ANIMACIÓN DEL JUGADOR TERMINANDO ---
                                // uiManager.PlayPlayerFinishedTypingAnimation();
                                gameManager.PlayerFinishedRound(timeTaken);
                                // isListening = false; // GameManager se encarga de llamar a StopListening
                            }
                        }
                        else
                        {
                            // Error de tipeo
                            Debug.Log($"Error de tipeo! Esperaba '{expectedChar}', recibió '{inputCharUpper}'");
                            // --- AQUÍ PODRÍA IR UNA ANIMACIÓN DE ERROR (ej. shake input field) ---
                            // uiManager.PlayTypingErrorAnimation();
                            // Opcional: Penalización por error (ej. reiniciar input, añadir tiempo)
                            // currentInput = ""; // Reinicia input completamente
                            // O solo ignorar el carácter erróneo
                            inputChanged = true; // Para actualizar UI y mostrar el error (o no si se ignora)
                        }
                    }
                    // Ignorar input si ya se alcanzó la longitud del prompt
                }
                // Considerar Backspace (opcional)
                else if (c == '\b' && currentInput.Length > 0) // Backspace
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                    inputChanged = true;
                }
            }

            // Actualiza la UI solo si el input cambió
            if (inputChanged && uiManager != null)
            {
                uiManager.UpdatePlayerRealtimeInput(currentInput);
            }
        }
    }

    public void StartListening(string prompt)
    {
        targetPrompt = prompt.ToUpper();
        currentInput = "";
        isListening = true;
        startTime = Time.time;
        if (uiManager != null) uiManager.UpdatePlayerRealtimeInput(currentInput); // Muestra input vacío inicial
        Debug.Log("PlayerInput: Empezando a escuchar.");
    }

    public void StopListening()
    {
        if (isListening) // Solo actuar si estaba escuchando
        {
            isListening = false;
            Debug.Log("PlayerInput: Detenido.");

            // Podrías decidir si quieres limpiar el input aquí o dejarlo visible
            if (uiManager != null) uiManager.UpdatePlayerRealtimeInput("");
        }
    }

    public string GetFinalInput()
    {
        return currentInput;
    }
}