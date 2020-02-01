﻿using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts
{
    public class Cursor : MonoBehaviour, ICursor
    {

        private const float OneTurnDegree = 360f;

        public event NewTurnHandler OnNewTurn;
        /// <summary>
        /// Length of the cursor, Length = radius, and PositionOffset = 0 to have a radius line
        /// </summary>
        public float Length;
        /// <summary>
        /// Distance from the center of the circle
        /// </summary>
        public float PositionOffset;
        /// <summary>
        /// Current state of the cursor, turning or not (active/inactive)
        /// </summary>
        public CursorState CursorState;
        /// <summary>
        /// Rotation speed of the cursor
        /// </summary>
        public float RotationSpeed = 30f;
        private bool _secondHalf = false;

        /// <summary>
        /// Angle of the cursor in degree
        /// </summary>
        //public float CurrenAngleInDegree => transform.rotation.eulerAngles.z
        public float CurrenAngleInDegree => transform.rotation.eulerAngles.z;

        private void Start()
        {
            // todo: instantiate new line
        }

        private void FixedUpdate()
        {
            if (CursorState == CursorState.ACTIVE)
            {
                transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime);
                if (TurnDone(CurrenAngleInDegree, OneTurnDegree))
                    NewTurn();
            }
        }

        private bool TurnDone(float currentAngle, float oneTurnDegree)
        {
            var _lastSecondHalfValue = _secondHalf;
            if (currentAngle >= 180)
                _secondHalf = true;
            else
                _secondHalf = false;
            return _lastSecondHalfValue == true && _secondHalf == false;
        }

        private void NewTurn()
        {
            CursorState = CursorState.INACTIVE;
            transform.Rotate(new Vector3(0, 0, 0));
            OnNewTurn?.Invoke();
        }

    }
}
