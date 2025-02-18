using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;




#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor
{

#if UNITY_EDITOR
    // 단축키 옵션
    // % Ctrl, # Shift, & Alt
    [MenuItem("Tools/GenerateMap %#g")]
    private static void GenerateMap()
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject go in gameObjects)
        {
            Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

            // 파일 생성
            using (var writer = File.CreateText($"Assets/Resources/Map/{go.name}.txt"))
            {
                writer.WriteLine(tm.cellBounds.xMin);
                writer.WriteLine(tm.cellBounds.xMax);
                writer.WriteLine(tm.cellBounds.yMin);
                writer.WriteLine(tm.cellBounds.yMax);

                // 타일 맵을 순회하며, 해당위치에 타일이 존재한다면 1, 존재하지 않다면 0을 출력한다.
                for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
                {
                    for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
                    {
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }
            }
        }
    }

#endif

}
