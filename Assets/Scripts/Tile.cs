using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    public Vector2 TilePosition;

    private bool _isAvail = false;
    private SpriteRenderer _tileSprite;

    private bool IsAvail => _isAvail;

    private void OnDrawGizmos()
    {
        // draw tileposition text using handles with larger font
        Handles.Label(transform.position, $"[{(int)TilePosition.x},{(int)TilePosition.y}]", new GUIStyle() { fontSize = 10 });

    }

    private void Awake()
    {
        _tileSprite = GetComponent<SpriteRenderer>();
        //change sprite color randomly minutely between green and yellow
        _tileSprite.color = new Color(0.5f, Random.Range(0.5f, 1f), 0.5f);
    }

    public void SpawnMonster()
    {

    }

    private void GetRandomMonster()
    {

    }

    public void Log()
    {
        Debug.Log($"TilePosition: {TilePosition} , localPos:{transform.localPosition}, realPos:{transform.position}",gameObject); 
    }
}