using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Spawn : MonoBehaviour
{
    [SerializeField] private Text tilePosition;
    [SerializeField] private Tilemap tilemap;



    void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    var position = new Vector3(x, y, 0);
                    var text = Instantiate(tilePosition.gameObject, position, Quaternion.identity);
                    text.transform.localScale = new Vector3(1,1,1);
                    text.transform.SetParent(transform, true);

                    text.GetComponent<Text>().text = "x:" + x + " y:" + y;
                    Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                }
                else
                {
                    Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }
            }
        }
    }
}
