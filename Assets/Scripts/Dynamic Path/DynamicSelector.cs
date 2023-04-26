using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicSelector : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap[] tilemaps;
    public TileBase originTile;
    public TileBase pathTile;
    public DynamicDijkstra dijkstra;

    private Vector3Int? _originCoord = null;
    private Vector3Int _previousCoord;

    private void Update()
    {
        if(Input.GetMouseButton(0)) DetectTileClick();
        if (_originCoord.HasValue) DrawPath();
    }
    
    private void DetectTileClick()
    {
        tilemaps[1].ClearAllTiles();
        Vector3Int clickedTile = CurrentTileCoord();
        
        if (!tilemaps[0].HasTile(clickedTile)) return;

        _originCoord = new Vector3Int(clickedTile.x, clickedTile.y, clickedTile.z);
        tilemaps[1].SetTile(clickedTile, originTile);
    }

    private void DrawPath()
    {
        tilemaps[1].ClearAllTiles();
        var goal = CurrentTileCoord();
        if (_originCoord == _previousCoord || !tilemaps[0].HasTile(goal)) return;
        
        dijkstra.Origin = _originCoord.Value;
        dijkstra.Goal = goal;
        dijkstra.TileMap = tilemaps[0];
        dijkstra.pathTileMap = tilemaps[1];
        dijkstra.pathTile = pathTile;
        
        dijkstra.Dijkstra2D();
    }

    private Vector3Int CurrentTileCoord()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int current = tilemaps[0].WorldToCell(mousePosition);
        current.z = 0;
        return current;
    }
    
}
