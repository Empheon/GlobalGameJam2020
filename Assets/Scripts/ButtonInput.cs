using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Image _img;

        private bool _touching = false;
        private bool _prevTouching = false;

        private void Start()
        {
            _img = GetComponent<Image>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _touching = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _touching = false;
        }

        public void Update()
        {
            if (_touching)
            {
                GameManager.Instance.Touch(_img.color, false);
            } else if (_prevTouching)
            {
                GameManager.Instance.Touch(_img.color, true);
            }
            _prevTouching = _touching;
        }
    }
}
