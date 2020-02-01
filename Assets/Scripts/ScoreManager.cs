using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ScoreManager
    {

        public int CurrentScore;
        public float GaugePercent;
        private Slider _gaugeSlider;
        private Text _scoreText;

        public ScoreManager(Slider gaugeSlider, Text scoreText)
        {
            _gaugeSlider = gaugeSlider;
            _scoreText = scoreText;
        }

        public void AddPoint()
        {
            CurrentScore++;
            _scoreText.text = CurrentScore.ToString();
        }

        private void DestroyedAngle(float angle)
        {

        }

        private void BuiltAngle(float angle)
        {

        }

    }
}
