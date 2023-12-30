using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    GameObject _gamePrefab;
    
    [SerializeField]
    UIController _uiController;

    private GameManager _gameManager;
    
    void Start()
    {
        _uiController.StartGameDelegate = StartGame;
        _uiController.PauseGameDelegate = PauseGame;
        _uiController.ResumeGameDelegate = ResumeGame;
        _uiController.ResetGyroDelegate = ResetGyro;
    }

    void StartGame()
    {
        if (_gameManager != null)
        {
            Destroy(_gameManager.gameObject);
        }
        SetGame();
        _gameManager.Resume();
    }

    void SetGame()
    {
        _gameManager = Instantiate(_gamePrefab).GetComponent<GameManager>();
        _gameManager.OnGameOverDelegate = OnGameOver;
    }
    
    void PauseGame()
    {
        _gameManager.Pause();
    }
    
    void ResumeGame()
    {
        _gameManager.Resume();
    }
    
    void ResetGyro()
    {
        _gameManager.ResetGyro();
    }

    void OnGameOver()
    {
        _gameManager.Pause();
        _uiController.OnGameOver();
    }
}
