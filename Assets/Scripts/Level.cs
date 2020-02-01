using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Level : MonoBehaviour, ILevel
    {

        public event ReduceHandler OnReduce;
        public event NextLevelReadyHandler OnNextLevelReady;

        public int LifeTime = 3;
        public float Radius;

        public Level LevelGameObject;

        private Level _next;
        private List<Circle> _circles = new List<Circle>();
        private bool _firstLevel = false;

        public void Init()
        {
            _firstLevel = true;
            InstantiateNext();
        }

        /// <summary>
        /// Go to next level
        /// </summary>
        /// <returns>return the next Level</returns>
        public Level NextLevel()
        {
            _next.InstantiateNext();
            return _next;
        }

        private void InstantiateNext()
        {
            _next = Instantiate(LevelGameObject.gameObject).GetComponent<Level>();
            _next.OnReduce += Reduce;
            Reduce();
        }

        private void Reduce()
        {
            OnReduce?.Invoke();
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
            OnNextLevelReady?.Invoke(this);
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
