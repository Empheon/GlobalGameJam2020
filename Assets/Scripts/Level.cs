﻿using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Level : MonoBehaviour, ILevel
    {

        public event ReduceHandler OnReduce;

        public int LifeTime = 3;
        public float Radius;

        private Level _next;
        private List<Circle> _circles;

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
            _next = new Level();
            _next.OnReduce += Reduce;
            // todo: instantiate the circles
            Reduce();
        }

        private void Reduce()
        {
            LifeTime--;
            if (LifeTime == 0)
            {
                Destroy();
                return;
            }
            foreach (var circle in _circles)
            {
                circle.Reduce();
            }
        }

        private void Destroy()
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
