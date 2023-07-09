using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private Slider _slider;

    private int curr=0;
    private int max=1;
    private int level = 0;

    public void OnExpChange(int curr, int max)
    {
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
        _slider.value = (float)curr / max;
        Debug.Log($"exp {curr} / {max} : " + ((float)curr / max));
        _expText.text = $"Level{level} Exp:{curr}/{max}";
    }
}
