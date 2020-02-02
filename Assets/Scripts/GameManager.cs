using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static Color[] Colors = new Color[] { new Color(1, 0.65f, 0), Color.cyan };
        public List<Button> Buttons;
        public Slider Gauge;
        public Text ScoreText;
        public GameObject GameOverObject;

        [Header("Manager")]
        public static GameManager Instance;

        public Cursor CursorGameObject;
        public Level LevelGameObject;

        private Level _currentLevel;
        private Cursor _cursor;
        public ScoreManager ScoreManager;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            else if (Instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _cursor = Instantiate(CursorGameObject.gameObject).GetComponent<Cursor>();
            _currentLevel = Instantiate(LevelGameObject.gameObject).GetComponent<Level>();
            _currentLevel.OnNextLevelReady += NextLevelReady;
            _currentLevel.Init();
            _cursor.OnNewTurn += NextLevel;

            ScoreManager = new ScoreManager(Gauge, ScoreText);
            ScoreManager.OnGameOver += GameOver;

            for (var i = 0; i < Colors.Length; i++)
            {
                Buttons[i].gameObject.GetComponent<Image>().color = Colors[i];
                var color = Colors[i];
            }
        }

        private void GameOver()
        {
            Time.timeScale = .0f;
            GameOverObject.SetActive(true);
            GameOverObject.transform.SetAsLastSibling();
        }

        private void NextLevel()
        {
            ScoreManager.AddPoint(Level.levelNumber * 10);
            _currentLevel.OnNextLevelReady -= NextLevelReady;
            _currentLevel = _currentLevel.Next;
            _currentLevel.OnNextLevelReady += NextLevelReady;
            _currentLevel.InstantiateNext();
        }

        private void NextLevelReady()
        {
            _cursor.CursorState = CursorState.ACTIVE;
        }

        public void Touch(Color color, bool lastPress)
        {
            _currentLevel.Press(color, _cursor.CurrenAngleInDegree, lastPress);
        }

    }
}