using UnityEngine;
using UnityEngine.Tilemaps;

public class TileClickDetector : MonoBehaviour
{
    public Camera mainCamera;
    public Grid grid;
    public Tilemap tilemap;
    public TileBase startTile;
    public TileBase goalTile;
    
    private FloodFill _breathSearchFirst;
    private Vector3Int _origin;
    private Vector3Int _goal;
    
    private void Start()
    {
        _breathSearchFirst = GetComponent<FloodFill>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_origin.Equals(_goal))
            {
                Debug.Log("Origin or Goal not set");
                return;
            }
        
            // Start FloodFill
            _breathSearchFirst.Origin = _origin;
            _breathSearchFirst.Goal = _goal;
            _breathSearchFirst.TileMap = tilemap;
            StartCoroutine(_breathSearchFirst.FloodFill2D());
        }

        if (Input.GetMouseButtonDown(0))
        {
            _origin = GetTileCoordinate();
            tilemap.SetTile(_origin, startTile);
        }

        if (Input.GetMouseButtonDown(1))
        {
            _goal = GetTileCoordinate();
            tilemap.SetTile(_goal, goalTile);
        }
    }

    private Vector3Int GetTileCoordinate()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
            new Vector3( Input.mousePosition.x,
                Input.mousePosition.y,
                - mainCamera.transform.position.z));
        Vector3Int cellPosition = grid.WorldToCell(mousePosition);
        Vector3 cellWorldPosition = grid.GetCellCenterWorld(cellPosition);
        if (tilemap.HasTile(cellPosition))
        {
            return cellPosition;
        }

        return new Vector3Int(0, 0, 0);
    }
}
