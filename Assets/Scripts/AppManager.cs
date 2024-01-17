using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    GameObject _gamePrefab;
    
    [SerializeField]
    GameObject _tutorialPrefab;
    
    [SerializeField]
    UIController _uiController;

    private GameManager _gameManager;
    private TutorialManager _tutorialManager;

    void Start()
    {
        _uiController.StartGameDelegate = StartGame;
        _uiController.StartTutorialDelegate = StartTutorial;
        _uiController.PauseGameDelegate = PauseGame;
        _uiController.ResumeGameDelegate = ResumeGame;
        _uiController.ResetGyroDelegate = ResetGyro;
    }
    
    void StartTutorial()
    {
        if (_tutorialManager != null)
        {
            Destroy(_tutorialManager.gameObject);
        }
        _tutorialManager = Instantiate(_tutorialPrefab).GetComponent<TutorialManager>();
        _tutorialManager.FinishTutorialDelegate = FinishTutorial;
    }
    
    void FinishTutorial()
    {
        _tutorialManager.FinishTutorialDelegate = null;
        Destroy(_tutorialManager.gameObject);
        StartGame();
    }

    void StartGame()
    {
        if (_gameManager != null)
        {
            Destroy(_gameManager.gameObject);
        }
        SetGame();
        _gameManager.Resume();
        SoundManager.instance.PlayBGM();
    }

    void SetGame()
    {
        _gameManager = Instantiate(_gamePrefab).GetComponent<GameManager>();
        _gameManager.OnGameOverDelegate = OnGameOver;
    }
    
    void PauseGame()
    {
        _gameManager.Pause();
        SoundManager.instance.PauseBGM();
    }
    
    void ResumeGame()
    {
        _gameManager.Resume();
        SoundManager.instance.PlayBGM();
    }
    
    void ResetGyro()
    {
        _gameManager.ResetGyro();
    }

    void OnGameOver()
    {
        _gameManager.Pause();
        _uiController.OnGameOver();
        SoundManager.instance.StopBGM();
    }
}
