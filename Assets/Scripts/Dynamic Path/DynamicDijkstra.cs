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
    public Tilemap pathTileMap;
    
    private PriorityQueue<Vector3> _frontier = new PriorityQueue<Vector3>();
    private Dictionary<Vector3, Vector3> _cameFrom = new Dictionary<Vector3, Vector3>();
    private Dictionary<Vector3, double> _costSoFar = new Dictionary<Vector3, double>();
    

    public void Dijkstra2D()
    {
        _frontier.Clear();
        _cameFrom.Clear();
        _costSoFar.Clear();
        
        _frontier.Enqueue(Origin,0);
        _cameFrom[Origin] = Vector3.zero;
        _costSoFar[Origin] = 0;
        
        int maxIterations = 1000;
        int iterations = 0;
        
        while (_frontier.Count > 0 && iterations < maxIterations)
        {
            Vector3 current = _frontier.Dequeue();
            
            if (earlyExit && current == Goal) break;
            
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
            iterations++;
        }
        DrawPath(Goal);
    }

    private double GetCost(Vector3 next)
    {
        var nextTile = TileMap.GetTile(new Vector3Int((int) next.x, (int) next.y, (int) next.z));
        double cost = nextTile.name switch
        {
            "isometric_pixel_0055" => 15,
            "isometric_pixel_0059" => 10,
            "isometric_pixel_0061" => 7,
            "isometric_pixel_0060" => 5,
            "isometric_pixel_0062" => 3,
            "isometric_pixel_0053" => 1,
            _ => 1
        };
        return cost;
    }
    
    private void DrawPath(Vector3 goal)
    {
        Vector3 current = goal;
        int maxIterations = 1000;
        int iterations = 0;
        while (current != Origin && iterations < maxIterations)
        {
            Vector3Int currentInt = new Vector3Int((int) current.x, (int) current.y, (int) current.z);
            pathTileMap.SetTile(currentInt, pathTile);
            current = _cameFrom[current];
            iterations++;
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
