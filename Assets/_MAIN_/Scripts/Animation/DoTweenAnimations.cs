using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class DoTweenAnimations
{
    public static IEnumerator MoveFromUp(RectTransform rect,
                                         float offset,
                                         float duration)
    {
        Vector3 startPos = rect.anchoredPosition;

        // Move above screen
        rect.anchoredPosition = startPos + Vector3.up * offset;
        rect.gameObject.SetActive(true);

        Tween tween = rect
            .DOAnchorPos(startPos, duration)
            .SetEase(Ease.OutBack);

        yield return tween.WaitForCompletion();
    }


    public static IEnumerator FadeInOneByOne(List<CanvasGroup> canvasGroups,
                                             float fadeDuration,
                                             float delayBetween)
    {
        foreach (CanvasGroup cg in canvasGroups)
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;

            Tween tween = cg.DOFade(1, fadeDuration);

            yield return tween.WaitForCompletion();

            cg.blocksRaycasts = true;

            yield return new WaitForSeconds(delayBetween);
        }
    }

    public static IEnumerator FadeOutOneByOne(List<CanvasGroup> canvasGroups,
                                             float fadeDuration,
                                             float delayBetween)
    {
        foreach (CanvasGroup cg in canvasGroups)
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;

            Tween tween = cg.DOFade(0, fadeDuration);

            yield return tween.WaitForCompletion();

            yield return new WaitForSeconds(delayBetween);
        }
    }
}