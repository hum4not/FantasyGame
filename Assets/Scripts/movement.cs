using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class TileHighlighting : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase originalTile;
    public TileBase lightedTile;
    public GameObject player;
    public Grid gridToChange;
    public Grid test;

    private List<Vector3Int> availableTiles = new List<Vector3Int>();
    private Dictionary<TileBase, Sprite> tilesDictionary = new Dictionary<TileBase, Sprite>();

    private bool canMove = false;

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

            Debug.Log($"hit collider: {hit.collider}");

            if (hit.collider != null && hit.collider.gameObject == player)
            {
                canMove = true;
                HighlightAvailableTiles();
            }
            else/* if (availableTiles.Contains(tilemap.WorldToCell(mousePos)))*/
            {
                Debug.Log("Moving player");
                MovePlayer(mousePos);

            }
            Debug.Log(availableTiles.Contains(tilemap.WorldToCell(mousePos)));
            Debug.Log($"You are trying to move on: {cellPosition}");
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
                if (dx == 0 && dy == 0) continue; // пропускать активную ячейку

                Vector3Int neighborCell = playerCell + new Vector3Int(dx, dy, 0);
                if (tilemap.HasTile(neighborCell))
                {
                    if (IsTileWalkable(neighborCell))
                    {
                        tilemap.SetTile(neighborCell, lightedTile);
                        availableTiles.Add(neighborCell);

                        var tileBase = tilemap.GetTile(neighborCell);
                        var tile = tilemap.GetTile(neighborCell) as Tile;
                        tilesDictionary.Add(tileBase, tile.sprite);
                    }
                }
            }
        }
        Debug.Log(availableTiles.Count);
        if (availableTiles.Count > 0)
        {
            foreach (var position in availableTiles)
            {
                var gridPosition = tilemap.WorldToCell(position);
               /// tilemap.SetTileFlags(gridPosition, TileFlags.None);
                //var tile = tilemap.GetTile(gridPosition) as Tile;
                //tile.color = Color.red;
                tilemap.RefreshAllTiles();
            }
        }
    }

    bool IsTileWalkable(Vector3Int cell)
    {
        //добавить проверку если можно ходить(например для воды/лавы/если на ячейке уже есть персонаж)
        return true;
    }

    void MovePlayer(Vector3 targetWorldPosition)
    {
        if (availableTiles.Contains(tilemap.WorldToCell(targetWorldPosition)))
        {
            Vector3Int targetCell = tilemap.WorldToCell(targetWorldPosition);
            Vector3 targetPosition = tilemap.GetCellCenterWorld(targetCell);
            if (canMove)
            {
                player.transform.position = targetPosition;
                ResetTileColors();
            }
        }
        else
        {
            throw new Exception();
        }
    }

    void ResetTileColors()
    {
        foreach (Vector3Int cell in availableTiles)
        {
            tilemap.SetTile(cell, originalTile);
        }

        for (int i = 0; i < tilesDictionary.Count; i++)
        {
            var tile = tilesDictionary.ElementAt(i).Key as Tile;
            tile.sprite = tilesDictionary.ElementAt(i).Value;
        }

        tilesDictionary.Clear();
        tilemap.RefreshAllTiles();
        availableTiles.Clear();
        //test = gridToChange;
    }
}