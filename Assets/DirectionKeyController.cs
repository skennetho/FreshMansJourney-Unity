using System.Collections.Generic;
using UnityEngine;

public class DirectionKeyController : MonoBehaviour
{
    public const char NONE_KEY_CHAR = ' ';

    [SerializeField] private DirectionKey _leftKey;
    [SerializeField] private DirectionKey _rightKey;
    [SerializeField] private DirectionKey _upKey;
    [SerializeField] private DirectionKey _downKey;
    private Dictionary<Direction, DirectionKey> _directionKeys;


    public void Initialize(DiabloManager diabloManager)
    {
        _directionKeys = new Dictionary<Direction, DirectionKey>();
        _directionKeys.Add(Direction.Left, _leftKey);
        _directionKeys.Add(Direction.Right, _rightKey);
        _directionKeys.Add(Direction.Up, _upKey);
        _directionKeys.Add(Direction.Down, _downKey);

        SetDirectionKeys('a', 'd', 'w', 's');
    }

    public void SetDirectionKeys(char leftKey, char rightKey, char upKey, char downKey)
    {
        if (!CheckIfValidKeys(leftKey, rightKey, upKey, downKey))
        {
            return;
        }

        _leftKey.SetKey(leftKey);
        _rightKey.SetKey(rightKey);
        _upKey.SetKey(upKey);
        _downKey.SetKey(downKey);
    }

    public void SetDirectionKeysRandomly()
    {
        bool[] alphabets = new bool[26];
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, alphabets.Length);
            while (alphabets[randomIndex])
            {
                randomIndex = Random.Range(0, alphabets.Length);
            }
            alphabets[randomIndex] = true;
            _directionKeys[(Direction)i].SetKey((char)('a' + randomIndex));
        }
    }

    private bool CheckIfValidKeys(char leftKey, char rightKey, char upKey, char downKey)
    {
        if (leftKey == rightKey || leftKey == upKey || leftKey == downKey ||
            rightKey == upKey || rightKey == downKey ||
            upKey == downKey)
        {
            Debug.LogError("Invalid Keys");
            return false;
        }
        return true;
    }

    public Direction InputKeyboard(char inputKey)
    {
        if (inputKey == _leftKey.GetKey())
        {
            return Direction.Left;
        }
        else if (inputKey == _rightKey.GetKey())
        {
            return Direction.Right;
        }
        else if (inputKey == _upKey.GetKey())
        {
            return Direction.Up;
        }
        else if (inputKey == _downKey.GetKey())
        {
            return Direction.Down;
        }
        else
        {
            return Direction.None;
        }
    }
}
