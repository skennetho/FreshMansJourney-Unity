using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiabloManager : MonoBehaviour
{
    // constants
    public int DAMAGE_WRONG_KEY = 5;

    [Header("Player")]
    [HideInInspector] public UnityEvent OnPlayerMove;
    [HideInInspector] public UnityEvent<bool> OnGameEnd;
    public DiabloPlayer Player;
    public DirectionKeyController KeyController;
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
    }

    public void Initialize()
    {
        TileGenerator.Initialize(this);
        TileGenerator.OnTileMoving.AddListener(SetPause);
        TileGenerator.OnTileMoveEnd.AddListener(OnUpdatePlayerPosition);

        Player.OnDeath.AddListener(() => OnGameEnd.Invoke(false));
        Player.OnMaxLevel.AddListener(() => OnGameEnd.Invoke(true));

        OnGameEnd.AddListener(isGameClear => SetPause(true));

        KeyController.Initialize(this);
        KeyController.SetDirectionKeys('a', 'd', 'w', 's');

        // intializing ends when inputmanger is retrieved
        ReferenceHolder.Request<InputManager>(OnRetrieveInputManager);
    }

    private void OnRetrieveInputManager(InputManager inputManager)
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

        OnPlayerMove.Invoke();
    }
}
