using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CanvasGroup settingsCG;
    [SerializeField] TMP_Text bgMusicVolumePercentage, sfxVolumePercentage;
    [SerializeField] Slider bgMusicSlider, sfxSlider;


    private void OnEnable()
    {
        bgMusicSlider.onValueChanged.AddListener(OnBGMusicSliderValueChange);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderValueChange);
    }

    private void OnDisable()
    {
        bgMusicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        bgMusicVolumePercentage.text = $"{Persisting.Instance.BGMusicVolume * 100:F0}%";
        sfxVolumePercentage.text = $"{Persisting.Instance.SFXVolume * 100:F0}%";
        bgMusicSlider.value = Persisting.Instance.BGMusicVolume;
        sfxSlider.value = Persisting.Instance.SFXVolume;
    }

    void OnBGMusicSliderValueChange(float value)
    {
        bgMusicVolumePercentage.text = $"{value * 100:F0}%";
        Persisting.Instance.BGMusicVolume = value;
    }

    void OnSfxSliderValueChange(float value)
    {
        sfxVolumePercentage.text = $"{value * 100:F0}%";
        Persisting.Instance.SFXVolume = value;
    }

    public void Show()
    {
        settingsCG.blocksRaycasts = true;
        settingsCG.DOFade(1f, 0.5f);
    }

    public void Hide()
    {
        settingsCG.DOFade(0f, 0.5f).OnComplete(() =>
        {
            settingsCG.blocksRaycasts = false;
        });
    }
}
