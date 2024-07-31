using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    private static LevelGenerator instance;

    public static LevelGenerator Instance
    {
        get { return instance; }
    }


    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private int minDistancePath; // min distance between start node and end node
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform cellParent;
    [SerializeField] private LevelGeneratorPathFinding pathFinding;
    [SerializeField] private float offsetCameraSize;
    private List<ConnectColor> connectColorTemp;
    private int connectionCount;
    private float time;
    private bool simulate = false;
    public Grid grid;
    public LevelData levelInfo;
    [Header("UI")] public GameObject simulateButton;
    public GameObject stopSimulateButton;
    public GameObject simulateText;
    public GameObject saveButton;
    public GameObject reSimulateButton;

    private Coroutine _coroutine;

    private void Awake()
    {
        //Singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CreateGrid();
    }

    #region GRID SYSTEM

    private void CreateGrid()
    {
        connectionCount = Math.Min(row, column) - 1; //Count of numbers in the level
        grid = new Grid(row, column, minDistancePath, connectionCount, new LevelGeneratorCell[row, column]);
        for (int y = 0; y < column; y++)
        {
            for (int x = 0; x < row; x++)
            {
                LevelGeneratorCell cellObject = Instantiate(cellPrefab).GetComponent<LevelGeneratorCell>();
                cellObject.transform.position = new Vector3(x, y, 0);
                cellObject.transform.GetChild(1).GetComponent<TextMeshPro>().text = $"[{x},{y}]";
                cellObject.name = $" Cell [{x}-{y}]";
                cellObject.Init(x, y, true, ConnectColor.None);
                grid.cells[x, y] = cellObject;
                grid.cells[x, y] = cellObject;
                cellObject.gameObject.SetActive(true);
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

    }

    #endregion

    #region Simulate

    public void SimulateButton()
    {
        time = Time.time; //for system reset
        System.Array colors = System.Enum.GetValues(typeof(ConnectColor));
        List<ConnectColor> cols = colors.OfType<ConnectColor>().ToList();
        cols.RemoveAt(0);
        connectColorTemp = cols;
        pathFinding.Clear();
        CancelInvoke(nameof(SimulateButton));
        simulate = true;
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Simulate());
    }

    public IEnumerator Simulate()
    {
        while (simulate)
        {
            int startNodeX = Random.Range(0, grid.cells.GetLength(0));
            int startNodeY = Random.Range(0, grid.cells.GetLength(1));
            while (grid.cells[startNodeX, startNodeY].isAvailable == false) //until find a available cell
            {
                //
                startNodeX = Random.Range(0, grid.cells.GetLength(0));
                startNodeY = Random.Range(0, grid.cells.GetLength(1));
            }

            int endNodeX = Random.Range(0, grid.cells.GetLength(0));
            int endNodeY = Random.Range(0, grid.cells.GetLength(1));
            while ((endNodeX == startNodeX &&
                    endNodeY == startNodeY) || // start-end node equal or end node is not available
                   grid.cells[endNodeX, endNodeY].isAvailable == false)
            {
                endNodeX = Random.Range(0, grid.cells.GetLength(0));
                endNodeY = Random.Range(0, grid.cells.GetLength(1));
            }

            //Control between start-end node
            if (!CheckBetweenNodes(grid.cells[startNodeX, startNodeY], grid.cells[endNodeX, endNodeY]))
            {
                if (Time.time - time > 1f) // cannot be found within 5 seconds , system searches again
                {
                    time = Time.time;
                    pathFinding.Clear();

                    Invoke(nameof(SimulateButton), 0.5f);
                    yield break;
                }

            }
            else
            {
                LevelGeneratorCell startNode = grid.cells[startNodeX, startNodeY];
                LevelGeneratorCell endNode = grid.cells[endNodeX, endNodeY];
                ConnectColor rndColor = connectColorTemp[Random.Range(0, connectColorTemp.Count)];
                startNode.Connect(rndColor); //node connected
                endNode.Connect(rndColor);
                connectColorTemp.Remove(rndColor);
              
                // if count of nodes in the level is not equal count of nodes find
                // Find new available nodes
                yield return pathFinding.DrawLineRenderer(startNode, endNode);
                time = Time.time;
                if (pathFinding.activeConnectionCells.Count != connectionCount)
                {
                    pathFinding.ClearOnlyAvailable();
                }
                else
                {
                    Debug.Log("Generated new level!");
                    simulateText.SetActive(false);
                    stopSimulateButton.SetActive(false);
                    saveButton.SetActive(true);
                    reSimulateButton.SetActive(true);
                    yield break;
                }
            }

            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns></returns>
    public bool CheckBetweenNodes(LevelGeneratorCell startNode, LevelGeneratorCell endNode)
    {
        if (startNode.isAvailable == false || endNode.isAvailable == false)
            return false; // if node isn't avilable
        if (startNode.x == endNode.x && startNode.y == endNode.y)
            return false; //start node is equal end node
        if (Mathf.Abs((startNode.x - endNode.x) + (startNode.y - endNode.y)) < grid.minDistancePath)
            return false; // Distance between start node and end node
        if (pathFinding.PathFinding(startNode, endNode, grid) == false) return false;
        return true;
    }

    #endregion


    #region UI

    public void StartSimulate()
    {
        simulateButton.SetActive(false);
        stopSimulateButton.SetActive(true);
        simulateText.SetActive(true);
        saveButton.SetActive(false);
        reSimulateButton.SetActive(false);
        SimulateButton();
    }
    public void StopSimulate()
    {
        StopAllCoroutines();
        simulateButton.SetActive(true);
        stopSimulateButton.SetActive(false);
        simulateText.SetActive(false);
        saveButton.SetActive(false);
        reSimulateButton.SetActive(false);
        pathFinding.Clear();
    }

    public void Save()
    {
        LevelInfo newLevel = new LevelInfo();
        newLevel.levelID = levelInfo.levels.Count;
        newLevel.levelRow = row;
        newLevel.levelColumn = column;
        List<LevelConnectionInfo> connectionInfos = new List<LevelConnectionInfo>();
        for (int i = 0; i < pathFinding.activeConnectionCells.Count; i++)
        {
            Vector2Int start = new Vector2Int(pathFinding.activeConnectionCells[i].startNode.x,
                pathFinding.activeConnectionCells[i].startNode.y);
            Vector2Int end = new Vector2Int(pathFinding.activeConnectionCells[i].endNode.x,pathFinding.activeConnectionCells[i].endNode.y);
            connectionInfos.Add(new LevelConnectionInfo(start,end,pathFinding.activeConnectionCells[i].color));
        }

        newLevel.levelConnectionInfos = connectionInfos;
      levelInfo.levels.Add(newLevel);
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}

[System.Serializable]
public class LevelGeneratorReference
{
    public LevelGeneratorCell startNode, endNode;

    public LevelGeneratorReference(LevelGeneratorCell start, LevelGeneratorCell end)
    {
        startNode = start;
        endNode = end;
    }
}

[System.Serializable]
public class Grid
{
    public int row;
    public int column;
    public int minDistancePath;
    public int connectCount;
    public LevelGeneratorCell[,] cells;

    public Grid(int row, int column, int minDistancePath, int connectCount, LevelGeneratorCell[,] cells)
    {
        this.row = row;
        this.column = column;
        this.minDistancePath = minDistancePath;
        this.connectCount = connectCount;
        this.cells = cells;
    }
}