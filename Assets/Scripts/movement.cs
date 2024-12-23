using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using System;
using Unity.Loading;
using UnityEngine.Rendering;

public class TileHighlighting : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase originalTile;
    public TileBase lightedTile;
    public GameObject player;
    public GameObject prefab;

    private List<Vector3Int> availableTiles = new List<Vector3Int>();

    private bool canMove = false;



    private void Start()
    {
        tilemap.color = Color.white;
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
                MovePlayer(new Vector3Int((int)mousePos.x, (int)mousePos.y, (int)mousePos.z));

            }
            Debug.Log(availableTiles.Contains(tilemap.WorldToCell(mousePos)));
            Debug.Log($"You are trying to move on: {cellPosition}");
        }
    }

    void HighlightAvailableTiles()
    {
        ResetTileColors();


        Vector3Int playerCell = tilemap.WorldToCell(player.transform.position);
        Debug.LogError(playerCell);

        Vector3Int[] positions = new Vector3Int[6]
        {
            new Vector3Int(playerCell.x - 1, playerCell.y, 0),
            new Vector3Int(playerCell.x + 1, playerCell.y, 0),
            new Vector3Int(playerCell.x, playerCell.y + 1, 0),
            new Vector3Int(playerCell.x - 1, playerCell.y + 1, 0),
            new Vector3Int(playerCell.x, playerCell.y - 1, 0),
            new Vector3Int(playerCell.x - 1, playerCell.y - 1, 0)
        };


        for (int i  = 0; i < 5; i++)
        {
            Debug.LogError(positions[i]);
        }

        foreach (Vector3Int position in positions)
        {
            //if (position.x == 0 && position.y == 0) continue; // пропускать активную €чейку
            Debug.Log(position);
            if (tilemap.HasTile(position))
            {
                Debug.Log("meow meow");
                if (IsTileWalkable(position))
                {
                    Debug.Log("guf guf");
                    Tile kek = tilemap.GetTile(position) as Tile;
                    availableTiles.Add(position);
                    Debug.Log(kek.name + "\t" + position + "\t" + playerCell);
                    tilemap.SetTile(position, lightedTile);
                }
            }
        }

        //Vector3Int fix = new Vector3Int(playerCell.x + 1, playerCell.y, 0);
        //tilemap.SetTile(fix, lightedTile);
        //availableTiles.Add(fix);
        //Debug.Log(availableTiles.Count);
    }

    bool IsTileWalkable(Vector3Int cell)
    {
        //добавить проверку если можно ходить(например дл€ воды/лавы/если на €чейке уже есть персонаж)
        return true;
    }

    void MovePlayer(Vector3Int targetWorldPosition)
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
            Debug.LogError($"Target world position {targetWorldPosition}");
        }
    }

    void ResetTileColors()
    {
        foreach (Vector3Int cell in availableTiles)
        {
            tilemap.SetTile(cell, originalTile);
        }

        //for (int i = 0; i < tilesDictionary.Count; i++)
        //{
        //    var tile = tilesDictionary.ElementAt(i).Key as Tile;
        //    tile.sprite = tilesDictionary.ElementAt(i).Value;
        //}

        //tilesDictionary.Clear();
        tilemap.RefreshAllTiles();
        availableTiles.Clear();
        //ReplaceTilemap();
        //test = gridToChange;
    }

    public void ReplaceTilemap()
    {
        // ”дал€ем старый Tilemap
        Destroy(tilemap);

        // —оздаем новый Tilemap из префаба
        Instantiate(prefab, transform);
    }

}