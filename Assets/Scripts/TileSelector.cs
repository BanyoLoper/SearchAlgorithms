using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

// Note: Set Custom Axis in Graphcs Settings > Transaprency Sort Mode to (0,1,-1)

public class TileSelector : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap[] tilemaps;
    public Vector3 offset = new Vector3(0, 0.03f,0);
    public TileBase originTile;
    public TileBase goalTile;
    public FloodFill floodFill;
    public Dijkstra dijkstra;


    public enum SearchAlgorithm
    {
        FloodFill,
        Dijkstra,
        AStar
    }

    public SearchAlgorithm selectedAlgorithm;
    
    private Dictionary<Tilemap, Vector3Int> _previousTilePosition = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _origin = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _goal = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Vector3Int, Vector3> _originalOffset = new Dictionary<Vector3Int, Vector3>();
    
    private void Start()
    {
        foreach (var tilemap in tilemaps)
        {
            _previousTilePosition[tilemap] = new Vector3Int(-1, -1, 0);
            _originalOffset[_previousTilePosition[tilemap]] = Vector3.zero;
        }
    }

    void Update()
    {
        foreach (var tilemap in tilemaps)
        {
            SelectTile(tilemap);
        }
        if (Input.GetMouseButtonDown(0)) DetectTileClick(isOrigin:true);
        if (Input.GetMouseButtonDown(1)) DetectTileClick(isOrigin:false);
        if (Input.GetKeyDown(KeyCode.Return)) StartAlgorithm();
    }

    private void SelectTile(Tilemap subTilemap)
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = subTilemap.WorldToCell(mousePosition);
        tilePosition.z = 0;
        
        if (!tilePosition.Equals(_previousTilePosition[subTilemap]))
        {
            if (subTilemap.HasTile(_previousTilePosition[subTilemap]))
            {
                Vector3 originalOffset = _originalOffset[_previousTilePosition[subTilemap]];
                subTilemap.SetTransformMatrix(_previousTilePosition[subTilemap],
                    Matrix4x4.TRS(originalOffset, Quaternion.identity, Vector3.one));
            }
            if (subTilemap.HasTile(tilePosition))
            {
                Vector3 currentOffset = subTilemap.GetTransformMatrix(tilePosition).GetColumn(3);
                _originalOffset[tilePosition] = currentOffset;
                subTilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(currentOffset + offset, Quaternion.Euler(0, 0, 0), Vector3.one));
            }
            _previousTilePosition[subTilemap] = tilePosition;
        }
    }

    private void DetectTileClick(bool isOrigin)
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        TileBase newTile = isOrigin ? originTile : goalTile;
        Dictionary<Tilemap, Vector3Int> selectedDictionary = isOrigin ? _origin : _goal;

        foreach (var tilemap in tilemaps)
        {
            var clickedTile = tilemap.WorldToCell(mousePosition);
            clickedTile.z = 0;
            
            if (tilemap.HasTile(clickedTile))
            {
                var oldTile = tilemap.GetTile(clickedTile);
                tilemap.SetTile(clickedTile, newTile);
                if (selectedDictionary.ContainsKey(tilemap)) tilemap.SetTile(selectedDictionary[tilemap], oldTile);
                selectedDictionary[tilemap] = clickedTile;
                break;
            }
        }
    }

    private void StartAlgorithm()
    {
        switch (selectedAlgorithm)
        {
            case SearchAlgorithm.FloodFill:
                StartFloodFill();
                break;
            case SearchAlgorithm.Dijkstra:
                StartDijkstra();
                break;
            case SearchAlgorithm.AStar:
                break;
        }
    }
    
    private void StartFloodFill()
    {
        // To do: Add FloodFill if do not exists
        foreach (var tilemap in tilemaps)
        {
            if (_origin[tilemap].Equals(_goal[tilemap])) return;
            
            // Start FloodFill
            floodFill.Origin = _origin[tilemap];
            floodFill.Goal = _goal[tilemap];
            floodFill.TileMap = tilemap;
            floodFill.visitedTile = originTile;
            floodFill.pathTile = goalTile;
            StartCoroutine(floodFill.FloodFill2D());
        }
    }
    
    private void StartDijkstra()
    {
        // To do: Add Dijkstra if do not exists
        foreach (var tilemap in tilemaps)
        {
            if (_origin[tilemap].Equals(_goal[tilemap])) return;
            dijkstra.Origin = _origin[tilemap];
            dijkstra.Goal = _goal[tilemap];
            dijkstra.TileMap = tilemap;
            dijkstra.visitedTile = originTile;
            dijkstra.pathTile = goalTile;
            dijkstra.camera = mainCamera;
            StartCoroutine(dijkstra.Dijkstra2D());
        }   
    }
}
