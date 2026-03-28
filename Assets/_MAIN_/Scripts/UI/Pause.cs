using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CanvasGroup panel;

    [Header("Animation References")]
    [SerializeField] List<CanvasGroup> buttonCgs;


    public void Show()
    {
        panel.blocksRaycasts = true;
        panel.DOFade(1f, 0.35f).OnComplete(() =>
        {
            StartCoroutine(ShowCoroutine());
        });
    }

    IEnumerator ShowCoroutine()
    {
        yield return DoTweenAnimations.FadeInOneByOne(buttonCgs, 0.35f, 0.1f);
    }

    public void Hide()
    {
        panel.blocksRaycasts = false;
        StartCoroutine(HideCoroutine());
    }

    IEnumerator HideCoroutine()
    {
        yield return DoTweenAnimations.FadeOutOneByOne(buttonCgs, 0.15f, 0.1f);
        panel.DOFade(0f, 0.25f).OnComplete(() =>
        {
            panel.blocksRaycasts = false;
        });
    }
}
