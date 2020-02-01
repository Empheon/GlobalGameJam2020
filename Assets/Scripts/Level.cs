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

        public Level Next => _next;

        private Level _next;
        private Dictionary<Color, Circle> _circles = new Dictionary<Color, Circle>();

        /// <summary>
        /// Called only one time, the initialization of the game
        /// </summary>
        public void Init()
        {
            name = "Level-" + levelNumber;
            CreateInitCircles();
            InstantiateNext(true);
        }

        public void InstantiateNext(bool init = false)
        {
            _next = Instantiate(LevelGameObject.gameObject).GetComponent<Level>();
            levelNumber++;
            _next.name = "Level-" + levelNumber;
            _next.CreateCircles();
            _next.OnReduce += Reduce;
            if (init == true)
            {
                LifeTime--;
                return;
            }
            Reduce();
        }

        private void CreateCircles()
        {
            var circle = Instantiate(CircleGameObject.gameObject).GetComponent<Circle>();
            circle.name = "Circle-" + name;
            var lineWidth = new List<float>();
            var radius = new List<float>();
            lineWidth.Add(0.8f);
            lineWidth.Add(0.65f);
            lineWidth.Add(0.8f);
            radius.Add(2.5f);
            radius.Add(1.6f);
            radius.Add(0.7f);

            _circles.Add(circle.Init(lineWidth, radius, Mathf.Min(levelNumber / 4 + 1, 4)), circle);
        }

        public void Press(Color color, float angle, bool lastPress)
        {
            if (_circles.ContainsKey(color))
            {
                _circles[color].UpdatePress(360 - angle, lastPress);
            }
        }

        private void CreateInitCircles()
        {
            var circle = Instantiate(CircleGameObject.gameObject).GetComponent<Circle>();
            var lineWidth = new List<float>();
            var radius = new List<float>();
            lineWidth.Add(0.65f);
            lineWidth.Add(0.8f);
            radius.Add(1.6f);
            radius.Add(0.7f);
            _circles.Add(circle.Init(lineWidth, radius, 1), circle);
        }

        private void Reduce()
        {
            LifeTime--;
            foreach (var circle in _circles.Values)
            {
                circle.OnReduceFinished += ReduceFinished;
                circle.Reduce();
            }
            OnReduce?.Invoke();
            if (_circles.Count == 0)
                OnNextLevelReady?.Invoke();
        }

        private void ReduceFinished(Circle circle)
        {
            circle.OnReduceFinished -= ReduceFinished;
            foreach (var c in _circles.Values)
            {
                if (c.OnReduceFinishedEventCountInvocation != null && c.OnReduceFinishedEventCountInvocation >= 1)
                    return;
            }
            DestroyLevel();
            OnNextLevelReady?.Invoke();
        }

        private void DestroyLevel()
        {
            if (LifeTime > 0)
                return;
            foreach (var circle in _circles.Values)
            {
                Destroy(circle.gameObject);
            }
            _next.OnReduce -= Reduce;
            Destroy(gameObject);
        }

    }
}
