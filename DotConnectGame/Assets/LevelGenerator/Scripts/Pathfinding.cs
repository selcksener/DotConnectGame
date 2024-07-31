using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public static List<LevelGeneratorCell> FindPath(LevelGeneratorCell startPos, LevelGeneratorCell targetPos, Grid grid)
    {
        List<LevelGeneratorCell> nodes_path = new List<LevelGeneratorCell>();

        LevelGeneratorCell startNode = grid.cells[startPos.x, startPos.y];
        LevelGeneratorCell endNode = grid.cells[targetPos.x, targetPos.y];

        List<LevelGeneratorCell> openList = new List<LevelGeneratorCell>();
        HashSet<LevelGeneratorCell> closedList = new HashSet<LevelGeneratorCell>();

        openList.Add(startNode);
    
        while (openList.Count > 0)
        {
            LevelGeneratorCell currentNode = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                //cost is lower than current
                if (openList[i].fCost < currentNode.fCost ||
                    (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                    
                }
            }

            openList.Remove(currentNode); //not to check again
            closedList.Add(currentNode); //

            // the pathfinding ends when the current node reaches the end node
            if (currentNode == endNode)
            {
                List<LevelGeneratorCell> path = new List<LevelGeneratorCell>();
                LevelGeneratorCell node = endNode;
                while (node != startNode)
                {
                    path.Add(node);
                    node = node.parent;
                }
                nodes_path = path;
                nodes_path.Reverse();
                break;
            }
            
            List<LevelGeneratorCell> neighCells = GetNeighbours(currentNode,grid);
            foreach (LevelGeneratorCell neighbour in neighCells)
            {
                if (closedList.Contains(neighbour) || neighbour.isAvailable == false) continue;
                //Calculate movement cost to neighbour
                int movementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (movementCostToNeighbour < neighbour.gCost || openList.Contains(neighbour) == false)
                {
                    //Set Cost
                    neighbour.gCost = movementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = currentNode;
                  
                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }
        
        //if find path , add start node to the list
        if (nodes_path.Count > 0)
            nodes_path.Insert(0, startPos);
        return nodes_path;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private static int GetDistance(LevelGeneratorCell nodeA, LevelGeneratorCell nodeB)
    {
        //pathfinding algorithm - A* algorithm
        int distX = Mathf.Abs(nodeA.x - nodeB.x);
        int distY = Mathf.Abs(nodeA.y - nodeB.y);
        int d = distX > distY ? 14 * distY + 10 * (distX - distY) : 14 * distX + 10 * (distY - distX);
        return d;
    }


    /// <summary>
    /// neighbors of the cell
    /// </summary>
    /// <param name="_node">cell to search neighbors</param>
    /// <returns></returns>
    public static List<LevelGeneratorCell> GetNeighbours(LevelGeneratorCell _node,Grid grid)
    {
        List<LevelGeneratorCell> neighbours = new List<LevelGeneratorCell>();
        neighbours = new List<LevelGeneratorCell>();
        for (int i = -1; i <= 1; i++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (i == 0 && y == 0) continue; // not search for itself
                if ((Mathf.Abs(i) + Mathf.Abs(y)) % 2 == 0) continue; //only searches left-right-up-down neighbour
                int checkX = _node.x + i;
                int checkY = _node.y + y;
                if (checkX >= 0 && checkX < grid.row && checkY >= 0 &&
                    checkY < grid.column)
                {
                    neighbours.Add(grid.cells[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
}