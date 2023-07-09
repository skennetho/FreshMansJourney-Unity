using UnityEngine;
using UnityEngine.Events;

public class GamePlayManager : MonoBehaviour
{
    public UnityEvent<bool> OnPaused = new();
    public DiabloManager DiabloManager;
    private bool _isPaused = false;

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);

        if (!ReferenceHolder.TryRegister(this))
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        GetESCKey();
    }

    private void GetESCKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
                DiabloManager.Timer.Resume();
            }
            else
            {
                PauseGame();
                DiabloManager.Timer.Pause();
            }
        }
    }

    public void PauseGame()
    {
        if (!_isPaused)
        {
            _isPaused = true;
            UIManager.Instance.PausedCanvas.gameObject.SetActive(true);
            OnPaused.Invoke(true);
        }
        else
        {
            Debug.LogWarning("Game is already paused");
        }
    }


    public void ResumeGame()
    {
        if (_isPaused)
        {
            _isPaused = false;
            OnPaused.Invoke(false);
            UIManager.Instance.PausedCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Game is already running");
        }
    }
}