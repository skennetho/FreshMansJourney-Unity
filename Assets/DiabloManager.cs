using System;
using UnityEngine;

public class DiabloManager : MonoBehaviour
{
    public DiabloPlayer Player;
    public Vector2 playerPosition;

    [Header("Tilemap")]
    public Sprite tileSprite;
    public int tileWidthCount;
    public int tileHeightCount;
    public float tileMargin = 1.2f;
    public TileInfo[,] tileInfo;
    public Transform tileParent;

    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    private void Awake()
    {
        InitiatedDiable();
        ReferenceHolder.TryRegister(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)
        {

        }
    }

    public void InitiatedDiable()
    {
        tileInfo = new TileInfo[tileHeightCount, tileWidthCount];
        playerPosition = new Vector2(tileHeightCount / 2, tileWidthCount / 2);
    }

    private void GenerateTileInfo()
    {
        SpriteRenderer[] tileSprites = tileParent.GetComponentsInChildren<SpriteRenderer>();
            
        for (int i = 0; i < tileInfo.GetLength(0); i++)
        {
            for (int j = 0; j < tileInfo.GetLength(1); j++)
            {
                tileInfo[i, j] = new TileInfo();
                tileInfo[i, j].Reset();
                if (i == playerPosition.x && j == playerPosition.y)
                {
                    tileInfo[i, j].Clear();
                }
            }
        }
    }

    //1:right, 2:left, 3:up, 4:down
    private void MoveTile(int direction) 
    {
        if (direction == 1)
        {
            if (playerPosition.y < tileWidthCount - 1)
            {
                playerPosition.y++;
            }
        }
        else if (direction == 2)
        {
            if (playerPosition.y > 0)
            {
                playerPosition.y--;
            }
        }
        else if (direction == 3)
        {
            if (playerPosition.x > 0)
            {
                playerPosition.x--;
            }
        }
        else if (direction == 4)
        {
            if (playerPosition.x < tileHeightCount - 1)
            {
                playerPosition.x++;
            }
        }
    }
    public void DrawTile()
    {

    }

    public void DrawSingleTile(int x, int y, int tileType)
    {

    }

    public Vector2 GetTilePosition(int x, int y)
    {
        Vector2 pos = Vector3.zero;


        return new Vector2(x * tileMargin, y * tileMargin);
    }

    public class TileInfo
    {
        bool isEnemyOn = false;
        bool isHealthOn = false;
        bool isTileHidden = false;
        Color tileColor = Color.white;

        public void Reset()
        {
            isEnemyOn = UnityEngine.Random.Range(0, 100) <= 10 ? true : false;
            isHealthOn = (!isEnemyOn && UnityEngine.Random.Range(0, 100) <= 10) ? true : false;
            tileColor = RandomColor();
        }

        public void Clear()
        {
            isEnemyOn = false;
            isHealthOn = false;
        }

        private Color RandomColor()
        {
            int rand = UnityEngine.Random.Range(0, 100);
            if (rand <= 50)
            {
                return new Color(255, 200, 0);
            }
            else if (rand <= 30)
            {
                return new Color(145, 255, 0);
            }
            else
            {
                return new Color(255, 255, 100);
            }
        }
    }
}
