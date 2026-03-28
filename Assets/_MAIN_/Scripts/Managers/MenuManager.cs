using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] string loadScene;

    [Space(20), Header("AnimationReferences")]
    [SerializeField] RectTransform titleRect;
    [SerializeField] RectTransform subtitleRect;
    [SerializeField] List<CanvasGroup> buttonCgs;


    private void Start()
    {
        StartCoroutine(AnimateUI());
    }

    private IEnumerator AnimateUI()
    {
        yield return DoTweenAnimations.MoveFromUp(titleRect, 1000, 1f);
        yield return DoTweenAnimations.MoveFromUp(subtitleRect, 1000, 0.5f);
        yield return DoTweenAnimations.FadeInOneByOne(buttonCgs, 0.5f, 0.2f);
    }

    public void StartGame()
    {
        Persisting.Instance.LoadScene(loadScene);
    }

    public void ShowSettings()
    {
        Persisting.Instance.ShowSettings();
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
