using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiabloManager : MonoBehaviour
{
    [Header("Player")]
    [HideInInspector] public UnityEvent OnPlayerMove;
    public DiabloPlayer Player;
    public Vector2 PlayerPosition;

    [Header("Monster")]
    public List<Monster> _monsterPrefabs;

    [Header("Tiles")]
    public TileGenerator TileGenerator;

    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    private void Awake()
    {
        Initiatialize();
        ReferenceHolder.TryRegister(this);
    }

    private void Update()
    {
        if (_isPaused) { return; }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayer(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlayer(Direction.Right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MovePlayer(Direction.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MovePlayer(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Player.PlayAttackAnim();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Player.GetDamaged(10);
        }
    }

    public void Initiatialize()
    {
        TileGenerator.Initialize(this);
        TileGenerator.OnTileMoving.AddListener(SetPause);
        TileGenerator.OnTileMoveEnd.AddListener(OnUpdatePlayerPosition);
    }

    public void SetPause(bool isPause)
    {
        _isPaused = isPause;
    }

    private void OnUpdatePlayerPosition()
    {
        PlayerPosition = TileGenerator.CurrentTilePos;
        OnPlayerMove.Invoke();
    }

    private void MovePlayer(Direction direction)
    {
        if (_isPaused) { return; }

        Player.PlayMoveAnim();
        TileGenerator.MoveToDirection(direction);
    }
}
