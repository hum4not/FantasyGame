using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

public class movement2 : MonoBehaviour
{
    //нельзя пройти больше 5 тайлов, если более то путь отображается красной линией и не происходит перемещение.
    //ЕСЛИ менее ппяти тайлов то зеленая линия и с задержкой в пол сек происходит пермещение
    //линия отображает путь по центрам всех тайлов, персонаж должен обходить препятствия

    public Tilemap tilemap;
    public Transform player;
    public float moveSpeed = 5f;
    public float tileSize = 1f; // Примерный размер тайла
    private Vector3Int currentTile;
    private List<Vector3Int> path;
    private int pathIndex = 0;
    private Vector3Int offset = new Vector3Int(0, 1, 0);

    void Start()
    {
        if (tilemap == null || player == null)
        {
            Debug.LogError("Не назначена тайловая карта или персонаж.");
            enabled = false;
            return;
        }
        currentTile = tilemap.WorldToCell(player.position);
        player.position = tilemap.GetCellCenterWorld(currentTile);
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int targetTile = tilemap.WorldToCell(worldPos);

            if (currentTile != targetTile)
            {
                path = FindPath(currentTile, targetTile);

                if (path != null && path.Count > 0)
                {
                    pathIndex = 0;
                }

            }

        }
        MovePlayer();
    }

    void MovePlayer()
    {
        if (path != null && pathIndex < path.Count)
        {
            Vector3 nextCell = tilemap.GetCellCenterWorld(path[pathIndex]);
            player.position = Vector3.MoveTowards(player.position, nextCell, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(player.position, nextCell) < 0.01f)
            {
                currentTile = path[pathIndex];
                pathIndex++;
            }


        }
    }

    List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> costSoFar = new Dictionary<Vector3Int, float>();
        PriorityQueue<Vector3Int> frontier = new PriorityQueue<Vector3Int>();

        frontier.Enqueue(start, 0);
        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (Vector3Int next in GetNeighbors(current))
            {

                float newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {

                    costSoFar[next] = newCost;
                    float priority = newCost + HexDistance(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;

                }
            }

        }

        if (!cameFrom.ContainsKey(goal))
        {
            return null; // Путь не найден
        }

        return ReconstructPath(cameFrom, start, goal);

    }

    List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int current = goal;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
            if (current == start)
                break;
        }
        path.Reverse();
        return path;
    }

    Vector3Int[] GetNeighbors(Vector3Int tile)
    {

        Vector3Int[] neighbors = {
                tile + new Vector3Int(1, 0, 0),  // Right
                tile + new Vector3Int(-1, 0, 0),  // Left
                tile + new Vector3Int(0, 1, 0),    // Upper right
                tile + new Vector3Int(0, -1, 0), // Lower left
                tile + new Vector3Int(1, 1, 0),   // Upper left
                tile + new Vector3Int(-1, -1, 0) // Lower right
        };
        return neighbors;
    }


    float HexDistance(Vector3Int a, Vector3Int b)
    {

        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        int dz = Mathf.Abs(a.x + a.y - b.x - b.y);
        return Mathf.Max(dx, dy, dz);

    }



}

public class PriorityQueue<T>
{
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
        elements.Sort((a, b) => a.Value.CompareTo(b.Value));
    }

    public T Dequeue()
    {
        if (elements.Count == 0)
        {
            throw new System.InvalidOperationException("PriorityQueue is empty");
        }
        T item = elements[0].Key;
        elements.RemoveAt(0);
        return item;
    }

}
