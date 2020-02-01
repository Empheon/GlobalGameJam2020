using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts
{
    public class Circle : MonoBehaviour, ICircle
    {

        public event DestroyHandler OnDestroyFinished;

        public void Reduce()
        {

        }

        public void Destroy()
        {
            OnDestroyFinished?.Invoke(this);
            // todo: destroy the circle
        }

    }
}
