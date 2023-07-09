using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    private Slider _slider;
    private string _warnString = "누군가가 낌새를 채기까지";

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        if (_slider == null)
        {
            Debug.LogError("TimerUI: No slider component found!");
        }
    }

    public void OnTimeChanged(float curr, float max)
    {
        _slider.DOValue(curr / max, 0.1f);

        _timerText.text = $"{_warnString} {curr:F1}초";
    }

    public void OnWarningLeftChange(int i)
    {
        if (i >= 3)
        {
            _warnString = "누군가가 낌새를 채기까지";
        }
        else if (i >= 2)
        {
            _warnString = "과장님께서 눈치를 채기까지";
        }
        else if (i >= 1)
        {
            _warnString = "과장님께서 자리로 오시기까지";
        }
        else
        {
            _warnString = "걸렸다...";
        }
    }
}
