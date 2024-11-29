using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class TileHighlighting : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase highlightTile; //надо было раньше(юзлесс в апд)
    public TileBase originalTile;
    public GameObject player;
    private List<Vector3Int> availableTiles = new List<Vector3Int>();

    private void Start()
    {
        tilemap.color = Color.white;
       // ResetTileColors();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Vector3Int cellPosition = tilemap.WorldToCell(mousePos);
            Vector3 tilePosition = tilemap.CellToWorld(cellPosition);

            Debug.Log($"hit collider: {hit.collider}");

            if (hit.collider != null && hit.collider.gameObject == player)
            {
                HighlightAvailableTiles();
            }
            else/* if (availableTiles.Contains(tilemap.WorldToCell(mousePos)))*/
            {
                Debug.Log("yes");
                MovePlayer(tilePosition);

            }
        }
    }


    void HighlightAvailableTiles()
    {
        ResetTileColors();

        Vector3Int playerCell = tilemap.WorldToCell(player.transform.position);

        // проверка прилигающих 
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue; // пропускать активную €чейку

                Vector3Int neighborCell = playerCell + new Vector3Int(dx, dy, 0);
                if (tilemap.HasTile(neighborCell))
                {
                    // чек если можно ходить
                    if (IsTileWalkable(neighborCell))
                    {
                        tilemap.SetTile(neighborCell, highlightTile);

                        availableTiles.Add(neighborCell);

                    }
                }
            }
        }
        Debug.Log(availableTiles.Count);
        if (availableTiles.Count > 0)
        {
            foreach (var position in availableTiles)
            {
                //  var tile = tilemap.GetTile(position);
                var gridPosition = tilemap.WorldToCell(position);
                // var tile = tilemap.GetTile(gridPosition);
                tilemap.SetTileFlags(gridPosition, TileFlags.None);
                //tilemap.SetColor(gridPosition, Color.red);

                var tile = tilemap.GetTile(gridPosition) as Tile;
                tile.color = Color.red;

                tilemap.RefreshAllTiles();
                Debug.Log(position);
            }
        }
    }


    bool IsTileWalkable(Vector3Int cell)
    {
        //добавить проверку если можно ходить(например дл€ воды/лавы/если на €чейке уже есть персонаж)
        return true;
    }



    void MovePlayer(Vector3 targetWorldPosition)
    {
        Vector3Int targetCell = tilemap.WorldToCell(targetWorldPosition);
        Vector3 targetPosition = tilemap.GetCellCenterWorld(targetCell);

        Vector3 position = new Vector3(targetPosition.x, targetPosition.y, 0);
        Debug.Log(targetCell);
        foreach (var s in availableTiles)
        {
            Debug.Log(s);
        }
        if (availableTiles.Contains(Vector3Int.FloorToInt(position))) 
        {
            
            player.transform.position = position;
            ResetTileColors();
        }

    }

    void ResetTileColors()
    {
        foreach (Vector3Int cell in availableTiles)
        {
            //tilemap.SetTile(cell, originalTile);

            var tile = tilemap.GetTile(cell) as Tile;
            tile.color = Color.white;
        }
        availableTiles.Clear();
    }
}