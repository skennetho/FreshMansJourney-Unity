using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DirectionKey : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _keyText;
    [SerializeField] private Image _arrowImage;
    private char _keyCharacter = DirectionKeyController.NONE_KEY_CHAR;

    public void SetKey(char key)
    {
        _keyCharacter = key;
        _keyText.text = key.ToString();
    }

    public void OnWrongKey()
    {
        _keyText.transform.DOShakePosition(0.5f, 10.0f, 10, 90.0f, false, true);
    }

    public void OnRightKey()
    {
        _arrowImage.DOColor(Color.green, 0.5f).OnComplete(() => _arrowImage.DOColor(Color.white, 0.5f));
    }

    public char GetKey()
    {
        return _keyCharacter;
    }
}
