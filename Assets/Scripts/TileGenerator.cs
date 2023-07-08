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
    public UnityEvent OnTileMoveEnd = new();

    [Header("Generator Setting")]
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private float _horizontalGap;
    [SerializeField] private float _verticalGap;
    [SerializeField] private int _horizontalCount = 10;
    [SerializeField] private int _verticalCount = 10;

    [Header("ViewPort")]
    [SerializeField] private int _viewportRowSize = 7;
    [SerializeField] private int _viewportColSize = 5;
    float _moveAnimSeconds = 0.25f;


    [Header("Logical MapSize")]
    [SerializeField] private int _mapRowSize = 10;
    [SerializeField] private int _mapColSize = 10;


    private List<Tile> _tiles;
    private bool _isTileMoving;
    [SerializeField] private Vector2 _currentTilePos;

    private DiabloManager _diabloManager;

    public bool IsTileMoving => _isTileMoving;
    public Vector2 CurrentTilePos => _currentTilePos;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.localPosition, new Vector3(_horizontalGap * _viewportRowSize, _verticalGap * _viewportColSize, 0));
    }

    public void Initialize(DiabloManager diabloManager)
    {
        _diabloManager = diabloManager;

        _viewportRowSize = _viewportRowSize % 2 == 0 ? _viewportRowSize + 1 : _viewportRowSize;
        _viewportColSize = _viewportColSize % 2 == 0 ? _viewportColSize + 1 : _viewportColSize;

        _horizontalCount = _viewportRowSize;
        _verticalCount = _viewportColSize;

        _tiles = _tiles == null ? new List<Tile>() : _tiles;
        _isTileMoving = false;

        GenerateTiles(_horizontalCount, _verticalCount);
    }

    private void GenerateTiles(int horizontalCount, int verticalCount)
    {
        ClearTiles();

        for (int i = 0; i < horizontalCount; i++)
        {
            for (int j = 0; j < verticalCount; j++)
            {
                GameObject tileObject = Instantiate(_tilePrefab.gameObject, transform);
                Tile tile = tileObject.GetComponent<Tile>();
                
                // CurrentPos as Viewport center
                tile.TilePosition = new Vector2(i - (horizontalCount - 1) / 2, j - (verticalCount - 1) / 2);
                tile.transform.localPosition = TilePosToRealPosition(tile.TilePosition);
                _tiles.Add(tile);
            }
        }
    }

    private void ClearTiles()
    {
        foreach (var tile in _tiles)
        {
            Destroy(tile);
        }
        _tiles.Clear();
        _tiles = new();
        SetCurrentTilePos(Vector2.zero);
    }

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
        Debug.Log("MoveCo targetPos" + tilePos);
        tilePos.x = (int)tilePos.x;
        tilePos.y = (int)tilePos.y;
        SetCurrentTilePos(tilePos);

        _isTileMoving = true;
        float elapsedTime = 0.0f;

        // move animation
        Vector2 startPos = transform.localPosition;
        Vector2 worldPos = -TilePosToRealPosition(tilePos);

        while (elapsedTime < _moveAnimSeconds)
        {
            transform.localPosition = Vector2.Lerp(startPos, worldPos, elapsedTime / _moveAnimSeconds);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = worldPos;

        _isTileMoving = false;
        OnTileMoving.Invoke(_isTileMoving);
        OnTileMoveEnd.Invoke();
    }

    private void SetCurrentTilePos(Vector2 tilePos)
    {
        Debug.Log("TileGenerator SetCurrentPosition" + tilePos);
        _currentTilePos = tilePos;
        UpdateViewPort();
        OnTileMoving.Invoke(_isTileMoving);
    }

    int leftEdge;
    int rightEdge;
    int topEdge;
    int bottomEdge;

    private void UpdateViewPort()
    {
        leftEdge = (int)_currentTilePos.x - (_viewportRowSize - 1) / 2;
        rightEdge = (int)_currentTilePos.x + (_viewportRowSize - 1) / 2;
        topEdge = (int)_currentTilePos.y + (_viewportColSize - 1) / 2;
        bottomEdge = (int)_currentTilePos.y - (_viewportColSize - 1) / 2;

        foreach (var tile in _tiles)
        {
            // tile이 viewport 밖에 있으면 위치를 바꿔준다.
            if (tile.TilePosition.x < leftEdge ||
                tile.TilePosition.x > rightEdge ||
                tile.TilePosition.y < bottomEdge ||
                tile.TilePosition.y > topEdge)
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
                tile.transform.localPosition = TilePosToRealPosition(tilePos);
                tile.Log();
            }
        }
    }

    private Vector2 TilePosToRealPosition(Vector2 tilePos)
    {
        return new Vector2(tilePos.x * _horizontalGap, tilePos.y * _verticalGap);
    }

    private Vector2 RealPositionToTilePos(Vector2 realPos)
    {
        return new Vector2(-realPos.x / _horizontalGap, -realPos.y / _verticalGap);
    }
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}