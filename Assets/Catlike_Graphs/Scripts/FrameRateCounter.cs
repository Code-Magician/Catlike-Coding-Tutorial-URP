using TMPro;
using UnityEngine;

public class FrameRateCounter : MonoBehaviour
{
    public enum DisplayMode { FPS, MS}

    #region Inspector Fields

    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField]
    DisplayMode displayMode;

    [SerializeField, Range(0.1f, 2f)]
    float sampleDuration = 1f;

    #endregion


    #region Private Fields

    int frames = 0;
    float duration = 0, bestDuration = float.MaxValue, worstDuration = 0;

    #endregion


    #region Mono Methods

    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;

        frames++;
        duration += frameDuration;

        if(frameDuration < bestDuration) 
            bestDuration = frameDuration;
        if(frameDuration > worstDuration)
            worstDuration = frameDuration;

        if (duration >= sampleDuration)
        {
            if (displayMode == DisplayMode.FPS)
            {
                display.SetText(
                    "FPS\n{0:0}\n{1:0}\n{2:0}", 
                    1f / bestDuration, 
                    frames / duration, 
                    1f / worstDuration
                );
            }
            else
            {
                display.SetText(
                    "MS\n{0:1}\n{1:1}\n{2:1}", 
                    1000f*bestDuration,
                    1000f*duration / frames, 
                    1000f*worstDuration
                );
            }

            frames = 0;
            duration = 0;
            bestDuration = float.MaxValue;
            worstDuration = 0;
        }
    }

    #endregion
}
