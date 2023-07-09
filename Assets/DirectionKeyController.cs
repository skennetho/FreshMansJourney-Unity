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
    private bool[] _usedKeys;


    public void Initialize(DiabloManager diabloManager)
    {
        _directionKeys = new Dictionary<Direction, DirectionKey>();
        _directionKeys.Add(Direction.Left, _leftKey);
        _directionKeys.Add(Direction.Right, _rightKey);
        _directionKeys.Add(Direction.Up, _upKey);
        _directionKeys.Add(Direction.Down, _downKey);

        _usedKeys = new bool[26];
        SetDirectionKeys('a', 'd', 'w', 's');
    }

    public void SetDirectionKeys(char leftKey, char rightKey, char upKey, char downKey)
    {
        if (!CheckIfValidKeys(leftKey, rightKey, upKey, downKey))
        {
            return;
        }
        SetKey(Direction.Left, leftKey);
        SetKey(Direction.Right, rightKey);
        SetKey(Direction.Up, upKey);
        SetKey(Direction.Down, downKey);
    }

    public void SetDirectionKeyRandomly(Direction direction)
    {
        int randomIndex = Random.Range(0, 26);
        while (_usedKeys[randomIndex])
        {
            randomIndex = Random.Range(0, 26);
        }

        SetKey(direction, (char)('a' + randomIndex));
    }

    public void SetDirectionKeyNone(Direction direction)
    {
        SetKey(direction, NONE_KEY_CHAR);
    }

    private bool CheckIfValidKeys(char leftKey, char rightKey, char upKey, char downKey)
    {
        // check if all keys are different
        if (leftKey == rightKey || leftKey == upKey || leftKey == downKey || 
            rightKey == upKey || rightKey == downKey || 
            upKey == downKey)
        {
            Debug.LogError("All keys must be different");
            return false;
        }
        return true;
    }

    private void SetKey(Direction direction, char key)
    {
        if (_directionKeys[direction].GetKey() != NONE_KEY_CHAR)
        {
            _usedKeys[_directionKeys[direction].GetKey() - 'a'] = false;
        }

        _directionKeys[direction].SetKey(key);

        if (key != NONE_KEY_CHAR)
        {
            _usedKeys[_directionKeys[direction].GetKey() - 'a'] = true;
        }
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

    public char GetKey(Direction direction)
    {
        return _directionKeys[direction].GetKey();
    }
}
