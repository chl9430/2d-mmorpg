using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{
    public Tilemap _tilemap;
    public TileBase _tile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tilemap.SetTile(new Vector3Int(0,0,0), _tile);
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3Int> blocked = new List<Vector3Int>();

        // 타일맵 범위를 모두 순회하고, 타일이 있다면 그 위치를 리스트에 삽입한다.
        foreach (Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _tilemap.GetTile(pos);

            if (tile != null)
                blocked.Add(pos);
        }
    }
}
