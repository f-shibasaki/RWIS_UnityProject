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


    public void GameStart()
    {
        this._scoreText = GameObject.Find("ScoreText").GetComponent<Text>(); 
        _scoreText.text = score.ToString("N0");
    }

    // Update is called once per frame
    public void AddScoreForClearLine(int lineNumber)
    {
        score += basicScore;

        // •¡”ƒ‰ƒCƒ“Á‚³‚ê‚½ê‡
        if(lineNumber > 1 && lineNumber < 5)
        {
            score += bonusScore;
        }else if(lineNumber >= 5)
        {
            score += bonusScore * 2;
        }

        _scoreText.text = score.ToString("N0");
    }

    public void ResultScore()
    {
        this._resultScoreText = GameObject.Find("ResultScoreText").GetComponent<Text>();
        _resultScoreText.text = score.ToString("N0");
    }
}
