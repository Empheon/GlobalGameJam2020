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

        /// <summary>
        /// Called only one time, the initialization of the game
        /// </summary>
        public void Init()
        {
            name = "Level-" + levelNumber;
            //CreateInitCircles();
            InstantiateNext();
        }

        public void InstantiateNext()
        {
            _next = Instantiate(LevelGameObject.gameObject).GetComponent<Level>();
            levelNumber++;
            _next.name = "Level-" + levelNumber;
            _next.CreateCircles();
            _next.OnReduce += Reduce;
            Reduce();
        }

        private void CreateCircles()
        {
            var circle = Instantiate(CircleGameObject.gameObject).GetComponent<Circle>();
            circle.name = "Circle-" + name;
            var lineWidth = new List<float>();
            var radius = new List<float>();
            lineWidth.Add(0.75f);
            lineWidth.Add(0.5f);
            lineWidth.Add(0.25f);
            radius.Add(3f);
            radius.Add(1.5f);
            radius.Add(0.75f);
            circle.Init(lineWidth, radius);
            _circles.Add(circle);
        }

        private void CreateInitCircles()
        {
            var circle = Instantiate(CircleGameObject.gameObject).GetComponent<Circle>();
            var lineWidth = new List<float>();
            var radius = new List<float>();
            lineWidth.Add(0.5f);
            lineWidth.Add(0.25f);
            radius.Add(1.5f);
            radius.Add(0.75f);
            circle.Init(lineWidth, radius);
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
            DestroyLevel();
            OnNextLevelReady?.Invoke(_next);
        }

        private void DestroyLevel()
        {
            if (LifeTime > 0)
                return;
            foreach (var circle in _circles)
            {
                Destroy(circle.gameObject);
            }
            _next.OnReduce -= Reduce;
            Destroy(gameObject);
        }

    }
}
