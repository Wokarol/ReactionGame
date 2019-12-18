using UnityEngine;

public class HighScoreController : MonoBehaviour
{
    [Tooltip("{0} will be replaced with score, and {1} will be replaced with highscore")]
    [SerializeField, TextArea(2, 5)] private string scoreTextFormat = "";
    [SerializeField] private TMPro.TMP_Text scoreText;

    private void Awake()
    {
        int score = Game.SessionEndData.Score;
        int highscore = PlayerPrefs.GetInt(SaveTags.Highscore);

        if(highscore < score) {
            highscore = score;
            PlayerPrefs.SetInt(SaveTags.Highscore, highscore);
        }

        scoreText.text = string.Format(scoreTextFormat, score, highscore);
    }
}