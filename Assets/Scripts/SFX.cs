using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    // Sound List
    [SerializeField] private List<AudioClip> soundList;

    // Audio Source
    private AudioSource audioSource;
    private AudioReverbFilter reverbFilter;

    public static SFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize the AudioSource
        audioSource = GetComponent<AudioSource>();
        reverbFilter = GetComponent<AudioReverbFilter>();
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = soundList.Find(s => s.name == soundName);
        if (clip != null)
        {
            // Play the sound
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log($"Sound '{soundName}' not found in the list.");
        }
    }
}
