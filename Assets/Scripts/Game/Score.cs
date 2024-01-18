using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private Text _scoreText;
    private int score = 0;


    public void GameStart()
    {
        this._scoreText = GameObject.Find("ScoreText").GetComponent<Text>(); // textコンポーネントを取得
        _scoreText.text = score.ToString("N0");
    }

    // Update is called once per frame
    public void AddScoreForClearLine()
    {
        score += 1000;
        _scoreText.text = score.ToString("N0");
    }
}
