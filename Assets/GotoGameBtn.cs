using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GotoGameBtn : MonoBehaviour
{
    private Button btn;
    public Button QuitBtn;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);

        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));

        QuitBtn.onClick.AddListener(() => Application.Quit());
    }
}
