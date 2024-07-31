using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorCell : Node
{
    public SpriteRenderer nodeSprite;
    public LevelData.ConnectColorReference connectColorReference;
    public float tiles_cost;
    public LevelGeneratorCell parent;


    public void Init(int x, int y, bool isAvailable, ConnectColor color)
    {
        SetNode(x, y, isAvailable);
    }

    public void Connect(ConnectColor color)
    {
        isAvailable = false;
        if (color != ConnectColor.None)
        {
            LevelData.ConnectColorReference reference = LevelGenerator.Instance.levelInfo.GetColorReference(color);
            SetConnectConnect(color, reference);
        }
    }

    public void SetConnectConnect(ConnectColor _type, LevelData.ConnectColorReference reference)
    {
        reference = LevelGenerator.Instance.levelInfo.GetColorReference(_type);
        connectColor = _type;
        connectColorReference = reference;
        nodeSprite.color = connectColorReference.connectColor;
        nodeSprite.gameObject.SetActive(true);
    }

    public void ResetCell()
    {
        isAvailable = true;
        nodeSprite.gameObject.SetActive(false);
        connectColor = ConnectColor.None;
        hCost = 0;
        gCost = 0;
        parent = null;
    }
}