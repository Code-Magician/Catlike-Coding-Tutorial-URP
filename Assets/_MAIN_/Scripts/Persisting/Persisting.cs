using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Persisting : MonoBehaviour
{
    public static Persisting Instance;

    [Header("References")]
    [SerializeField] LoadingScreen loading;
    [SerializeField] AudioSource bgMusicAudioSource, sfxAudioSource;
    [SerializeField] Settings settings;
    [SerializeField] Pause pausePanel;
    [SerializeField] GameObject pauseBtn;


    public float BGMusicVolume
    {
        get { return bgMusicAudioSource.volume; }
        set { bgMusicAudioSource.volume = value; }
    }

    public float SFXVolume
    {
        get { return sfxAudioSource.volume; }
        set { sfxAudioSource.volume = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
        {
            pauseBtn.SetActive(false);
        }
        else
        {
            pauseBtn.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        loading.StartLoading(sceneName);
    }

    public void ShowSettings()
    {
        settings.Show();
    }

    public void ShowPausePanel()
    {
        pausePanel.Show();
    }
}