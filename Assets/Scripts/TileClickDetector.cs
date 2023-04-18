using UnityEngine;
using UnityEngine.Tilemaps;

public class TileClickDetector : MonoBehaviour
{
    public Camera mainCamera;
    public Grid grid;
    public Tilemap tilemap;

    private FloodFill _breathSearchFirst;
    
    private void Start()
    {
        _breathSearchFirst = GetComponent<FloodFill>();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
                new Vector3( Input.mousePosition.x,
                                    Input.mousePosition.y,
                                    - mainCamera.transform.position.z));
            Vector3Int cellPosition = grid.WorldToCell(mousePosition);
            Vector3 cellWorldPosition = grid.GetCellCenterWorld(cellPosition);
            if (tilemap.HasTile(cellPosition))
            {
                Debug.Log("Tile clicked: " + cellPosition + " World Position" + cellWorldPosition);
            }
            
            // Start FloodFill
            _breathSearchFirst.Origin = cellPosition;
            _breathSearchFirst.TileMap = tilemap;
            StartCoroutine(_breathSearchFirst.FloodFill2D());
        }        
    }
}
