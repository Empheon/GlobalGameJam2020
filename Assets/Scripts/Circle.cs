using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Circle : MonoBehaviour, ICircle
    {

        public event CircleReduceFinishedeHandler OnReduceFinished;

        public Material CircleMat;
        public Material ToConstructMat;
        public float AnimationDuration;

        private List<float[]> _toConstructDegrees;

        private List<float> _lineWidth;
        private List<float> _radius;
        private Color _color;

        private GameObject _circleBase;
        private List<GameObject> _circlesToConstruct;
        private List<GameObject> _destructedCircles;

        private GameObject _destructionCircleInConstruction;
        private GameObject _creationCircleInConstruction;
        private bool _destroying;
        private bool _creating;
        private float _creationEndDegree;

        private const float MIN_START_RADIUS = 50f;

        public int? OnReduceFinishedEventCountInvocation => OnReduceFinished?.GetInvocationList().Length;

        public Color Init(List<float> lineWidth, List<float> radius, int difficultness)
        {
            _lineWidth = lineWidth;
            _radius = radius;
            _color = GameManager.Colors[Random.Range(0, GameManager.Colors.Length)];

            _toConstructDegrees = new List<float[]>();

            var minrad = (80 / difficultness);
            var maxrad = (150 / difficultness);
            for (var i = 0; i < difficultness; i++)
            {
                var cond = true;
                while (cond)
                {
                    float angle1 = Random.Range(MIN_START_RADIUS, 359 - minrad - maxrad);
                    float angle2 = Random.Range(angle1 + minrad, angle1 + maxrad);
                    if (_toConstructDegrees.Count == 0)
                    {
                        _toConstructDegrees.Add(new float[] { angle1, angle2 });
                        break;
                    }
                    var createAngle = true;
                    foreach (var ang in _toConstructDegrees)
                    {
                        if (!(Mathf.Abs(angle1 - ang[0]) > minrad && Mathf.Abs(angle2 - ang[1]) > minrad &&
                            Mathf.Abs(angle2 - ang[0]) > minrad && Mathf.Abs(angle1 - ang[1]) > minrad &&
                            !(angle1 > ang[0] && angle1 < ang[1]) &&
                            !(angle2 > ang[0] && angle2 < ang[1]) &&
                            !(ang[0] > angle1 && ang[0] < angle2) &&
                            !(ang[1] > angle1 && ang[1] < angle2)))
                        {
                            createAngle = false;
                            break;
                        }
                    }
                    if (createAngle)
                    {
                        _toConstructDegrees.Add(new float[] { Mathf.Floor(angle1), Mathf.Floor(angle2) });
                        cond = false;
                    }
                }
            }
            //_toConstructDegrees.Clear();
            //_toConstructDegrees.Add(new float[] { 20, 50 });
            //_toConstructDegrees.Add(new float[] { 120, 200 });
            //_toConstructDegrees.Add(new float[] { 60, 80 });
            //_toConstructDegrees.Add(new float[] { 220, 310 });
            //////////////// above is temporary stuff

            _circlesToConstruct = new List<GameObject>();
            _destructedCircles = new List<GameObject>();
            _circleBase = new GameObject("Circle base");
            _circleBase.transform.parent = transform;


            var baseCC = _circleBase.AddComponent<CircleComponent>();
            baseCC.Init(_lineWidth, _radius, _color, CircleMat, false, AnimationDuration, null);

            foreach (var section in _toConstructDegrees)
            {
                var ctc = new GameObject("Circle to construct");
                ctc.transform.parent = transform;
                ctc.transform.position = new Vector3(0, 0, -5);
                var tcCC = ctc.AddComponent<CircleComponent>();
                tcCC.Init(_lineWidth, _radius, _color, ToConstructMat, true, AnimationDuration, section);

                _circlesToConstruct.Add(ctc);
            }

            return _color;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Reduce()
        {
            _circleBase.GetComponent<CircleComponent>().Reduce();

            if (_creating)
            {
                _creating = false;
                _circlesToConstruct.Add(_creationCircleInConstruction);
            }

            foreach (var circle in _circlesToConstruct)
            {
                var circleComponent = circle.GetComponent<CircleComponent>();
                circleComponent.OnReduceFinished += ReduceFinished;
                circleComponent.Reduce();
            }

            if (_destroying)
            {
                _destroying = false;
                _destructedCircles.Add(_destructionCircleInConstruction);
            }

            foreach (var circle in _destructedCircles)
            {
                var circleComponent = circle.GetComponent<CircleComponent>();
                circleComponent.OnReduceFinished += ReduceFinished;
                circleComponent.Reduce();
            }
        }

        private void ReduceFinished(CircleComponent circleComponent)
        {
            circleComponent.OnReduceFinished -= ReduceFinished;
            foreach (var circle in _circlesToConstruct)
            {
                var cc = circle.GetComponent<CircleComponent>();
                if (cc.OnReduceFinishedEventCountInvocation != null && cc.OnReduceFinishedEventCountInvocation >= 1)
                    return;
            }
            OnReduceFinished?.Invoke(this);
        }

        public void UpdatePress(float degree, bool lastPress)
        {
            var shouldDestroy = true;
            if (_creating)
            {
                shouldDestroy = false;
                var circle = _creationCircleInConstruction.GetComponent<CircleComponent>();
                GameManager.Instance.ScoreManager.BuiltAngle(degree - circle.GetDegrees()[0]);
                circle.UpdateDegreeIn(degree);
                circle.RedrawCircle();
                if (lastPress || degree > _creationEndDegree)
                {
                    _creating = false;
                    _circlesToConstruct.Add(_creationCircleInConstruction);
                }
                if (degree > _creationEndDegree)
                {
                    shouldDestroy = true;
                }
            } else
            {
                // redraw the to construct circles with the updated angles
                for (var i = 0; i < _circlesToConstruct.Count; i++)
                {
                    var circle = _circlesToConstruct[i].GetComponent<CircleComponent>();
                    var degs = circle.GetDegrees();
                    if (degree > degs[0] && degree < degs[1])
                    {

                        // if the diff is so small we should'nt create 2 separate circles
                        if (Mathf.Abs(degree - degs[0]) < 0.5)
                        {
                            GameManager.Instance.ScoreManager.BuiltAngle(degree - circle.GetDegrees()[0]);
                            circle.UpdateDegreeIn(degree);
                        } else
                        {
                            _creating = true;
                            _creationCircleInConstruction = new GameObject("Circle to construct");
                            _creationCircleInConstruction.transform.parent = transform;
                            _creationCircleInConstruction.transform.position = new Vector3(0, 0, -5);
                            var tcCC = _creationCircleInConstruction.AddComponent<CircleComponent>();
                            var endDeg = degs[1];
                            tcCC.StepsIndex = _circleBase.GetComponent<CircleComponent>().StepsIndex;
                            tcCC.Init(_lineWidth, _radius, _color, ToConstructMat, true, AnimationDuration, new float[] { degree, endDeg });
                            _creationEndDegree = endDeg;
                            tcCC.RedrawCircle();
                            circle.UpdateDegreeOut(degree);
                        }
                        // we tap in a to construct circle
                        var prevDegree = degs[0];
                        circle.RedrawCircle();
                        shouldDestroy = false;

                        // if we were destroying just before, we need to stop
                        if (_destroying)
                        {
                            circle = _destructionCircleInConstruction.GetComponent<CircleComponent>();
                            GameManager.Instance.ScoreManager.DestroyedAngle(degree - circle.GetDegrees()[1]);
                            circle.UpdateDegreeOut(degree);
                            circle.RedrawCircle();
                            _destroying = false;
                            _destructedCircles.Add(_destructionCircleInConstruction);
                        }

                        break;
                    }
                }
            }

            // if it doesnt construct, it destroys on normal angles
            // we need to create a new circle
            if (shouldDestroy)
            {
                _creating = false;
                // we update the circle in destruction if we're already destroying it
                if (_destroying)
                {
                    var circle = _destructionCircleInConstruction.GetComponent<CircleComponent>();
                    GameManager.Instance.ScoreManager.DestroyedAngle(degree - circle.GetDegrees()[1]);
                    circle.UpdateDegreeOut(degree);
                    circle.RedrawCircle();
                    if (lastPress)
                    {
                        _destroying = false;
                        _destructedCircles.Add(_destructionCircleInConstruction);
                    }
                } else
                {
                    // else we create a destruction circle
                    _destroying = true;
                    _destructionCircleInConstruction = new GameObject("Circle to construct");
                    _destructionCircleInConstruction.transform.parent = transform;
                    _destructionCircleInConstruction.transform.position = new Vector3(0, 0, -5);
                    var tcCC = _destructionCircleInConstruction.AddComponent<CircleComponent>();
                    tcCC.StepsIndex = _circleBase.GetComponent<CircleComponent>().StepsIndex;
                    tcCC.Init(_lineWidth, _radius, _color, ToConstructMat, true, AnimationDuration, new float[] { degree, degree });
                }

            }

            if (lastPress)
            {
                _creating = false;
                _destroying = false;
            }
        }

    }
}
