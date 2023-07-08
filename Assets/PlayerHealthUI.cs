using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        if(_slider == null)
        {
            Debug.LogError("PlayerHealthUI: No slider component found!");
        }
    }

    public void SetHealth(int curr, int max)
    {
        _slider.DOValue((float)curr / max, 0.2f);
        _healthText.text = $"HP: {curr}/{max}";
    }

}
