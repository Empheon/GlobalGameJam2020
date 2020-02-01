using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Level : MonoBehaviour, ILevel
    {

        public static int levelNumber = 0;

        public event ReduceHandler OnReduce;
        public event NextLevelReadyHandler OnNextLevelReady;

        public int LifeTime = 3;
        public float Radius;

        public Level LevelGameObject;
        public Circle CircleGameObject;

        private Level _next;
        private List<Circle> _circles = new List<Circle>();

        public void Init()
        {
            name = "Level-" + levelNumber;
            InstantiateNext();
            CreateCircles();
        }

        public void InstantiateNext()
        {
            _next = Instantiate(LevelGameObject.gameObject).GetComponent<Level>();
            levelNumber++;
            _next.name = "Level-" + levelNumber;
            // todo: multiple Circles is possible here
            _next.CreateCircles();
            _next.OnReduce += Reduce;
            Reduce();
        }

        private void CreateCircles()
        {
            var circle = Instantiate(CircleGameObject.gameObject, transform).GetComponent<Circle>();
            _circles.Add(circle);
        }

        private void Reduce()
        {
            LifeTime--;
            OnReduce?.Invoke();
            foreach (var circle in _circles)
            {
                circle.OnReduceFinished += ReduceFinished;
                circle.Reduce();
            }
            if (_circles.Count == 0)
                OnNextLevelReady?.Invoke(_next);
        }

        private void ReduceFinished(Circle circle)
        {
            circle.OnReduceFinished -= ReduceFinished;
            foreach (var c in _circles)
            {
                if (c.OnReduceFinishedEventCountInvocation != null && c.OnReduceFinishedEventCountInvocation >= 1)
                    return;
            }
            RemoveCircle(circle);
            OnNextLevelReady?.Invoke(_next);
        }

        private void RemoveCircle(Circle circle)
        {
            if (LifeTime > 0)
                return;
            _next.OnReduce -= Reduce;
            Destroy(gameObject);
        }

    }
}
