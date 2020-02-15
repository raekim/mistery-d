using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGrid : MonoBehaviour
{
    Player player;

    [SerializeField] Color hoverTileColor = Color.red;
    [SerializeField] Color canMoveTileColor = Color.blue; // color of 8 directions tile

    Vector2 mousePos;
    Vector3Int curCellPos;
    Color originalCellColor;
    Vector3Int playerCellPos;
    
    Grid grid;
    Tilemap tileMap;

    bool firstCellPosSet = false;


    private void Awake()
    {
        player = FindObjectOfType<Player>();
        grid = GetComponent<Grid>();
        tileMap = GetComponentInChildren<Tilemap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("origin:" + tileMap.origin);
        curCellPos = tileMap.WorldToCell(player.transform.position);
        PlacePlayerOnCell(curCellPos);
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse position and convert it to tile position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int newCellPos = tileMap.WorldToCell((Vector3)mousePos);

        ChangeHoveredOverCellColor(newCellPos);
        if (Input.GetMouseButtonDown(0))
        {
            if (tileMap.HasTile(curCellPos))
            {
                PlacePlayerOnCell(curCellPos);
            }
        }
    }

    private void ChangeHoveredOverCellColor(Vector3Int newCellPos)
    {
        if (!firstCellPosSet || curCellPos != newCellPos)
        {
            // 이전 타일 색깔 되돌린다
            if (firstCellPosSet && tileMap.HasTile(curCellPos))
            {
                tileMap.SetTileFlags(curCellPos, TileFlags.None);
                tileMap.SetColor(curCellPos, originalCellColor);
            }

            firstCellPosSet = true;

            curCellPos = newCellPos;

            if (tileMap.HasTile(curCellPos))
            {
                originalCellColor = tileMap.GetColor(curCellPos);
                // 선택된 타일 색깔 빨간색으로
                tileMap.SetTileFlags(curCellPos, TileFlags.None);
                tileMap.SetColor(curCellPos, hoverTileColor);
            }
        }
    }

    private void PlacePlayerOnCell(Vector3Int cellPos)
    {
        // color previous 8 direction tiles to white
        for (int dx = -1; dx <= 1; dx++)  // -1, 0, 1
        {
            for (int dy = -1; dy <= 1; dy++) // -1, 0, 1
            {
                Vector3Int pos;
                pos = playerCellPos + new Vector3Int(dx, dy, 0);
                tileMap.SetTileFlags(pos, TileFlags.None);
                tileMap.SetColor(pos, Color.white);
            }
        }

        // place player object onto the cell
        Vector3 newPlayerPos = tileMap.CellToWorld(cellPos);
        newPlayerPos.z = player.transform.position.z;
        player.transform.position = newPlayerPos;

        // player cell position update
        playerCellPos = cellPos;

        // color current 8 direction tiles to the designated color
        for (int dx = -1; dx <= 1; dx++)  // -1, 0, 1
        {
            for (int dy = -1; dy <= 1; dy++) // -1, 0, 1
            {
                Vector3Int pos;
                pos = playerCellPos + new Vector3Int(dx, dy, 0);
                tileMap.SetTileFlags(pos, TileFlags.None);
                tileMap.SetColor(pos, canMoveTileColor);
            }
        }
    }
}
