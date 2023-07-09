using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiabloManager : MonoBehaviour
{
    // constants
    public int DAMAGE_PER_WRONG_KEY = 5;
    public int DAMAGE_FROM_MONSTER = 10;

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

    private void Update()
    {
        if (_isPaused) { return; }

        // test codes
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Player.PlayAttackAnim();
            Player.LevelUp();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Player.GetDamaged(DAMAGE_PER_WRONG_KEY);
        }
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
            Player.GetDamaged(DAMAGE_PER_WRONG_KEY);
            return;
        }

        MovePlayer(dir);
        //KeyController.SetDirectionKeyRandomly(dir);
        KeyController.SetDirectionKeys('a', 'd', 'w', 's'); //test
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
        PlayerPosition = TileGenerator.CurrentTilePos;
        OnPlayerMove.Invoke();
    }
}
