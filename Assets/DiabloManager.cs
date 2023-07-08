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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TileGenerator.MoveToDirection(Direction.Right);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TileGenerator.MoveToDirection(Direction.Left);
            PlayerPosition.x++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TileGenerator.MoveToDirection(Direction.Down);
            PlayerPosition.y++;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TileGenerator.MoveToDirection(Direction.Up);
            PlayerPosition.y--;
        }
        PlayerPosition = TileGenerator.CurrentLocation;
    }

    public void Initiatialize()
    {
        TileGenerator.Initialize(this);
    }

}
