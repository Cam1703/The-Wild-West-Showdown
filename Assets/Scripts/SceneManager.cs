using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(WaitForSFXAndChangeScene(0.8f, sceneName));
        }
        else
        {
            Debug.LogError("El nombre de la escena no es válido.");
        }
    }
    private IEnumerator WaitForSFXAndChangeScene(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
