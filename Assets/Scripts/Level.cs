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
        private Dictionary<Color, Circle> _circles = new Dictionary<Color, Circle>();

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
            _next.CreateCircles();
            _next.OnReduce += Reduce;
            Reduce();
        }

        private void CreateCircles()
        {
            var circle = Instantiate(CircleGameObject.gameObject).GetComponent<Circle>();

            var lineWidth = new List<float>();
            var radius = new List<float>();
            lineWidth.Add(0.75f);
            lineWidth.Add(0.5f);
            lineWidth.Add(0.25f);
            radius.Add(3f);
            radius.Add(1.5f);
            radius.Add(0.75f);

            _circles.Add(circle.Init(lineWidth, radius), circle);
        }

        public void Press(Color color, float angle, bool lastPress)
        {
            //Debug.Log(gameObject.name +" "+ color + " " + _circles.Keys);
            if (_circles.ContainsKey(color))
            {
                _circles[color].UpdatePress(360-angle, lastPress);
            }
        }

        private void Reduce()
        {
            LifeTime--;
            if (LifeTime == 0)
            {
                DestroyCircles();
                Destroy(gameObject);
                return;
            }
            foreach (var circle in _circles.Values)
            {
                circle.Reduce();
            }
            OnNextLevelReady?.Invoke(_next);
            OnReduce?.Invoke();
        }

        private void DestroyCircles()
        {
            foreach (var circle in _circles.Values)
            {
                circle.OnDestroyFinished += RemoveCircle;
            }
        }

        private void RemoveCircle(Circle circle)
        {
            //_circles.Remove(circle);
            //if (_circles.Count == 0)
            //{
            //    Destroy(gameObject);
            //}
        }

    }
}
