using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    GameObject _gamePrefab;
    
    [SerializeField]
    GameObject _tutorialPrefab;
    
    [SerializeField]
    UIController _uiController;
    
    [SerializeField]
    GameObject _floatingBlocksPrefab;

    private GameManager _gameManager;
    private TutorialManager _tutorialManager;
    private GameObject _floatingBlocks;

    void Start()
    {
        _uiController.StartGameDelegate = StartGame;
        _uiController.StartTutorialDelegate = StartTutorial;
        _uiController.PauseGameDelegate = PauseGame;
        _uiController.ResumeGameDelegate = ResumeGame;
        _uiController.QuitGameDelegate = QuitGame;
        _uiController.ResetGyroDelegate = ResetGyro;
        
        _floatingBlocks = Instantiate(_floatingBlocksPrefab);
    }
    
    void StartTutorial()
    {
        Destroy(_floatingBlocks);
        
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
        _uiController.OnStartGame();
    }

    void StartGame()
    {
        Destroy(_floatingBlocks);

        if (_gameManager != null)
        {
            Destroy(_gameManager.gameObject);
        }
        SetGame();
        _gameManager.Resume();
        SoundManager.instance.StopBGM();
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
    
    void QuitGame()
    {
        if (_gameManager != null)
        {
            Destroy(_gameManager.gameObject);
        }

        _floatingBlocks = Instantiate(_floatingBlocksPrefab);
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
