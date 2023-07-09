using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DiabloManager : MonoBehaviour
{
    // constants
    public int DAMAGE_WRONG_KEY = 5;

    [HideInInspector] public UnityEvent<int> OnWarningLeftChange = new();
    public Timer Timer;
    private int _warningLeft = 3;

    [Header("Player")]
    [HideInInspector] public UnityEvent OnPlayerMove;
    [HideInInspector] public UnityEvent<GameEnd> OnGameEnd;
    public DiabloPlayer Player;
    public DirectionKeyController KeyController;
    public Vector2 PlayerPosition;

    [Header("Monster")]
    public List<Monster> _monsterPrefabs;

    [Header("Tiles")]
    public TileGenerator TileGenerator;

    private bool _isPaused = false;

    public bool IsPaused => _isPaused;
    public int WarningLeft => _warningLeft;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        TileGenerator.Initialize(this);
        TileGenerator.OnTileMoving.AddListener(SetPause);
        TileGenerator.OnTileMoveEnd.AddListener(OnUpdatePlayerPosition);

        Player.OnDeath.AddListener(() => OnGameEnd.Invoke(GameEnd.PlayerDeath));
        Player.OnMaxLevel.AddListener(() => OnGameEnd.Invoke(GameEnd.GameClear));

        OnGameEnd.AddListener(isGameClear =>
        {
            SetPause(true);
            Timer.Pause();
        });

        KeyController.Initialize(this);
        KeyController.SetDirectionKeys('a', 'd', 'w', 's');

        Timer.Reset();
        Timer.OnTimeEnd.AddListener(WarnTimeEnd);
        Player.OnLevelUpdate.AddListener(level=> {
            if (level >= DiabloPlayer.MAX_LEVEL / 2)
            {
                Timer.Reset(Timer.MaxSec / 3 * 2);
            }
        });
        
        ReferenceHolder.Request<InputManager>(OnGetInputManager);
    }

    private void OnGetInputManager(InputManager inputManager)
    {
        inputManager.OnAlphabetInput.AddListener(OnAlphabetInput);
        ReferenceHolder.TryRegister(this);
    }

    public void OnAlphabetInput(char inputKey)
    {
        if (_isPaused) { return; }

        Direction dir = KeyController.InputKeyboard(inputKey);
        if (dir == Direction.None)
        {
            int damage = DAMAGE_WRONG_KEY;
            for(int i = 0; i < 4; i++)
            {
                var nearDir = (Direction)i;
                var nearTile = TileGenerator.NearByTiles[nearDir];

                if (nearTile.TileType == TileType.Monster)
                {
                    nearTile.CurrentMonster.Attack(Player);
                }
            }

            Player.GetDamaged(damage);
            return;
        }

        Timer.Reset();
        KeyController.SetDirectionKeyRandomly(dir);
        var dirTile = TileGenerator.NearByTiles[dir];
        if (dirTile.TileType == TileType.Normal)
        {
            MovePlayer(dir);
        }
        else if (dirTile.TileType == TileType.Monster)
        {
            Player.Attack(dirTile.CurrentMonster);
        }
    }

    private void MovePlayer(Direction direction)
    {
        if (_isPaused) { return; }

        Player.PlayMoveAnim();
        Player.FaceDirection(direction);
        TileGenerator.MoveToDirection(direction);
        OnPlayerMove.Invoke();
    }

    public void SetPause(bool isPause)
    {
        _isPaused = isPause;
    }

    private void OnUpdatePlayerPosition()
    {
        PlayerPosition = TileGenerator.TilePosToMapPos(TileGenerator.CurrentTilePos);

        // update new key
        for (int i = 0; i < 4; i++)
        {
            var nearDir = (Direction)i;
            var nearType = TileGenerator.GetTileType(nearDir, PlayerPosition);
            if (nearType == TileType.Blocked)
            {
                KeyController.SetDirectionKeyNone(nearDir);
                continue;
            }
            else
            {
                if (KeyController.GetKey(nearDir) == DirectionKeyController.NONE_KEY_CHAR)
                {
                    KeyController.SetDirectionKeyRandomly(nearDir);
                }
            }
        }
    }

    private void WarnTimeEnd()
    {
        _warningLeft--;
        OnWarningLeftChange.Invoke(_warningLeft);
        if (_warningLeft <= 0)
        {
            OnGameEnd.Invoke(GameEnd.TimeOver);
        }
        else
        {
            Timer.Reset();
        }
    }
}

public enum GameEnd
{
    PlayerDeath,
    GameClear,
    TimeOver
}