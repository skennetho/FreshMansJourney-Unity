using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private const int SPAWN_COOLDOWN_MAX = 40;
    private const int SPAWN_COOLDOWN_MIN = 10;

    public Vector2 TilePosition;
    public Vector2 MapPosition;

    [SerializeField] private List<Monster> _monsters;
    private SpriteRenderer _tileSprite;
    private TileType _currentType = TileType.Normal;
    private TileGenerator _tileGenerator; // need to get closed tiles type and sprites

    private int _spawnCooldown = SPAWN_COOLDOWN_MIN;
    private Monster _currentMonster;

    public TileType TileType => _currentType;
    public Monster CurrentMonster => _currentMonster;

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle() { fontSize = 10 };
        style.normal.textColor = _currentType == TileType.Monster ? Color.red : Color.green;
        Handles.Label(transform.position, $"[{(int)TilePosition.x},{(int)TilePosition.y}]", style);
        Handles.Label(transform.position + Vector3.down * 0.3f, $"[{(int)MapPosition.x},{(int)MapPosition.y}]", new GUIStyle() { fontSize = 10 });
        Handles.Label(transform.position + Vector3.down * 0.6f, $"Cooldown:{_spawnCooldown}", new GUIStyle() { fontSize = 10 });
    }

    private void Awake()
    {
        _tileSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetTileGenerator(TileGenerator tileGenerator)
    {
        _tileGenerator = tileGenerator;
        _spawnCooldown = Random.Range(SPAWN_COOLDOWN_MIN, SPAWN_COOLDOWN_MAX);
    }

    public void SetTileType(TileType type)
    {
        _currentType = type;
        switch (_currentType)
        {
            case TileType.Normal:
                _tileSprite.sprite = GetNormalSprite();
                HideMonsters();
                break;
            case TileType.Monster:
                Debug.Log("Monster!", gameObject);
                _tileSprite.sprite = GetNormalSprite();
                if (_currentMonster == null)
                {
                    SpawnRandomMonster();
                }
                break;
            case TileType.Blocked:
                _tileSprite.sprite = GetBlockedSprite();
                HideMonsters();
                break;
        }
    }

    public void OnViewPort()
    {
        switch (_currentType)
        {
            case TileType.Normal:
                if (_tileGenerator.CurrentTilePos == TilePosition)
                {
                    return;
                }

                if (_spawnCooldown > 0)
                {
                    _spawnCooldown--;
                }
                else
                {
                    SpawnRandomMonster();
                    _spawnCooldown = Random.Range(SPAWN_COOLDOWN_MIN, SPAWN_COOLDOWN_MAX);
                }
                break;
        }
    }

    private void SpawnRandomMonster()
    {
        HideMonsters();
        _currentType = TileType.Monster;

        _currentMonster = _monsters[Random.Range(0, _monsters.Count)];
        _currentMonster.gameObject.SetActive(true);
        Debug.Log(_currentMonster.gameObject.name, _currentMonster.gameObject);
        _currentMonster.OnDeath = () =>
        {
            Debug.Log("Monster died!", gameObject);
            SetTileType(TileType.Normal);
        };
    }

    private void HideMonsters()
    {
        foreach (var monster in _monsters)
        {
            monster.gameObject.SetActive(false);
        }
        _currentMonster = null;
    }

    private Sprite GetNormalSprite()
    {
        int s = Random.Range(0, 3);
        if (s == 1)
        {
            return _tileGenerator.Normal1;
        }
        else if (s == 2)
        {
            return _tileGenerator.Normal2;
        }
        else
        {
            return _tileGenerator.Normal3;
        }
    }

    private Sprite GetBlockedSprite()
    {
        // check close tiles from mapData of _tileGenerator and return the correct sprite among fill, leftedge, rightedge, topedge, bottomedge, topleftcorner, toprightcorner, bottomleftcorner, bottomrightcorner
        bool left = _tileGenerator.GetTileType(MapPosition + Vector2.left) == TileType.Blocked;
        bool right = _tileGenerator.GetTileType(MapPosition + Vector2.right) == TileType.Blocked;
        bool top = _tileGenerator.GetTileType(MapPosition + Vector2.up) == TileType.Blocked;
        bool bottom = _tileGenerator.GetTileType(MapPosition + Vector2.down) == TileType.Blocked;


        if (left && right && top && bottom)
        {
            return _tileGenerator.BlockedFill;
        }
        if (left && right && top && !bottom)
        {
            return _tileGenerator.BlockedBottomEdge;
        }
        if (left && right && !top && bottom)
        {
            return _tileGenerator.BlockedTopEdge;
        }
        if (left && !right && top && bottom)
        {
            return _tileGenerator.BlockedRightEdge;
        }
        if (!left && right && top && bottom)
        {
            return _tileGenerator.BlockedLeftEdge;
        }
        if (!left && right && top && !bottom)
        {
            return _tileGenerator.BlockedBottomLeftCorner;
        }
        if (!left && right && !top && bottom)
        {
            return _tileGenerator.BlockedTopLeftCorner;
        }
        if (left && !right && top && !bottom)
        {
            return _tileGenerator.BlockedBottomRightCorner;
        }
        if(left && !right && !top && bottom)
        {
            return _tileGenerator.BlockedTopRightCorner;
        }
        return _tileGenerator.BlockedFill;
    }

    public void Log()
    {
        Debug.Log($"TilePosition: {TilePosition} , localPos:{transform.localPosition}, realPos:{transform.position}", gameObject);
    }
}