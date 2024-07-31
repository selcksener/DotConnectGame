using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DotConnect/LevelData", fileName = "LevelData")]
public class LevelData : ScriptableObject
{
    public List<LevelInfo> levels = new List<LevelInfo>();
    public List<ConnectColorReference> connectColorReference = new List<ConnectColorReference>();

    [System.Serializable]
    public struct ConnectColorReference
    {
        public ConnectColor colorType;
        public Color connectColor;
    }

    public ConnectColorReference GetColorReference(ConnectColor _type)
    {
        ConnectColorReference reference = new ConnectColorReference();
        foreach (var item in connectColorReference)
        {
            if (item.colorType == _type)
            {
                reference = item;
                break;
            }
        }

        return reference;
    }
}

[System.Serializable]
public class LevelInfo
{
    public int levelID;
    public int levelRow;
    public int levelColumn;
    public List<LevelConnectionInfo> levelConnectionInfos = new List<LevelConnectionInfo>();

    public LevelInfo(int levelID, int levelRow, int levelColumn, List<LevelConnectionInfo> levels)
    {
        this.levelID = levelID;
        this.levelRow = levelRow;
        this.levelColumn = levelColumn;
        this.levelConnectionInfos = levels;
    }

    public LevelInfo()
    {
        
    }
}

[System.Serializable]
public class LevelConnectionInfo
{
    
    public Vector2Int levelStartIndex;
    public Vector2Int levelEndIndex;
    public ConnectColor levelColor;

    public LevelConnectionInfo(Vector2Int start, Vector2Int end, ConnectColor color)
    {
        levelStartIndex = start;
        levelEndIndex = end;
        levelColor = color;
    }
}

public enum ConnectColor
{
    None,
    Red,
    Yellow,
    Green,
    Blue,
    Purple
}