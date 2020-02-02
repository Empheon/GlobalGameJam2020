using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public Text StartCounterText;

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
            Time.timeScale = 1f;
            StartCoroutine(StartDelayCoroutine());
        }

        public void StartGame()
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

        IEnumerator StartDelayCoroutine()
        {
            var waitTime = .5f;
            StartCounterText.gameObject.SetActive(true);
            StartCounterText.text = "3";
            yield return new WaitForSeconds(waitTime);
            StartCounterText.text = "2";
            yield return new WaitForSeconds(waitTime);
            StartCounterText.text = "1";
            yield return new WaitForSeconds(waitTime);
            StartCounterText.gameObject.SetActive(false);
            StartGame();
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

        public void Restart()
        {
            SceneManager.LoadScene("Scenes/Main", LoadSceneMode.Single);
            Destroy();
        }

        public void Quit()
        {
            SceneManager.LoadScene("Scenes/HomeScreen", LoadSceneMode.Single);
            Destroy();
        }

        private void Destroy()
        {
            Level.levelNumber = 0;
            Destroy(gameObject);
        }

    }
}