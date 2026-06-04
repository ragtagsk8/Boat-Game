using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualiser : MonoBehaviour
{
    [SerializeField] private Tilemap oceanTilemap;
    [SerializeField] private TileBase waterTiles;

    public void PaintOceanTiles(IEnumerable<Vector2Int> floorPositions) {
        PaintTiles(floorPositions, oceanTilemap, waterTiles);
        oceanTilemap.RefreshAllTiles();
    }

    [SerializeField] private Tilemap landTilemap;
    [SerializeField] private TileBase landTiles;

    public void PaintLandTiles(IEnumerable<Vector2Int> floorPositions) {
        PaintTiles(floorPositions, landTilemap, landTiles);
        landTilemap.RefreshAllTiles();
    }

    [SerializeField] private Tilemap rockTilemap;
    [SerializeField] private TileBase rockTiles;

    public void PaintRockTile(Vector2Int position) {
        PaintSingleTile(rockTilemap, rockTiles, position);
        rockTilemap.RefreshAllTiles();
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) {
        foreach (var position in positions) {
            PaintSingleTile(tilemap, tile, position);
        }
    }
    /*private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }*/
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) {
        tilemap.SetTile((Vector3Int)position, tile);
    }

    public void Clear() {
        oceanTilemap.ClearAllTiles();
        landTilemap.ClearAllTiles();
        rockTilemap.ClearAllTiles();
    }
}
