using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private Slider _slider;

    private int curr=0;
    private int max=0;
    private int level = 0;

    public void OnExpChange(int curr, int max)
    {
        _slider.DOValue((float)curr / max, 0.2f);
        this.curr = curr;
        this.max = max;
        UpdateExpText();
    }

    public void OnLevelChange(int level)
    {
        this.level = level;
        UpdateExpText();
    }

    private void UpdateExpText()
    {
        _expText.text = $"Level {level} Exp{curr}/{max}";
    }
}
