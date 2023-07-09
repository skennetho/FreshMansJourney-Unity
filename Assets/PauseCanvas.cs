using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour
{
    [SerializeField] private Button _backToMainBtn;

    private GamePlayManager _gamePlayManager;

    private void Start()
    {
        ReferenceHolder.Request<GamePlayManager>(Initialize);
    }

    private void Initialize(GamePlayManager gamePlayManager)
    {
        _gamePlayManager = gamePlayManager;
        _backToMainBtn.onClick.AddListener(BackToMainMenu);
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ClosePauseCanvas()
    {
        gameObject.SetActive(false);
    }

    private void Resume()
    {
        ClosePauseCanvas();
        _gamePlayManager.ResumeGame();
    }
}
