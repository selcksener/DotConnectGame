using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private int _x;
    private int _y;
    private bool _isAvailable;
    private ConnectColor _connectColor;
    private int _gCost;
    private int _hCost;
    private int _fCost => _gCost + _hCost;

    public int x => _x;
    public int y => _y;

    public bool isAvailable
    {
        get => _isAvailable;
        set => _isAvailable = value;
    }

    public ConnectColor connectColor
    {
        get => _connectColor;
        set => _connectColor = value;
    }

    public int gCost
    {
        get { return _gCost; }
        set { _gCost = value; }
    }

    public int hCost  {
        get { return _hCost; }
        set { _hCost = value; }
    }
    public int fCost => _fCost;

    public void SetNode(int x, int y, bool isAvailable)
    {
        _x = x;
        _y = y;
        _isAvailable = isAvailable;
    }

    public void ResetNode()
    {
        _x = 0;
        _y = 0;
        _isAvailable = true;
    }
}