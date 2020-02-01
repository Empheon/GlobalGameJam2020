using Assets.Scripts.Enums;
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
        /// Thickness of the line
        /// </summary>
        public float Thickness = 1f;
        /// <summary>
        /// Color of the line
        /// </summary>
        public Color Color;
        /// <summary>
        /// Distance from the center of the circle
        /// </summary>
        public float PositionOffset;
        /// <summary>
        /// Current state of the cursor, turning or not (active/inactive)
        /// </summary>
        public CursorState CursorState;
        /// <summary>
        /// Direction of the cursor
        /// </summary>
        public Vector3 Direction = Vector3.back;
        /// <summary>
        /// Rotation speed of the cursor
        /// </summary>
        public float RotationSpeed = 30f;
        private bool _secondHalf = false;
        private LineRenderer line;

        /// <summary>
        /// Angle of the cursor in degree
        /// </summary>
        public float CurrenAngleInDegree => transform.rotation.eulerAngles.z;

        private void Start()
        {
            // todo: instantiate new line
            var go = new GameObject();
            go.transform.parent = transform;
            line = go.AddComponent<LineRenderer>();
            line.startWidth = Thickness;
            line.endWidth = Thickness;
            line.startColor = Color;
            line.endColor = Color;
            line.SetPosition(0, new Vector3(0, PositionOffset, 0));
            line.SetPosition(1, new Vector3(0, PositionOffset + Length, 0));
            line.useWorldSpace = false;
        }

        private void FixedUpdate()
        {
            if (CursorState == CursorState.ACTIVE)
            {
                transform.Rotate(Direction * RotationSpeed * Time.deltaTime);
                if (TurnDone(CurrenAngleInDegree, OneTurnDegree))
                    NewTurn();
            }
        }

        private bool TurnDone(float currentAngle, float oneTurnDegree)
        {
            var _lastSecondHalfValue = _secondHalf;
            if (currentAngle <= 180)
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
