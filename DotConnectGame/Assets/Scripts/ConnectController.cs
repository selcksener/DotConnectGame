using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;

public class ConnectController : MonoBehaviour
{
    [SerializeField] private GridManager grid;
    [SerializeField] private List<Cell> connectionCells = new List<Cell>();
    [SerializeField] private Cell selectedCell;
    [SerializeField] private Cell currentCell;
   
    [SerializeField] private List<SolveConnection> solved = new List<SolveConnection>();
     public LineRenderer lineRenderer;
    private void Awake()
    {
        EventBus.RegisterEvent<GameStatusType>(EventName.GameStatusEvent, EventListener);
    }

    private void OnDestroy()
    {
        EventBus.UnregisterEvent<GameStatusType>(EventName.GameStatusEvent, EventListener);
    }

    private void EventListener(GameStatusType obj)
    {
        switch (obj)
        {
            case GameStatusType.PlayLevel: //resetting
                connectionCells = new List<Cell>();
                selectedCell = null;
                currentCell = null;
                solved = new List<SolveConnection>();
                break;
        }
    }
    
    /// <summary>
    /// when clicked a cell,
    /// updating line renderer material and
    /// adding the list
    /// </summary>
    /// <param name="mousePosition"></param>
    public void SelectedCell(Vector3 mousePosition)
    {
        Cell _cell = GetCellFromMouseOver(mousePosition);
        if (_cell.connectColorReference.colorType != ConnectColor.None && _cell.isAvailable)
        {
            selectedCell = _cell;
            connectionCells.Add(_cell);
            lineRenderer.material.color =
                GameManager.Instance.levelInfo.GetColorReference(_cell.connectColorReference.colorType).connectColor;
        }
    }

    /// <summary>
    /// dragging mouse
    /// new cell is added to the list while dragging the mouse
    /// </summary>
    /// <param name="position"></param>
    public void DragMouse(Vector2 position)
    {
        position.x = Mathf.RoundToInt(position.x);
        position.y = Mathf.RoundToInt(position.y);
        Cell _cell = grid.cells[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)];
        ConnectCell(_cell);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    public void ConnectCell(Cell cell)
    {
        if (selectedCell == null) return;
        if (cell.isAvailable == false) return;
        if (currentCell != null && currentCell == cell) return;
        currentCell = cell;
        int _x = connectionCells[^1].x;
        int _y = connectionCells[^1].y;
        if (Mathf.Abs(cell.x - _x) > 1 || Mathf.Abs(cell.y - _y) > 1) return;
        if (Mathf.Abs(cell.x - _x) == 1 && Mathf.Abs(cell.y - _y) == 1) return;
        if (connectionCells.Contains(cell) == false)
            connectionCells.Add((cell));
        else
        {
            int getIndex = connectionCells.IndexOf(cell);
            connectionCells.RemoveRange(getIndex + 1, connectionCells.Count - getIndex - 1);
        }
        SetLineRendererPosition();
    }

    /// <summary>
    /// after mouse dragging is ended, the level is controlled
    /// </summary>
    /// <param name="isMouseOver"></param>
    /// <param name="mousePosition"></param>
    public void Deselected(bool isMouseOver, Vector3 mousePosition)
    {
        //if mouse last position isn't over the grid, resetting
        if (isMouseOver == false)
        {
            connectionCells = new List<Cell>();
            SetLineRendererPosition();
        }
        else
        {
            if (selectedCell == null || currentCell == null) return;
            Cell _cell = GetCellFromMouseOver(Input.mousePosition);
            if (IsSolve(_cell))
            {
                SolveConnect();
            }
            else
            {
                connectionCells = new List<Cell>();
                SetLineRendererPosition();
            }
        }

        selectedCell = null;
        currentCell = null;
    }

    private void SolveConnect()
    {
        SolveConnection solve = new SolveConnection
        {
            solvedConnectionCells = connectionCells,
            lineRenderer = lineRenderer
        };
        solved.Add(solve);
        foreach (var cell in connectionCells)
        {
            cell.Solve();
        }
        if (GameManager.Instance.currentLevel.levelConnectionInfos.Count == solved.Count)
        {
            EventBus.TriggerEvent(EventName.GameStatusEvent, GameStatusType.Win);
            return;
        }
        CreateNewReference();
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="_cell"></param>
    /// <returns></returns>
    private bool IsSolve(Cell _cell)
    {
        if (_cell.isAvailable == false || _cell.connectColor != selectedCell.connectColor ||
            _cell == selectedCell)
            return false;
        bool isSolve = true;
        for (int i = 0; i < connectionCells.Count; i++)
        {
            Cell currentCell = connectionCells[i];
            for (int j = 1; j < connectionCells.Count; j++)
            {
                if ((Mathf.Abs(connectionCells[i].x - currentCell.x) > 1 ||
                     Mathf.Abs(connectionCells[i].y - currentCell.y) > 1) ||
                    (Mathf.Abs(connectionCells[i].x - currentCell.x) == 1 &&
                     Mathf.Abs(connectionCells[i].y - currentCell.y) == 1))
                    isSolve = false;
            }
        }

        return isSolve;
    }

    private Cell GetCellFromMouseOver(Vector3 mousePosition)
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int pos = Vector2Int.one;
        pos.x = Mathf.RoundToInt(mousePosition.x);
        pos.y = Mathf.RoundToInt(mousePosition.y);
        Cell _cell = grid.cells[pos.x, pos.y];
        return _cell;
    }
    private void SetLineRendererPosition()
    {
        List<Vector3> cellPosition = new List<Vector3>();
        foreach (var cell in connectionCells)
        {
            cellPosition.Add((cell.transform.position));
        }

        lineRenderer.positionCount = cellPosition.Count;
        lineRenderer.SetPositions(cellPosition.ToArray());
    }
    private void CreateNewReference()
    {
        connectionCells = new List<Cell>();
        lineRenderer = PoolManager.Instance.linePool.Dequeue().LineRenderer;
    }
}

[System.Serializable]
public class SolveConnection
{
    public List<Cell> solvedConnectionCells = new List<Cell>();
    public LineRenderer lineRenderer;
}