using UnityEngine.UI;

namespace Assets.Scripts
{

    public delegate void GameOverHandler();

    public class ScoreManager
    {

        public event GameOverHandler OnGameOver;
        public int CurrentScore;
        public float GaugePercent;
        private Slider _gaugeSlider;
        private Text _scoreText;
        private bool _isGameOver => GaugePercent <= 0;

        public ScoreManager(Slider gaugeSlider, Text scoreText)
        {
            GaugePercent = 0.5f;
            CurrentScore = 0;
            _gaugeSlider = gaugeSlider;
            _scoreText = scoreText;
            _gaugeSlider.value = GaugePercent;
            _scoreText.text = CurrentScore.ToString();
        }

        public void AddPoint()
        {
            CurrentScore++;
            _scoreText.text = CurrentScore.ToString();
        }

        public void BuiltAngle(float angle) => SetGaugeScore(angle);
        public void DestroyedAngle(float angle) => SetGaugeScore(angle, true);

        private void SetGaugeScore(float angle, bool negative = false)
        {
            var gaugeScore = angle / 360f;
            if (negative)
                gaugeScore *= -1;
            GaugePercent += gaugeScore;
            _gaugeSlider.value = GaugePercent;
            if (_isGameOver)
                GameOver();
        }

        private void GameOver()
        {
            OnGameOver?.Invoke();
        }

    }
}
