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

        private Level _next;
        private List<Circle> _circles = new List<Circle>();

        public void Init()
        {
            name = "Level-" + levelNumber;
            InstantiateNext();
        }

        public void InstantiateNext()
        {
            _next = Instantiate(LevelGameObject.gameObject).GetComponent<Level>();
            levelNumber++;
            _next.name = "Level-" + levelNumber;
            _next.OnReduce += Reduce;
            Reduce();
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
            foreach (var circle in _circles)
            {
                circle.Reduce();
            }
            OnNextLevelReady?.Invoke(_next);
            OnReduce?.Invoke();
        }

        private void DestroyCircles()
        {
            foreach (var circle in _circles)
            {
                circle.OnDestroyFinished += RemoveCircle;
            }
        }

        private void RemoveCircle(Circle circle)
        {
            _circles.Remove(circle);
            if (_circles.Count == 0)
            {
                Destroy(gameObject);
            }
        }

    }
}
