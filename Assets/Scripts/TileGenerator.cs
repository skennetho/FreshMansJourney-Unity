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

    private List<Tile> _tiles;
    private bool _isTileMoving;
    private Vector2 _currentTilePos;
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

        _tiles = _tiles == null ? new List<Tile>() : _tiles;
        _isTileMoving = false;

        GenerateTiles(_viewportRowSize, _viewportColSize);
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

                // CurrentPos as Viewport center
                tile.TilePosition = new Vector2(i - (viewportRowSize - 1) / 2, j - (viewportColSize - 1) / 2);
                tile.transform.localPosition = TilePosToLocalPosition(tile.TilePosition);
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
        Debug.Log("TileGenerator SetCurrentPosition" + tilePos);
        _currentTilePos = tilePos;

        leftEdge = (int)_currentTilePos.x - halfRowSize;
        rightEdge = (int)_currentTilePos.x + halfRowSize;
        topEdge = (int)_currentTilePos.y + halfColSize;
        bottomEdge = (int)_currentTilePos.y - halfColSize;

        UpdateViewPort();
        OnTileMoving.Invoke(_isTileMoving);
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
                tile.Log();
            }
        }
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
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}