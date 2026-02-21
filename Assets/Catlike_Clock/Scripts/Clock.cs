using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] 
    GameObject hourIndicatorPrefab;

    [SerializeField] 
    Transform  hourArmPivot, minuteArmPivot, secondArmPivot;

    #endregion


    #region Private Fields

    float hoursToDegree = -360f / 12, minutesToDegree = -360f / 60, secondsToDegree = -360f / 60;

    #endregion


    #region Mono Methods

    private void Awake()
    {
        PlaceHourIndicator();
    }

    private void Update()
    {
        RunClock();
    }

    #endregion


    #region Clock Functions

    private void PlaceHourIndicator()
    {
        for(int i=30; i<=360; i+=30)
        {
            Vector3 pos = new Vector3(0, 0, -0.25f);
            pos.x = Mathf.Sin(Mathf.Deg2Rad * i) * 4f;
            pos.y = Mathf.Cos(Mathf.Deg2Rad * i) * 4f;

            GameObject hourIndicator = Instantiate(hourIndicatorPrefab, pos, Quaternion.Euler(0, 0, -i), transform);
            hourIndicator.name = $"Hour Indicator {i / 30}";
        }
    }

    private void RunClock()
    {
        var currentDateTime = DateTime.Now.TimeOfDay;

        hourArmPivot.localRotation = Quaternion.Euler(0, 0, hoursToDegree * (float) currentDateTime.TotalHours);
        minuteArmPivot.localRotation = Quaternion.Euler(0, 0, minutesToDegree * (float) currentDateTime.TotalMinutes);
        secondArmPivot.localRotation = Quaternion.Euler(0, 0, secondsToDegree * (float) currentDateTime.TotalSeconds);
    }

    #endregion
}
