using System.Collections.Generic;
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
    
    private Dictionary<Tilemap, Vector3Int> _previousTilePosition = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _origin = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _goal = new Dictionary<Tilemap, Vector3Int>();
    
    private void Start()
    {
        foreach (var tilemap in tilemaps)
        {
            _previousTilePosition[tilemap] = new Vector3Int(-1, -1, 0);
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
                subTilemap.SetTransformMatrix(_previousTilePosition[subTilemap], Matrix4x4.identity);
            }
            if (subTilemap.HasTile(tilePosition))
            {
                subTilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(offset, Quaternion.Euler(0, 0, 0), Vector3.one));
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
}
