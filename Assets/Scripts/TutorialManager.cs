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
    public Action BackHomeDelegate { get; set; }

    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        
        // 自分の子オブジェクトにUIを生成
        Instantiate(_tutorialUIPrefab, transform);
        _tutorialUIController = GetComponentInChildren<TutorialUIController>();
        _tutorialUIController.FinishHorizontalTutorialDelegate = RotateTutorial;
        _tutorialUIController.FinishRotationTutorialDelegate = HoldTutorial;
        _tutorialUIController.FinishHoldTutorialDelegate = SwipeTutorial;
        _tutorialUIController.FinishTutorialDelegate = FinishTutorial;
        _tutorialUIController.BackToHorizontalTutorialDelegate = MoveTutorial;
        _tutorialUIController.BackToRotationTutorialDelegate = RotateTutorial;
        _tutorialUIController.BackToHoldTutorialDelegate = HoldTutorial;
        _tutorialUIController.BackHomeDelegate = BackHome;

        // チュートリアル開始
        MoveTutorial();
        _tutorialUIController.OnStartTutorial();
    }
    

    void MoveTutorial()
    {
        _gameManager.PrepareForTutorial();

        Invoke("ResetGyro", 1f);
        _gameManager.EnableHorizontalMove();
    }
    
    void RotateTutorial()
    {
        _gameManager.PrepareForTutorial();
        
        _gameManager.EnableHorizontalMove();
        _gameManager.ResetGyro();
        _gameManager.EnableRotation();
    }
    
    void HoldTutorial()
    {
        _gameManager.PrepareForTutorial();

        _gameManager.EnableHorizontalMove();
        _gameManager.EnableRotation();
        _gameManager.EnableHold();
    }
    
    void SwipeTutorial()
    {
        _gameManager.EnableDrop();
        _gameManager.EnableSwipe();
    }
    
    void FinishTutorial()
    {
        FinishTutorialDelegate.Invoke();
    }
    
    void BackHome()
    {
        BackHomeDelegate.Invoke();
    }
    
    void ResetGyro()
    {
        _gameManager.ResetGyro();
    }
}
