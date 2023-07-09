using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileGenerator : MonoBehaviour
{
    // constants
    private readonly int[,] _directionMultiplier = new int[4, 2] { { -1, 0 }, { 1, 0 }, { 0, 1 }, { 0, -1 } };

    // events
    public UnityEvent<bool> OnTileMoving = new();
    public UnityEvent OnViewportUpdate = new();
    public UnityEvent OnTileMoveEnd = new();

    [Header("Generator Setting")]
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private float _horizontalGap;
    [SerializeField] private float _verticalGap;


    [Header("ViewPort")]
    [SerializeField] private int _viewportRowSize = 7;
    [SerializeField] private int _viewportColSize = 5;
    private float _moveAnimSeconds = 0.25f;
    private int halfRowSize;
    private int halfColSize;

    public int leftEdge { private set; get; }
    public int rightEdge { private set; get; }
    public int topEdge { private set; get; }
    public int bottomEdge { private set; get; }


    [Header("Logical MapSize")]
    [SerializeField] private int _mapRowSize = 10;
    [SerializeField] private int _mapColSize = 10;
    [SerializeField] private int _minBlockedRadius = 2;
    [SerializeField] private int _maxBlockedRadius = 4;
    [SerializeField] private int _minBlockedCount = 5;
    [SerializeField] private int _maxBlockedCount = 10;
    private TileType[,] _mapData;

    [Header("Sprites")]
    public Sprite Normal1;
    public Sprite Normal2;
    public Sprite Normal3;
    public Sprite BlockedFill;
    public Sprite BlockedLeftEdge;
    public Sprite BlockedRightEdge;
    public Sprite BlockedTopEdge;
    public Sprite BlockedBottomEdge;
    public Sprite BlockedTopLeftCorner;
    public Sprite BlockedBottomLeftCorner;
    public Sprite BlockedTopRightCorner;
    public Sprite BlockedBottomRightCorner;


    private List<Tile> _tiles;
    private Dictionary<Direction, Tile> _nearByTiles;
    private bool _isTileMoving;
    private Vector2 _currentTilePos;
    private DiabloManager _diabloManager;
    private int diffX;

    public bool IsTileMoving => _isTileMoving;
    public Vector2 CurrentTilePos => _currentTilePos;
    public Dictionary<Direction, Tile> NearByTiles => _nearByTiles;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.localPosition, new Vector3(_horizontalGap * _viewportRowSize, _verticalGap * _viewportColSize, 0));


        // draw blocked area
        if (_mapData == null) { return; }
        for (int i = 0; i < _mapRowSize; i++)
        {
            for (int j = 0; j < _mapColSize; j++)
            {
                if (_mapData[i, j] == TileType.Blocked)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(transform.position - transform.localPosition + new Vector3(_horizontalGap * i, _verticalGap * j, 0), new Vector3(_horizontalGap, _verticalGap, 0));
                }
            }
        }
    }

    public void Initialize(DiabloManager diabloManager)
    {
        _diabloManager = diabloManager;

        _viewportRowSize = _viewportRowSize % 2 == 0 ? _viewportRowSize + 1 : _viewportRowSize;
        _viewportColSize = _viewportColSize % 2 == 0 ? _viewportColSize + 1 : _viewportColSize;

        _tiles = _tiles == null ? new List<Tile>() : _tiles;
        _isTileMoving = false;

        GenenrateMapData();
        GenerateTiles(_viewportRowSize, _viewportColSize);

        _nearByTiles = new();
        _nearByTiles.Add(Direction.Left, null);
        _nearByTiles.Add(Direction.Right, null);
        _nearByTiles.Add(Direction.Up, null);
        _nearByTiles.Add(Direction.Down, null);
        UpdateNearbyTiles();
    }

    private void GenenrateMapData()
    {
        _mapData = new TileType[_mapRowSize, _mapColSize];

        // set all tile as normal
        for (int i = 0; i < _mapRowSize; i++)
        {
            for (int j = 0; j < _mapColSize; j++)
            {
                _mapData[i, j] = TileType.Normal;
            }
        }

        // set blocked area
        for (int cnt = 0; cnt < UnityEngine.Random.Range(_minBlockedCount, _maxBlockedCount); cnt++)
        {
            int radius = UnityEngine.Random.Range(_minBlockedRadius, _maxBlockedRadius);
            int row = UnityEngine.Random.Range(radius, _mapRowSize - radius);
            int col = UnityEngine.Random.Range(radius, _mapColSize - radius);
            for (int i = row - radius; i <= row + radius; i++)
            {
                for (int j = col - radius; j <= col + radius; j++)
                {
                    _mapData[i, j] = TileType.Blocked;
                }
            }
        }

        // set monster area
        for (int i = 0; i < _mapRowSize; i++)
        {
            for (int j = 0; j < _mapColSize; j++)
            {
                if (_mapData[i, j] != TileType.Normal)
                {
                    continue;
                }

                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    _mapData[i, j] = TileType.Monster;
                }
            }
        }
    }

    private void GenerateTiles(int viewportRowSize, int viewportColSize)
    {
        ClearTiles();
        halfRowSize = (viewportRowSize - 1) / 2;
        halfColSize = (viewportColSize - 1) / 2;
        for (int i = 0; i < viewportRowSize; i++)
        {
            for (int j = 0; j < viewportColSize; j++)
            {
                GameObject tileObject = Instantiate(_tilePrefab.gameObject, transform);
                Tile tile = tileObject.GetComponent<Tile>();
                tile.SetTileGenerator(this);
                _tiles.Add(tile);

                // set tilePos and localPos
                tile.TilePosition = new Vector2(i - (viewportRowSize - 1) / 2, j - (viewportColSize - 1) / 2);
                tile.transform.localPosition = TilePosToLocalPosition(tile.TilePosition);

                // set mapPos and tileType
                tile.MapPosition = TilePosToMapPos(tile.TilePosition);
                tile.SetTileType(GetTileType(tile.MapPosition));
            }
        }
    }

    public TileType GetTileType(Vector2 tilePos)
    {
        tilePos = TilePosToMapPos(tilePos);
        return _mapData[(int)tilePos.x, (int)tilePos.y];
    }

    public TileType GetTileType(Direction dir, Vector2 tilePos)
    {
        switch (dir)
        {
            case Direction.Up:
                return GetTileType(tilePos + Vector2.up);
            case Direction.Down:
                return GetTileType(tilePos + Vector2.down);
            case Direction.Left:
                return GetTileType(tilePos + Vector2.left);
            case Direction.Right:
                return GetTileType(tilePos + Vector2.right);
            default:
                return TileType.Normal;
        }
    }

    public Tile GetTile(Direction dir, Vector2 tilePos)
    {
        tilePos.x += _directionMultiplier[(int)dir, 0];
        tilePos.y += _directionMultiplier[(int)dir, 1];

        tilePos = TilePosToMapPos(tilePos);
        foreach (var tile in _tiles)
        {
            if (tile.MapPosition == tilePos)
            {
                return tile;
            }
        }
        Debug.LogError("GetTile not found");
        return null;
    }


    private void ClearTiles()
    {
        foreach (var tile in _tiles)
        {
            Destroy(tile);
        }
        _tiles.Clear();
        _currentTilePos = Vector2.zero;
    }

    #region move related
    public void MoveToDirection(Direction direction)
    {
        MoveToDirection(_directionMultiplier[(int)direction, 0], _directionMultiplier[(int)direction, 1]);
    }

    public void MoveToDirection(int xOffset, int yOffset)
    {
        if (_isTileMoving) return;

        Vector2 tilePos = _currentTilePos;
        tilePos.x += xOffset;
        tilePos.y += yOffset;
        StartCoroutine(MoveCo(tilePos));
    }
    private IEnumerator MoveCo(Vector2 tilePos)
    {
        SetCurrentTilePos(tilePos);

        _isTileMoving = true;
        OnTileMoving.Invoke(_isTileMoving);

        // move animation
        float elapsedTime = 0.0f;
        Vector2 startPos = transform.localPosition;
        Vector2 localPos = -TilePosToLocalPosition(tilePos);

        while (elapsedTime < _moveAnimSeconds)
        {
            transform.localPosition = Vector2.Lerp(startPos, localPos, elapsedTime / _moveAnimSeconds);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = localPos;

        _isTileMoving = false;
        OnTileMoving.Invoke(_isTileMoving);
        OnTileMoveEnd.Invoke();
    }

    private void SetCurrentTilePos(Vector2 tilePos)
    {
        tilePos.x = (int)tilePos.x;
        tilePos.y = (int)tilePos.y;
        _currentTilePos = tilePos;
        Debug.Log("current tile pos : " + _currentTilePos);

        leftEdge = (int)_currentTilePos.x - halfRowSize;
        rightEdge = (int)_currentTilePos.x + halfRowSize;
        topEdge = (int)_currentTilePos.y + halfColSize;
        bottomEdge = (int)_currentTilePos.y - halfColSize;

        UpdateViewPort();
    }

    private void UpdateViewPort()
    {
        foreach (var tile in _tiles)
        {
            // tile이 viewport 밖에 있으면 위치를 바꿔준다.
            if (IsTileInViewport(tile.TilePosition) == false)
            {
                Vector2 tilePos = tile.TilePosition;
                if (tilePos.x < leftEdge)
                {
                    tilePos.x += _viewportRowSize;
                }
                if (tilePos.x > rightEdge)
                {
                    tilePos.x -= _viewportRowSize;
                }
                if (tilePos.y < bottomEdge)
                {
                    tilePos.y += _viewportColSize;
                }
                if (tilePos.y > topEdge)
                {
                    tilePos.y -= _viewportColSize;
                }
                tile.TilePosition = tilePos;
                tile.transform.localPosition = TilePosToLocalPosition(tilePos);
                tile.MapPosition = TilePosToMapPos(tilePos);
                tile.SetTileType(GetTileType(tile.MapPosition));
            }
            else
            {
                tile.OnViewPort();
            }
        }
        UpdateNearbyTiles();
    }

    private void UpdateNearbyTiles()
    {
        for (int i = 0; i < 4; i++)
        {
            var direction = (Direction)i;
            _nearByTiles[direction] = GetTile(direction, CurrentTilePos);
        }
    }
    #endregion

    #region utils
    public Vector2 TilePosToMapPos(Vector2 tilePos)
    {
        // for example if x was -1 and mapRowSize was 10, then it should be 9 and also if x was -11 and mapRowSize was 10, then it should be 9
        int x = (int)tilePos.x % _mapRowSize;
        x = x < 0 ? x + _mapRowSize : x;
        int y = (int)tilePos.y % _mapColSize;
        y = y < 0 ? y + _mapColSize : y;
        return new Vector2(x, y);
    }

    public bool IsTileInViewport(Vector2 tilePos)
    {
        return tilePos.x >= leftEdge && tilePos.x <= rightEdge && tilePos.y >= bottomEdge && tilePos.y <= topEdge;
    }

    public Vector2 TilePosToLocalPosition(Vector2 tilePos)
    {
        return new Vector2(tilePos.x * _horizontalGap, tilePos.y * _verticalGap);
    }

    public Vector2 LocalPositionToTilePos(Vector2 realPos)
    {
        return new Vector2(-realPos.x / _horizontalGap, -realPos.y / _verticalGap);
    }
    #endregion
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
    None
}

public enum TileType
{
    Normal,
    Monster,
    Blocked,
}