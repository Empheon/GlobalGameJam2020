using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public Material CircleMat;
    public float AnimationDuration;

    private Queue<float> _lineWidth;
    private Queue<float> _radius;
    private Color _color;

    private LineRenderer _line;
    private bool _reduce = false;
    private float _reduceCounter = 0;
    private float _currentRadius;
    private float _currentWidth;

    private static float _small = 0.000001f;

    public Circle(Queue<float> lineWidth, Queue<float> radius, Color color)
    {
        _lineWidth = lineWidth;
        _radius = radius;
        _color = color;
    }

    void Awake()
    {
        _lineWidth = new Queue<float>();
        _radius = new Queue<float>();
        _lineWidth.Enqueue(0.5f);
        _lineWidth.Enqueue(0.5f);
        _lineWidth.Enqueue(0.25f);
        _radius.Enqueue(3f);
        _radius.Enqueue(1.5f);
        _radius.Enqueue(0.75f);
        _color = Color.yellow;

        _line = gameObject.AddComponent<LineRenderer>();
        DrawCircle(_radius.Peek(), _lineWidth.Peek());
    }

    // Update is called once per frame
    void Update()
    {
        // Asked to reduce
        if (_reduce)
        {
            _reduceCounter += Time.deltaTime / AnimationDuration;
            // if done reducing
            if (_reduceCounter >= 1)
            {
                _reduce = false;
                _reduceCounter = 0;
                if (_lineWidth.Count == 0 && _radius.Count == 0)
                {
                    // trigger destroy level
                }
            } else
            {
                // reducing
                var newWidth = Mathf.Lerp(_currentWidth, _lineWidth.Count > 0 ? _lineWidth.Peek() : _small, _reduceCounter);
                var newRadius = Mathf.Lerp(_currentRadius, _radius.Count > 0 ? _radius.Peek() : _small, _reduceCounter);
                DrawCircle(newRadius, newWidth);
            }
        }
    }

    public void Reduce()
    {
        _reduce = true;
        _currentRadius = _radius.Peek();
        _currentWidth = _lineWidth.Peek();
        _radius.Dequeue();
        _lineWidth.Dequeue();
    }

    private void DrawCircle(float radius, float lineWidth)
    {
        var segments = 500;
        _line.useWorldSpace = false;
        _line.startWidth = lineWidth;
        _line.endWidth = lineWidth;
        _line.positionCount = segments + 1;
        _line.material = CircleMat;
        _line.material.color = _color;

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
