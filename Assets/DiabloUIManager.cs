using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DiabloUIManager : MonoBehaviour
{
    [Header("Player")]
    public PlayerHealthUI PlayerHealthUI;
    public PlayerExpUI PlayerExpUI;
    public TimerUI TimerUI;

    [Header("For Game End")]
    [SerializeField] private Image _backGround;
    [SerializeField] private TextMeshProUGUI _gameEndText;
    [SerializeField] private TextMeshProUGUI _gameEndSubText;

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

        _diabloManager.Timer.OnTimeUpdate.AddListener(TimerUI.OnTimeChanged);
        TimerUI.OnTimeChanged(_diabloManager.Timer.CurrentSec, _diabloManager.Timer.MaxSec);
        _diabloManager.OnWarningLeftChange.AddListener(TimerUI.OnWarningLeftChange);
        TimerUI.OnWarningLeftChange(_diabloManager.WarningLeft);

        _diabloManager.OnGameEnd.AddListener(OnGameEnd);
    }

    public void OnGameEnd(GameEnd endType)
    {
        if (endType == GameEnd.GameClear)
        {
            _gameEndSubText.text = "내기에서 이겨 비싼 술을 얻어먹었다! 개꿀!";
            OnGameClear();
        }
        else
        {
            if (endType == GameEnd.TimeOver)
            {
                _gameEndSubText.text = "과장님 왈: 회사에서 게임을 해? 너는 이따보자. ^^";
            }
            else if (endType == GameEnd.PlayerDeath)
            {
                _gameEndSubText.text = "동기 왈: 게임도 못하고~ 내기도 지고~ 아 개꿀~ ㅋ";
            }
            OnGameOver();
        }
    }

    private void OnGameOver()
    {
        _gameEndText.text = "game over";
        _gameEndText.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);

        _gameEndSubText.color = _gameEndText.color;
        SwitchGameEndUIs();

        _gameEndText.transform.DOShakePosition(3.0f, 10.0f, 10, 90.0f, false, true);
    }

    private void OnGameClear()
    {
        _gameEndText.text = "game clear";
        _gameEndText.color = new Color(0.0f, 1.0f, 0.0f, 0.0f);

        _gameEndSubText.color = _gameEndText.color;
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

        _gameEndSubText.gameObject.SetActive(true);
        _gameEndSubText.DOFade(1.0f, 3.0f);
        StartCoroutine(TypeEffectCo());
        // use dotween to animate like typing at gameEndSubText
    }

    private IEnumerator TypeEffectCo()
    {
        float typeDelay = 0.1f;
        var wait = new WaitForSeconds(typeDelay);
        var text = _gameEndSubText.text;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            _gameEndSubText.text = sb.Append(text[i]).ToString();
            yield return wait;
        }
        _gameEndSubText.text = text;
    }

    public void SetGameEndSubText(string str)
    {
        _gameEndSubText.text = str;
    }

}
