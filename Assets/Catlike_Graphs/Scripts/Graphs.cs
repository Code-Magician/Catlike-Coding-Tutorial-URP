using Unity.VisualScripting;
using UnityEngine;

public class Graphs : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 200)]
    int resolution = 10;

    Transform[] points;

    private void Awake()
    {
        DrawGraph();
    }

    private void Update()
    {
        AnimateGraph();
    }


    private void DrawGraph()
    {
        points = new Transform[resolution];

        var step = 2f / resolution;
        var position = Vector3.zero;
        var scale = Vector3.one * step;

        for(int i=0; i<points.Length; i++)
        {
            Transform pointTr = points[i] = Instantiate(pointPrefab, transform);
            
            position.x = ((i + 0.5f) * step - 1f);
            position.y = position.x * position.x * position.x;

            pointTr.localPosition = position;
            pointTr.localScale = scale;
        }
    }

    private void AnimateGraph()
    {
        float time = Time.time;
        for (int i = 0; i < points.Length; i++)
        {
            var position = points[i].localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.x + time));

            points[i].localPosition = position;
        }
    }
}
