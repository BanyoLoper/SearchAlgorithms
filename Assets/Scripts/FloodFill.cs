using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloodFill : MonoBehaviour
{
    public Vector3 Origin { get; set; }
    public Vector3 Goal { get; set; }
    public Tilemap TileMap { get; set; }
    
    public TileBase visitedTile;
    public TileBase pathTile;
    public float delay = 0.2f;
    
    private Queue<Vector3> _frontier = new Queue<Vector3>();
    private Dictionary<Vector3, Vector3> _cameFrom = new Dictionary<Vector3, Vector3>();
    

    public IEnumerator FloodFill2D()
    {
        _frontier.Enqueue(Origin);
        _cameFrom[Origin] = Vector3.zero;
        
        while (_frontier.Count > 0)
        {
            Vector3 current = _frontier.Dequeue();
            foreach (Vector3 next in GetNeighbors(current))
            {
                if (!_cameFrom.ContainsKey(next))
                {
                    yield return new WaitForSeconds(delay);
                    _frontier.Enqueue(next);
                    _cameFrom[next] = current;
                }
            }
        }
        DrawPath(Goal);
    }

    public void DrawPath(Vector3 goal)
    {
        Vector3 current = goal;
        while (current != Origin)
        {
            Vector3Int currentInt = new Vector3Int((int) current.x, (int) current.y, (int) current.z);
            TileFlags flags = TileMap.GetTileFlags(currentInt);
            TileMap.SetTile(currentInt, pathTile);
            TileMap.SetTileFlags(currentInt, flags);
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
            TileFlags flags = TileMap.GetTileFlags(neighbourInt);

            neighbours.Add(neighbour);
            TileMap.SetTile(neighbourInt, visitedTile);
            TileMap.SetTileFlags(neighbourInt, flags);
        }
    }
}
