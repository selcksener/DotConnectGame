using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GridManager grid;
    public ConnectController controller;

    private void Update()
    {
        if(GameManager.Instance.isPlaying == false) return;
        if (Input.GetMouseButtonDown(0))
        {
            //if mouse position is over the grid
            if (IsMouseOverGrid(Input.mousePosition))
            {
                controller.SelectedCell(Input.mousePosition);
            }
        }
        //if dragging
        if (Input.GetMouseButton(0))
        {
            Vector2Int pos = ScreenToWorldPosition(Input.mousePosition);
            if (IsMouseOverGrid(Input.mousePosition))
            {
                controller.DragMouse(pos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            bool mouseOver = IsMouseOverGrid(Input.mousePosition);
            controller.Deselected(mouseOver, Input.mousePosition);
        }
    }

    private RaycastHit2D GetRayCast() =>
        Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

    /// <summary>
    /// The position is over the grid
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    private bool IsMouseOverGrid(Vector3 mousePosition)
    {
        Vector2Int pos = ScreenToWorldPosition(mousePosition);
        //Out of grid
        if (pos.x < 0 || pos.x > grid.row - 1
                      || pos.y < 0 ||
                      pos.y > grid.column - 1)
            return false;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    public Vector2Int ScreenToWorldPosition(Vector3 mousePosition)
    {
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2Int pos = Vector2Int.one;
        pos.x = Mathf.RoundToInt(mousePosition.x);
        pos.y = Mathf.RoundToInt(mousePosition.y);
        return pos;
    }
}