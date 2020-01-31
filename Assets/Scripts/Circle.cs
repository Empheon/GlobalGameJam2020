using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public Material CircleMat;
    public float AnimationDuration;


    private Queue<float> LineWidth;
    private Queue<float> Radius;

    private LineRenderer _line;
    //private bool _updateCircle = true;
    public bool _reduce = false;
    private float _reduceCounter = 0;
    private float _currentRadius;
    private float _currentWidth;

    private static float _small = 0.000001f;

    void Awake()
    {
        LineWidth = new Queue<float>();
        Radius = new Queue<float>();
        LineWidth.Enqueue(0.5f);
        LineWidth.Enqueue(0.5f);
        LineWidth.Enqueue(0.25f);
        Radius.Enqueue(3f);
        Radius.Enqueue(1.5f);
        Radius.Enqueue(0.75f);

        _line = gameObject.AddComponent<LineRenderer>();
        DrawCircle(Radius.Peek(), LineWidth.Peek());
    }

    // Update is called once per frame
    void Update()
    {
        //if (_updateCircle)
        //{
        //    _updateCircle = false;
        //    DrawCircle(Radius, LineWidth);
        //}
        // Asked to reduce
        if (_reduce)
        {
            _reduceCounter += Time.deltaTime / AnimationDuration;
            // if done reducing
            if (_reduceCounter >= 1)
            {
                _reduce = false;
                _reduceCounter = 0;
                if (LineWidth.Count == 0 && Radius.Count == 0)
                {
                    // trigger destroy level
                }
            } else
            {
                // reducing
                var newWidth = Mathf.Lerp(_currentWidth, LineWidth.Count > 0 ? LineWidth.Peek() : _small, _reduceCounter);
                var newRadius = Mathf.Lerp(_currentRadius, Radius.Count > 0 ? Radius.Peek() : _small, _reduceCounter);
                DrawCircle(newRadius, newWidth);
            }
        }
    }

    private void OnValidate()
    {
        // called when value in inspector is changed
        //_updateCircle = true;
    }

    public void Reduce()
    {
        //_updateCircle = true;
        _reduce = true;
        _currentRadius = Radius.Peek();
        _currentWidth = LineWidth.Peek();
        Debug.Log(_currentRadius + " " + _currentWidth);
        Radius.Dequeue();
        LineWidth.Dequeue();
    }

    private void DrawCircle(float radius, float lineWidth)
    {
        var segments = 500;
        _line.useWorldSpace = false;
        _line.startWidth = lineWidth;
        _line.endWidth = lineWidth;
        _line.positionCount = segments + 1;
        _line.material = CircleMat;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        }

        _line.SetPositions(points);
    }
}
