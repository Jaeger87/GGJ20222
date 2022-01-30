using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    private AudioSource m_AudioSource = null;

    [SerializeField]
    private AudioClip LobbyMusic = null;
    
    [SerializeField]
    private AudioClip GameMusic = null;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
        
        DontDestroyOnLoad(Instance);
    }

    private void Start()
    {
        PlayLobbyMusic();
    }

    public static void PlayLobbyMusic()
    {
        if (Instance.m_AudioSource != null)
        {
            Instance.m_AudioSource.Stop();
            Instance.m_AudioSource.clip = Instance.LobbyMusic;
            Instance.m_AudioSource.Play();
        }    
    }
    
    public static void PlayGameMusic()
    {
        if (Instance.m_AudioSource != null)
        {
            Instance.m_AudioSource.Stop();
            Instance.m_AudioSource.clip = Instance.GameMusic;
            Instance.m_AudioSource.Play();
        }
    }
}
