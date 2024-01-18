using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private Text _scoreText;
    private int score = 0;
    private int basicScore = 100;
    private int bonusScore = 50;


    public void GameStart()
    {
        this._scoreText = GameObject.Find("ScoreText").GetComponent<Text>(); // text�R���|�[�l���g���擾
        _scoreText.text = score.ToString("N0");
    }

    // Update is called once per frame
    public void AddScoreForClearLine(int lineNumber)
    {
        score += basicScore;

        // �������C�������ꂽ�ꍇ
        if(lineNumber > 1 && lineNumber < 5)
        {
            score += bonusScore;
        }else if(lineNumber >= 5)
        {
            score += bonusScore * 2;
        }

        _scoreText.text = score.ToString("N0");
    }
}
