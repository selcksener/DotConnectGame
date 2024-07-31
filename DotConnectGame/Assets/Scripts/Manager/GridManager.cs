using System;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] public int row;//Number of rows of the level
    [SerializeField] public int column;//Number of columns of the level
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform cellParent;
    [SerializeField] private float offsetCameraSize=0.1f;
    
    public Cell[,] cells;
    public LevelInfo levelInfo;
    public ConnectController connectController;
    public void CreateGrid(LevelInfo info)
    {
        levelInfo = info;
        row = levelInfo.levelRow;
        column = levelInfo.levelColumn;
        cells = new Cell[row, column];
        
        for (int y = 0; y < column; y++)
        {
            for (int x = 0; x < row; x++)
            {
                Cell cellObject = PoolManager.Instance.cellPool.Dequeue();
                cellObject.transform.position = new Vector3(x, y, 0);
                cellObject.transform.GetChild(1).GetComponent<TextMeshPro>().text = $"[{x},{y}]";
                cellObject.name = $" Cell [{x}-{y}]";
                cellObject.Init(x, y, true);
                cells[x, y] = cellObject;
            }
        }

        //Change camera position and orthographic size
        float columnInt = (column-1) / 2.0f;
        float rowInt = (row-1) / 2.0f;

        float size = column > row ? (column ) : (row );
        float ratio = (float)Screen.height / (float)Screen.width * 0.5f;
        size *= ratio+offsetCameraSize;

        Camera.main.orthographicSize = size;
        Camera.main.transform.position = new Vector3(rowInt, columnInt, -10);
        PlayLevel();
    }

    /// <summary>
    /// loading level objects
    /// </summary>
    public void PlayLevel()
    {
        foreach (var connection in levelInfo.levelConnectionInfos)
        {
            cells[connection.levelStartIndex.x, connection.levelStartIndex.y].SetConnectConnect(connection.levelColor,
                GameManager.Instance.levelInfo.GetColorReference(connection.levelColor));
            cells[connection.levelEndIndex.x, connection.levelEndIndex.y].SetConnectConnect(connection.levelColor,
                GameManager.Instance.levelInfo.GetColorReference(connection.levelColor));
        }

        connectController.lineRenderer = PoolManager.Instance.linePool.Dequeue().LineRenderer;
    }
}