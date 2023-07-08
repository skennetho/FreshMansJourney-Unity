using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileGenerator : MonoBehaviour
{
    public UnityEvent<bool> OnTileMoving = new();

    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private float _horizontalGap;
    [SerializeField] private float _verticalGap;
    [SerializeField] private int _horizontalCount = 10;
    [SerializeField] private int _verticalCount = 10;

    private readonly int[,] _directionMultiplier = new int[4, 2] { { -1, 0 }, { 1, 0 }, { 0, 1 }, { 0, -1 } };

    private List<Tile> _tiles;
    private bool _isTileMoving;
    private Vector2 _currentLocation = Vector2.zero;

    private DiabloManager _diabloManager;

    public bool IsTileMoving => _isTileMoving;
    public Vector2 CurrentLocation => _currentLocation;

    public void Initialize(DiabloManager diabloManager)
    {
        _diabloManager = diabloManager;
        _diabloManager.OnPlayerMove.AddListener(() => ActivateTileNeerby(_diabloManager.PlayerPosition, 1));

        _horizontalGap = _horizontalGap % 2 == 0 ? _horizontalGap + 1 : _horizontalGap;
        _verticalGap = _verticalGap % 2 == 0 ? _verticalGap + 1 : _verticalGap;
        _tiles = _tiles == null? new List<Tile>() : _tiles;
        _isTileMoving = false;

        GenerateTiles(_horizontalCount, _verticalCount);
    }

    private void ClearTiles()
    {
        foreach (var tile in _tiles)
        {
            Destroy(tile);
        }
        _tiles.Clear();
        _tiles = new();
        _currentLocation = Vector2.zero;
    }

    private void GenerateTiles(int horizontalCount, int verticalCount)
    {
        ClearTiles();
        for (int i = 0; i < horizontalCount; i++)
        {
            for (int j = 0; j < verticalCount; j++)
            {
                GameObject tileObject = Instantiate(_tilePrefab.gameObject, transform);
                tileObject.transform.localPosition = new Vector3(_horizontalGap * i, _verticalGap * j, 0);

                Tile tile = tileObject.GetComponent<Tile>();
                tile.TilePosition = new Vector2(i, j);
                _tiles.Add(tile);
            }
        }
    }


    public void MoveToDirection(Direction direction)
    {
        MoveToDirection(_directionMultiplier[(int)direction, 0], _directionMultiplier[(int)direction, 1]);
    }

    public void MoveToDirection(int xOffset, int yOffset)
    {
        if (_isTileMoving) return;

        Vector2 targetPos = _diabloManager.PlayerPosition;
        targetPos.x += xOffset;
        targetPos.y += yOffset;
        StartCoroutine(MoveCo(targetPos));
    }

    public void MoveToPosition(Vector2 nextPos)
    {
        if (_isTileMoving) return;

        StartCoroutine(MoveCo(nextPos));
    }


    private IEnumerator MoveCo(Vector2 targetPos)
    {
        Debug.Log("MoveCo targetPos" + targetPos);
        _isTileMoving = true;
        _currentLocation = targetPos;
        OnTileMoving.Invoke(_isTileMoving);

        float moveAnimSeconds = 0.5f;
        float elapsedTime = 0.0f;
        targetPos.x = (int)targetPos.x *_horizontalGap;
        targetPos.y = (int)targetPos.y *_verticalGap;

        Vector2 startPos = transform.position;
        while (elapsedTime < moveAnimSeconds)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / moveAnimSeconds);
            elapsedTime += Time.deltaTime;
            Debug.Log("elapsedTime: " + elapsedTime);
            yield return new WaitForEndOfFrame();
        }
        transform.position = targetPos;
        
        _isTileMoving = false;
        OnTileMoving.Invoke(_isTileMoving);

    }

    private void ActivateTileNeerby(Vector2 basePos, int range)
    {

    }

}

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}