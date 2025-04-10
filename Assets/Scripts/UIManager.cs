using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    // --- REFERENCIAS A ELEMENTOS UI ---

    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] TextMeshProUGUI playerLivesText;
    [SerializeField] TextMeshProUGUI enemyLivesText;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI playerTimeText;
    [SerializeField] TextMeshProUGUI enemyTimeText;
    [SerializeField] TextMeshProUGUI playerRealtimeInputText;
    [SerializeField] TextMeshProUGUI enemyRealtimeInputText;


    void Start()
    {
        ClearRoundDisplay();
        if (messageText) messageText.text = "";
        if (playerLivesText) playerLivesText.text = ""; 
        if (enemyLivesText) enemyLivesText.text = "";
    }

    // Limpia la información específica de la ronda
    public void ClearRoundDisplay()
    {
        if (countdownText) countdownText.gameObject.SetActive(false);
        if (promptText) promptText.gameObject.SetActive(false);
        if (playerTimeText) playerTimeText.gameObject.SetActive(false);
        if (enemyTimeText) enemyTimeText.gameObject.SetActive(false);
        if (playerRealtimeInputText) playerRealtimeInputText.gameObject.SetActive(false); // Oculta input jugador
        if (enemyRealtimeInputText) enemyRealtimeInputText.gameObject.SetActive(false); // Oculta input enemigo
        if (messageText) messageText.text = "";
    }


    // Coroutine para mostrar la cuenta regresiva
    public IEnumerator CountdownCoroutine(float duration)
    {
        if (!countdownText) yield break;
        ClearRoundDisplay(); // Limpia info anterior

        float timer = duration;
        countdownText.gameObject.SetActive(true);

        while (timer > 0)
        {
            int seconds = Mathf.CeilToInt(timer);
            countdownText.text = seconds.ToString();
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        countdownText.text = "Shoot!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
    }

    // Muestra el prompt
    public void ShowPrompt(string prompt)
    {
        if (promptText)
        {
            promptText.text = $"{prompt}"; // Añade etiqueta
            promptText.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
        }
        // Activa los campos de input real-time pero vacíos
        UpdatePlayerRealtimeInput("");
        UpdateEnemyRealtimeInput("");
        if (playerRealtimeInputText) playerRealtimeInputText.gameObject.SetActive(true);
        if (enemyRealtimeInputText) enemyRealtimeInputText.gameObject.SetActive(true);
    }

    // Actualiza los textos de las vidas
    public void UpdateLives(int playerLives, int enemyLives)
    {
        if (playerLivesText) playerLivesText.text = $"Lives: {playerLives}";
        if (enemyLivesText) enemyLivesText.text = $"Lives: {enemyLives}";
    }

    // Muestra un mensaje
    public void ShowMessage(string message, float duration = 0f)
    {
        if (messageText)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(!string.IsNullOrEmpty(message));
            if (duration > 0)
            {
                StopCoroutine("ClearMessageAfterDelay");
                StartCoroutine(ClearMessageAfterDelay(duration));
            }
        }
    }

    // Muestra los tiempos finales
    public void ShowTimes(float playerTime, float enemyTime)
    {
        if (playerTimeText)
        {
            playerTimeText.text = $"Your Time: {playerTime:F2}s";
            playerTimeText.gameObject.SetActive(true);
        }
        if (enemyTimeText)
        {
            enemyTimeText.text = $"Enemy Time: {enemyTime:F2}s";
            enemyTimeText.gameObject.SetActive(true);
        }
    }

    // NUEVO: Actualiza el texto del input en tiempo real del jugador
    public void UpdatePlayerRealtimeInput(string currentInput)
    {
        if (playerRealtimeInputText)
        {
            playerRealtimeInputText.text = $"{currentInput}";
            // No necesitamos activar/desactivar aquí si se hace en ShowPrompt
        }
    }

    // NUEVO: Actualiza el texto del input simulado en tiempo real del enemigo
    public void UpdateEnemyRealtimeInput(string simulatedInput)
    {
        if (enemyRealtimeInputText)
        {
            enemyRealtimeInputText.text = $"{simulatedInput}";
            // No necesitamos activar/desactivar aquí si se hace en ShowPrompt
        }
    }


    // Coroutine para limpiar el mensaje
    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageText)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}
