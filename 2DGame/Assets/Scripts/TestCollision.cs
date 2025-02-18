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

        // Ÿ�ϸ� ������ ��� ��ȸ�ϰ�, Ÿ���� �ִٸ� �� ��ġ�� ����Ʈ�� �����Ѵ�.
        foreach (Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _tilemap.GetTile(pos);

            if (tile != null)
                blocked.Add(pos);
        }
    }
}
