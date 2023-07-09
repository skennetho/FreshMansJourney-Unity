using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Slider _slider;

    public void OnHeathChange(int curr, int max)
    {
        _slider.DOValue((float)curr / max, 0.2f);
        _healthText.text = $"HP: {curr}/{max}";
    }

}
