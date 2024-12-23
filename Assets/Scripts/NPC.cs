using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class NPC : MonoBehaviour
{
    public Tilemap tilemap;

    public void move()
    {
        Vector3Int playerCell = tilemap.WorldToCell(this.transform.position);

        Vector3Int[] positions = new Vector3Int[6]
        {
            new Vector3Int(playerCell.x - 1, playerCell.y, 0),
            new Vector3Int(playerCell.x + 1, playerCell.y, 0),
            new Vector3Int(playerCell.x, playerCell.y + 1, 0),
            new Vector3Int(playerCell.x - 1, playerCell.y + 1, 0),
            new Vector3Int(playerCell.x, playerCell.y - 1, 0),
            new Vector3Int(playerCell.x - 1, playerCell.y - 1, 0)
        };

        Vector3Int position = positions[Random.Range(0, 6)];

        Vector3Int targetCell = tilemap.WorldToCell(position);
        Vector3 targetPosition = tilemap.GetCellCenterWorld(targetCell);

        transform.position = targetPosition;
        
    }
}
