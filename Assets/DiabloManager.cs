using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiabloManager : MonoBehaviour
{
    [Header("Player")]
    [HideInInspector] public UnityEvent OnPlayerMove;
    [HideInInspector] public UnityEvent<bool> OnGameEnd;
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
        Initialize();
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
            Player.LevelUp();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Player.GetDamaged(10);
        }
    }

    public void Initialize()
    {
        TileGenerator.Initialize(this);
        TileGenerator.OnTileMoving.AddListener(SetPause);
        TileGenerator.OnTileMoveEnd.AddListener(OnUpdatePlayerPosition);

        OnGameEnd.AddListener(isGameClear => SetPause(true));
        Player.OnDeath.AddListener(()=>OnGameEnd.Invoke(false));
        Player.OnMaxLevel.AddListener(()=>OnGameEnd.Invoke(true));
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
