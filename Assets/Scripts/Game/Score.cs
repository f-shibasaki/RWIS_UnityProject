using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private Text _scoreText;
    private Text _resultScoreText;
    private int score = 0;
    private int basicScore = 100;
    private int bonusScore = 50;

    public void Awake()
    {
        GameObject _UI = GameObject.Find("UI");
        _scoreText = _UI.transform.Find("GamePanel/ScoreText").gameObject.GetComponent<Text>();
        _resultScoreText = _UI.transform.Find("GameOverPanel/ResultScoreText").gameObject.GetComponent<Text>();
    }

    public void GameStart()
    {
        _scoreText.text = score.ToString("N0");
    }

    // Update is called once per frame
    public void AddScoreForClearLine(int lineNumber)
    {
        score += basicScore;

        // •¡”ƒ‰ƒCƒ“Á‚³‚ê‚½ê‡
        if(lineNumber > 1 && lineNumber < 4)
        {
            score += bonusScore;
        }else if(lineNumber == 4)
        {
            score += bonusScore * 2;
        }
        _scoreText.text = score.ToString("N0");
    }

    public void ResultScore()
    {
        _resultScoreText.text = score.ToString("N0");
    }
}
