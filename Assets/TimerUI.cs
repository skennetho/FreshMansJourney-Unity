using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    private Slider _slider;
    private string _warnString = "�������� ������ ä�����";

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

        _timerText.text = $"{_warnString} {curr:F1}��";
    }

    public void OnWarningLeftChange(int i)
    {
        if (i >= 3)
        {
            _warnString = "�������� ������ ä�����";
        }
        else if (i >= 2)
        {
            _warnString = "����Բ��� ��ġ�� ä�����";
        }
        else if (i >= 1)
        {
            _warnString = "����Բ��� �ڸ��� ���ñ����";
        }
        else
        {
            _warnString = "�ɷȴ�...";
        }
    }
}
