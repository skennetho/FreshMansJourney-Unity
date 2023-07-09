using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DiabloUIManager : MonoBehaviour
{
    [Header("Player")]
    public PlayerHealthUI PlayerHealthUI;
    public PlayerExpUI PlayerExpUI;

    [Header("For Game End")]
    [SerializeField] private Image _backGround;
    [SerializeField] private TextMeshProUGUI _gameEndText;

    // caching
    private DiabloManager _diabloManager;


    private void Awake()
    {
        ReferenceHolder.TryRegister(this);
        ReferenceHolder.Request<DiabloManager>(OnDiabeloManagerGet);
    }

    private void OnDiabeloManagerGet(DiabloManager diabloManager)
    {
        _diabloManager = diabloManager;

        _diabloManager.Player.OnHealthUpdate.AddListener(PlayerHealthUI.OnHeathChange);
        PlayerHealthUI.OnHeathChange(_diabloManager.Player.Health, DiabloPlayer.MAX_HEALTH);

        _diabloManager.Player.OnLevelUpdate.AddListener(PlayerExpUI.OnLevelChange);
        PlayerExpUI.OnLevelChange(_diabloManager.Player.Level);
        _diabloManager.Player.OnExpChange.AddListener(PlayerExpUI.OnExpChange);
        PlayerExpUI.OnExpChange(_diabloManager.Player.Exp, _diabloManager.Player.MaxExp);

        _diabloManager.OnGameEnd.AddListener(OnGameEnd);
    }

    public void OnGameEnd(bool isGameClear)
    {
        if (isGameClear)
        {
            OnGameClear();
        }
        else
        {
            OnGameOver();
        }
    }

    private void OnGameOver()
    {
        _gameEndText.text = "game over";
        _gameEndText.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
        SwitchGameEndUIs();

        _gameEndText.transform.DOShakePosition(3.0f, 10.0f, 10, 90.0f, false, true);
    }

    private void OnGameClear()
    {
        _gameEndText.text = "game clear";
        _gameEndText.color = new Color(0.0f, 1.0f, 0.0f, 0.0f);
        SwitchGameEndUIs();

        _gameEndText.transform.DOLocalJump(Vector3.one, 100.0f, 2, 2.0f);
    }

    private void SwitchGameEndUIs()
    {
        PlayerHealthUI.gameObject.SetActive(false);

        _backGround.gameObject.SetActive(true);
        _backGround.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        _backGround.DOFade(1.0f, 3.0f);

        _gameEndText.gameObject.SetActive(true);
        _gameEndText.DOFade(1.0f, 3.0f);
    }
}
