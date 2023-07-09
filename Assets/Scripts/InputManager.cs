using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public UnityEvent<char> OnAlphabetInput = new();
    private GamePlayManager _gamePlayManager;
    private bool _initialized = false;

    private void Awake()
    {
        if (!ReferenceHolder.TryRegister(this))
        {
            Destroy(gameObject);
        }
        ReferenceHolder.Request<GamePlayManager>(Initialize);
    }

    private void Initialize(GamePlayManager gamePlayManager)
    {
        _gamePlayManager = gamePlayManager;
        _initialized = true;
    }

    private void Update()
    {
        if(!_initialized || _gamePlayManager.IsPaused)
        {
            return;
        }

        if (Input.anyKeyDown)
        {
            if (Input.inputString.Length > 0 && char.IsLetter(Input.inputString[0]))
            {
                char inputKey = Input.inputString[0];
                if (inputKey >= 'A' && inputKey <= 'Z')
                {
                    inputKey = (char)(inputKey + 32);
                }

                OnAlphabetInput.Invoke(inputKey);
            }
        }
    }
}
