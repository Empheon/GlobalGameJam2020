using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public Material CircleMat;
    public Material ToConstructMat;
    public float AnimationDuration;

    private List<float[]> _toConstructDegrees;

    private List<float> _lineWidth;
    private List<float> _radius;
    private Color _color;

    private GameObject _circleBase;
    private List<GameObject> _circleToConstruct;

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
        _lineWidth.Add(0.5f);
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

        _circleToConstruct = new List<GameObject>();
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

            _circleToConstruct.Add(ctc);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Reduce()
    {
        _circleBase.GetComponent<CircleComponent>().Reduce();
        foreach (var circle in _circleToConstruct)
        {
            circle.GetComponent<CircleComponent>().Reduce();
        }
    }
}
