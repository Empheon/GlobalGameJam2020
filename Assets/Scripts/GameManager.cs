using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        [Header("Manager")]
        public static GameManager Instance;

        public Cursor CursorGameObject;
        public Level LevelGameObject;

        private Level _currentLevel;
        private Cursor _cursor;

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
            _cursor.OnNewTurn += NextLevel;
            _currentLevel = Instantiate(LevelGameObject.gameObject).GetComponent<Level>();
            _currentLevel.Init();
        }

        private void NextLevel()
        {
            _currentLevel = _currentLevel.NextLevel();
            _currentLevel.OnNextLevelDone += NextLevelDone;
        }

        private void NextLevelDone()
        {
            _cursor.CursorState = CursorState.ACTIVE;
        }

    }
}