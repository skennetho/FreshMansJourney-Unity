using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 TilePosition;
    public Vector2 MapPosition;

    [SerializeField] private List<GameObject> _monsters;
    private SpriteRenderer _tileSprite;
    private TileType _currentType = TileType.Normal;
    private TileGenerator _tileGenerator; // need to get closed tiles type and sprites

    public TileType TileType => _currentType;

    private void OnDrawGizmos()
    {
        Handles.color = Color.black;
        Handles.Label(transform.position, $"[{(int)TilePosition.x},{(int)TilePosition.y}]", new GUIStyle() { fontSize = 10 });

        Handles.color = Color.green;
        Handles.Label(transform.position + Vector3.down * 0.5f, $"[{(int)MapPosition.x},{(int)MapPosition.y}]", new GUIStyle() { fontSize = 10 });
    }

    private void Awake()
    {
        _tileSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetTileGenerator(TileGenerator tileGenerator)
    {
        _tileGenerator = tileGenerator;
    }

    public void SetTileType(TileType type)
    {
        _currentType = type;
        switch (_currentType)
        {
            case TileType.Normal:
                _tileSprite.sprite = GetNormalSprite();
                break;
            case TileType.Monster:
                SpawnRandomMonster();
                _tileSprite.sprite = GetNormalSprite();
                break;
            case TileType.Blocked:
                _tileSprite.sprite = GetBlockedSprite();
                break;
        }
    }

    public void SpawnRandomMonster()
    {
        _currentType = TileType.Monster;

        var monsterObj = _monsters[Random.Range(0, _monsters.Count)];
        foreach (var monster in _monsters)
        {
            monster.SetActive(false);
        }
        monsterObj.SetActive(true);
    }

    private Sprite GetNormalSprite()
    {
        int s = Random.Range(0, 3);
        if(s == 1)
        {
            return _tileGenerator.Normal1;
        }
        else if(s == 2)
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
        else if (left && right && top)
        {
            return _tileGenerator.BlockedBottomEdge;
        }
        else if (left && right && bottom)
        {
            return _tileGenerator.BlockedTopEdge;
        }
        else if (left && top && bottom)
        {
            return _tileGenerator.BlockedRightEdge;
        }
        else if (right && top && bottom)
        {
            return _tileGenerator.BlockedLeftEdge;
        }
        else if (left && right)
        {
            return _tileGenerator.BlockedTopLeftCorner;
        }
        else if (left && top)
        {
            return _tileGenerator.BlockedBottomRightCorner;
        }
        else if (left && bottom)
        {
            return _tileGenerator.BlockedTopRightCorner;
        }
        else if (right && top)
        {
            return _tileGenerator.BlockedBottomLeftCorner;
        }
        else if (right && bottom)
        {
            return _tileGenerator.BlockedTopLeftCorner;
        }
        else if (top && bottom)
        {
            return _tileGenerator.BlockedRightEdge;
        }
        else if (left)
        {
            return _tileGenerator.BlockedRightEdge;
        }
        else if (right)
        {
            return _tileGenerator.BlockedLeftEdge;
        }
        else if (top)
        {
            return _tileGenerator.BlockedBottomEdge;
        }
        else if (bottom)
        {
            return _tileGenerator.BlockedTopEdge;
        }
        else
        {
            return _tileGenerator.BlockedFill;
        }   
    }

    public void Log()
    {
        Debug.Log($"TilePosition: {TilePosition} , localPos:{transform.localPosition}, realPos:{transform.position}", gameObject);
    }
}