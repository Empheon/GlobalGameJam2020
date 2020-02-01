using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        [Header("Manager")]
        public static GameManager Instance;

        public Cursor Cursor;

        private Level _currentLevel;

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
            Cursor.OnNewTurn += NextLevel;
        }

        private void NextLevel()
        {
            _currentLevel = _currentLevel.NextLevel();
        }

    }
}