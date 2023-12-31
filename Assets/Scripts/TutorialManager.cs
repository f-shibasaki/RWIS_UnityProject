using System;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class TutorialManager : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField]
    private GameObject _tutorialUIPrefab;
    
    private TutorialUIController _tutorialUIController;
    
    public Action FinishTutorialDelegate { get; set; }

    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        
        _gameManager.PrepareForTutorial();
        
        // 自分の子オブジェクトにUIを生成
        Instantiate(_tutorialUIPrefab, transform);
        _tutorialUIController = GetComponentInChildren<TutorialUIController>();
        _tutorialUIController.FinishHorizontalTutorialDelegate = RotateTutorial;
        _tutorialUIController.FinishTutorialDelegate = FinishTutorial;
        
        // チュートリアル開始
        MoveTutorial();
        _tutorialUIController.OnStartTutorial();
    }
    

    void MoveTutorial()
    {
        Invoke("ResetGyro", 1f);
        _gameManager.EnableHorizontalMove();
    }
    
    void RotateTutorial()
    {
        _gameManager.ResetGyro();
        _gameManager.EnableRotation();
    }
    
    void FinishTutorial()
    {
        FinishTutorialDelegate.Invoke();
    }
    
    void ResetGyro()
    {
        _gameManager.ResetGyro();
    }
}
