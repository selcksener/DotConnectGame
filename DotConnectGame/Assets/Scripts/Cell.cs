using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;

public class Cell : Node,IPoolableObject<Cell>
{
   public SpriteRenderer nodeSprite;
   public LevelData.ConnectColorReference connectColorReference;
   public void Init(int x, int y, bool isAvailable)
   {
      SetNode(x, y, isAvailable);
   }
   public void SetConnectConnect(ConnectColor _type,LevelData.ConnectColorReference reference)
   {
      //node.isAvailable = false;
      connectColor = _type;
      connectColorReference = reference;
      nodeSprite.color = connectColorReference.connectColor;
      nodeSprite.gameObject.SetActive(true);
   }
   
   public void Solve() => isAvailable = false;

   public void ResetCell()
   {
      ResetNode();
      nodeSprite.gameObject.SetActive(false);
   }
   public IObjectPool<Cell> PoolParent { get; set; }
}
