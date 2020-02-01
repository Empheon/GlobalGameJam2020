using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour, ICircle
{

    public event DestroyHandler OnDestroyFinished;

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
    private bool _destroying;

    public Circle(List<float> lineWidth, List<float> radius, Color color, List<float[]> toConstructDegrees)
    {
        _lineWidth = lineWidth;
        _radius = radius;
        _color = color;
        _toConstructDegrees = toConstructDegrees;
    }

    void Awake()
    {
        _lineWidth = new List<float>();
        _radius = new List<float>();
        _lineWidth.Add(0.75f);
        _lineWidth.Add(0.5f);
        _lineWidth.Add(0.25f);
        _radius.Add(3f);
        _radius.Add(1.5f);
        _radius.Add(0.75f);
        _color = Color.yellow;

        _toConstructDegrees = new List<float[]>();
        _toConstructDegrees.Add(new float[] { 20, 50 });
        _toConstructDegrees.Add(new float[] { 120, 200 });
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
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Reduce()
    {
        _circleBase.GetComponent<CircleComponent>().Reduce();
        foreach (var circle in _circlesToConstruct)
        {
            circle.GetComponent<CircleComponent>().Reduce();
        }
    }

    public void UpdatePress(float degree)
    {
        var shouldDestroy = true;
        bool lastPress = false;
        // redraw the to construct circles with the updated angles
        for (var i = 0; i < _circlesToConstruct.Count; i++)
        {
            var circle = _circlesToConstruct[i].GetComponent<CircleComponent>();
            var degs = circle.GetDegrees();
            if (degree > degs[0] && degree < degs[1])
            {
                // we tap in a to construct circle
                var prevDegree = degs[0];
                circle.UpdateDegreeIn(degree);
                circle.RedrawCircle();
                shouldDestroy = false;

                // if we were destroying just before, we need to stop
                if (_destroying)
                {
                    circle = _destructionCircleInConstruction.GetComponent<CircleComponent>();
                    circle.UpdateDegreeOut(prevDegree);
                    circle.RedrawCircle();
                    _destroying = false;
                    _destructedCircles.Add(_destructionCircleInConstruction);
                }

                break;
            }
        }


        // if it doesnt construct, it destroys on normal angles
        // we need to create a new circle
        if (shouldDestroy)
        {
            // we update the circle in destruction if we're already destroying it
            if (_destroying)
            {
                var circle = _destructionCircleInConstruction.GetComponent<CircleComponent>();
                circle.UpdateDegreeOut(degree);
                circle.RedrawCircle();
                if (lastPress)
                {
                    _destroying = false;
                    _destructedCircles.Add(_destructionCircleInConstruction);
                }
            }
            else
            {
                // else we create a destruction circle
                _destroying = true;
                _destructionCircleInConstruction = new GameObject("Circle to construct");
                _destructionCircleInConstruction.transform.parent = transform;
                _destructionCircleInConstruction.transform.position = new Vector3(0, 0, -5);
                var tcCC = _destructionCircleInConstruction.AddComponent<CircleComponent>();
                tcCC.Init(_lineWidth, _radius, _color, ToConstructMat, true, AnimationDuration, new float[] { degree, degree });
                tcCC.StepsIndex = _circleBase.GetComponent<CircleComponent>().StepsIndex;
            }

        }

        public void Destroy()
        {
            OnDestroyFinished?.Invoke(this);
            // todo: destroy the circle
        }

    }
}
