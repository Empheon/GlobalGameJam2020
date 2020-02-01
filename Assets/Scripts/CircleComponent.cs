using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CircleComponent : MonoBehaviour, ICircleComponent
    {

        public event CircleComponentReduceFinishedeHandler OnReduceFinished;

        private const float _small = 0.000001f;

        private float _animationDuration;

        private List<float> _lineWidth;
        private List<float> _radius;
        private Color _color;
        private Material _circleMat;

        private LineRenderer _line;
        private bool _reduce = false;
        private float _reduceCounter = 0;
        private float _currentRadius;
        private float _currentWidth;
        private bool _isToConstruct;
        private float[] _toConstructDegrees;
        private int _stepsNumber;
        public int StepsIndex = 0;
        public int? OnReduceFinishedEventCountInvocation => OnReduceFinished?.GetInvocationList().Length;

        public void Init(List<float> lineWidth, List<float> radius, Color color, Material circleMat, bool isToConstruct, float animationDuration, float[] toConstructDegrees)
        {
            
            _lineWidth = lineWidth;
            _radius = radius;
            _color = color;
            _animationDuration = animationDuration;
            _isToConstruct = isToConstruct;
            _circleMat = circleMat;
            _toConstructDegrees = toConstructDegrees;
            _line = gameObject.AddComponent<LineRenderer>();
            _stepsNumber = lineWidth.Count;

            _currentRadius = _radius[0];
            _currentWidth = _lineWidth[0];

            DrawCircle(_currentRadius, _currentWidth);
        }


        // Update is called once per frame
        void Update()
        {
            // Asked to reduce
            if (_reduce)
            {
                _reduceCounter += Time.deltaTime / _animationDuration;
                // if done reducing
                if (_reduceCounter >= 1)
                {
                    var newWidth = StepsIndex < _stepsNumber ? _lineWidth[StepsIndex] : _small;
                    var newRadius = StepsIndex < _stepsNumber ? _radius[StepsIndex] : _small;
                    DrawCircle(newRadius, newWidth);

                    _reduce = false;
                    _reduceCounter = 0;
                    OnReduceFinished?.Invoke(this);
                }
                else
                {
                    // reducing
                    var newWidth = Mathf.Lerp(_currentWidth, StepsIndex < _stepsNumber ? _lineWidth[StepsIndex] : _small, _reduceCounter);
                    var newRadius = Mathf.Lerp(_currentRadius, StepsIndex < _stepsNumber ? _radius[StepsIndex] : _small, _reduceCounter);
                    DrawCircle(newRadius, newWidth);
                }
            }
        }

        public void Reduce()
        {
            _reduce = true;
            _currentRadius = _radius[StepsIndex];
            _currentWidth = _lineWidth[StepsIndex];
            StepsIndex++;
        }

        private void DrawCircle(float radius, float lineWidth)
        {
            var segments = 600;
            _line.useWorldSpace = false;
            _line.startWidth = lineWidth;
            _line.endWidth = lineWidth;
            _line.material = _circleMat;
            _line.startColor = _color;
            _line.endColor = _color;

            var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
            var points = new List<Vector3>();

            for (int i = 0; i < pointCount; i++)
            {
                var deg = (i * 360f / segments);
                var shouldDraw = false;
                if (_isToConstruct)
                {
                    if (deg >= _toConstructDegrees[0] && deg <= _toConstructDegrees[1])
                    {
                        shouldDraw = true;
                    }
                }
                else
                {
                    shouldDraw = true;
                }
                if (shouldDraw)
                {
                    var rad = Mathf.Deg2Rad * (i * 360f / segments);
                    points.Add(new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0));
                }
            }
            var arr = points.ToArray();
            _line.positionCount = arr.Length;
            _line.SetPositions(arr);
        }

        public float[] GetDegrees()
        {
            return _toConstructDegrees;
        }

        public void UpdateDegreeIn(float a)
        {
            _toConstructDegrees[0] = a;
        }

        public void UpdateDegreeOut(float a)
        {
            _toConstructDegrees[1] = a;
        }

        public void RedrawCircle()
        {
            DrawCircle(_radius[StepsIndex], _lineWidth[StepsIndex]);
        }

    }
}
