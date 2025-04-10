using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PromptGenerator : MonoBehaviour
{
    // --- CONFIGURACIÓN DEL PROMPT ---
    [SerializeField] int promptLength = 3; // Longitud de la secuencia a teclear
    [SerializeField] string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Caracteres permitidos

    // Genera y devuelve un nuevo prompt aleatorio
    public string GeneratePrompt()
    {
        StringBuilder promptBuilder = new StringBuilder();

        for (int i = 0; i < promptLength; i++)
        {
            // Elige un carácter aleatorio de la lista de permitidos
            int randomIndex = Random.Range(0, allowedCharacters.Length);
            promptBuilder.Append(allowedCharacters[randomIndex]);
        }

        string newPrompt = promptBuilder.ToString();
        Debug.Log($"Nuevo prompt generado: {newPrompt}");
        return newPrompt;
    }
}
