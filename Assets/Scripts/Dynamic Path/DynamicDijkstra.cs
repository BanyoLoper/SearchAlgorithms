using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ESarkis;
using TMPro;

public class DynamicDijkstra : MonoBehaviour
{
    public Vector3 Origin { get; set; }
    public Vector3 Goal { get; set; }
    public Tilemap TileMap { get; set; }
    
    public TileBase pathTile;
    public bool earlyExit = false;
    public TextMeshProUGUI costTextPrefab;
    public Camera camera;
    public Canvas canvas;
    public Tilemap pathTileMap;
    
    private PriorityQueue<Vector3> _frontier = new PriorityQueue<Vector3>();
    private Dictionary<Vector3, Vector3> _cameFrom = new Dictionary<Vector3, Vector3>();
    private Dictionary<Vector3, double> _costSoFar = new Dictionary<Vector3, double>();
    private RectTransform _canvasRectTransform;

    private void Start()
    {
        _canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    public void Dijkstra2D()
    {
        _frontier.Enqueue(Origin,0);
        _cameFrom[Origin] = Vector3.zero;
        _costSoFar[Origin] = 0;
        
        while (_frontier.Count > 0)
        {
            Vector3 current = _frontier.Dequeue();
            
            if (earlyExit & current == Goal) break;
            
            foreach (Vector3 next in GetNeighbors(current))
            {
                var newCost = _costSoFar[current] + GetCost(next);
                if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                {
                    _costSoFar[next] = newCost;
                    _frontier.Enqueue(next, newCost);
                    _cameFrom[next] = current;
                }
            }
        }
        DrawPath(Goal);
    }

    private double GetCost(Vector3 next)
    {
        var nextTile = TileMap.GetTile(new Vector3Int((int) next.x, (int) next.y, (int) next.z));
        double cost = nextTile.name switch
        {
            "isometric_pixel_0055" => 5,
            "isometric_pixel_0059" => 3,
            "isometric_pixel_0060" => 1,
            "isometric_pixel_0061" => 4,
            "isometric_pixel_0062" => 2,
            _ => 1
        };

        TextMeshProUGUI costInstance = Instantiate(costTextPrefab, canvas.transform, false);
        costInstance.text = cost.ToString();

        Vector3 tileWorldPos = TileMap.CellToWorld(new Vector3Int((int)next.x, (int)next.y, (int)next.z));
        Vector3 viewportPos = camera.WorldToViewportPoint(tileWorldPos);
        
        var sizeDelta = _canvasRectTransform.sizeDelta;
        Vector2 anchoredPosition = new Vector2(
            ((sizeDelta.x * viewportPos.x) - (sizeDelta.x * 0.5f)),
            ((sizeDelta.y * viewportPos.y) - (sizeDelta.y * 0.5f))
        );
    
        costInstance.rectTransform.anchoredPosition = anchoredPosition;
        return cost;
    }
    
    private void DrawPath(Vector3 goal)
    {
        Vector3 current = goal;
        while (current != Origin)
        {
            Vector3Int currentInt = new Vector3Int((int) current.x, (int) current.y, (int) current.z);
            pathTileMap.SetTile(currentInt, pathTile);
            current = _cameFrom[current];
        }
    }

    private List<Vector3> GetNeighbors(Vector3 current)
    {
        List<Vector3> neighbours = new List<Vector3>();
        CheckNeighbour(current + Vector3.right, neighbours);
        CheckNeighbour(current + Vector3.left, neighbours);
        CheckNeighbour(current + Vector3.up, neighbours);
        CheckNeighbour(current + Vector3.down, neighbours);
        return neighbours;
    }
    
    private void CheckNeighbour(Vector3 neighbour, List<Vector3> neighbours)
    {
        Vector3Int neighbourInt = new Vector3Int((int) neighbour.x, (int) neighbour.y, (int) neighbour.z);

        if (!TileMap.HasTile(neighbourInt)) return;
        if (!_frontier.Contains(neighbour))
        {
            neighbours.Add(neighbour);
        }
    }
}
