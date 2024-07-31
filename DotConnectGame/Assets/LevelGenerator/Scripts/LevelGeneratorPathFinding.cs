using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGeneratorPathFinding : MonoBehaviour
{
    private List<LevelGeneratorCell> pathCells = new List<LevelGeneratorCell>();
    public List<LevelGeneratorConnectionCell> activeConnectionCells = new List<LevelGeneratorConnectionCell>();
    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private LineRenderer currentLine;
    public List<LevelGeneratorCell> PathCells => pathCells;

    public bool PathFinding(LevelGeneratorCell startNode, LevelGeneratorCell endNode,Grid grid)
    {
        pathCells = new List<LevelGeneratorCell>();
        //finding path
        pathCells = Pathfinding.FindPath(startNode, endNode,grid);
        //not finding
        if (pathCells.Count < 1) return false;
        
        //create new line renderer
        if (currentLine == null) currentLine = Instantiate(linePrefab); //line renderer for nodes

      
        //add nodes to the level
        activeConnectionCells.Add(new LevelGeneratorConnectionCell(currentLine, startNode, endNode,
            startNode.connectColor));
        
        currentLine = null;
        
        return true;
    }
    public IEnumerator DrawLineRenderer(LevelGeneratorCell start,LevelGeneratorCell end)
    {
        LineRenderer current = activeConnectionCells[^1].connectedLine;

        //set color of the line renderer
        current.material.color = LevelGenerator.Instance.levelInfo.GetColorReference(start.connectColor)
            .connectColor;;
        activeConnectionCells[^1].color = start.connectColor;
      //  current.positionCount = pathCells.Count;
        //current.SetPositions();
        for (int i = 0; i < pathCells.Count; i++)
        {
            if (pathCells[i] == start || pathCells[i] == end)
                pathCells[i].nodeSprite.gameObject.SetActive(true);
            pathCells[i].isAvailable = false;
            current.positionCount = i + 1;
            current.SetPosition(i, pathCells[i].transform.position);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    /// <summary>
    /// System clear
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < activeConnectionCells.Count; i++)
        {
            Destroy(activeConnectionCells[i].connectedLine?.gameObject);
        }

        activeConnectionCells = new List<LevelGeneratorConnectionCell>();
        for (int i = 0; i < LevelGenerator.Instance.grid.cells.GetLength(0); i++)
        {
            for (int j = 0; j < LevelGenerator.Instance.grid.cells.GetLength(1); j++)
            {
                LevelGenerator.Instance.grid.cells[j, i].ResetCell();
            }
        }
    }

    /// <summary>
    /// clear only available cells
    /// </summary>
    public void ClearOnlyAvailable()
    {
        for (int i = 0; i < LevelGenerator.Instance.grid.cells.GetLength(0); i++)
        {
            for (int j = 0; j < LevelGenerator.Instance.grid.cells.GetLength(1); j++)
            {
                if (LevelGenerator.Instance.grid.cells[j, i].isAvailable)
                    LevelGenerator.Instance.grid.cells[j, i].ResetCell();
            }
        }
    }
}

[System.Serializable]
public class LevelGeneratorConnectionCell
{
    public LineRenderer connectedLine;
    public LevelGeneratorCell startNode;
    public LevelGeneratorCell endNode;
    public ConnectColor color;
    
    public LevelGeneratorConnectionCell(LineRenderer line, LevelGeneratorCell start, LevelGeneratorCell end,
        ConnectColor col)
    {
        connectedLine = line;
        startNode = start;
        endNode = end;
        color = col;
    }
}

[System.Serializable]
public class ConnectionCell
{
    public List<LevelGeneratorCell> cells = new List<LevelGeneratorCell>();
    public LevelGeneratorCell startCell, endCell;

    public ConnectionCell(List<LevelGeneratorCell> _cells, LevelGeneratorCell start, LevelGeneratorCell endCell)
    {
        cells = _cells;
        startCell = start;
        this.endCell = endCell;
    }
}