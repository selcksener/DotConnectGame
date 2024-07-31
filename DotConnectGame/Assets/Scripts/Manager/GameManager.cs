using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
   public GridManager gridManager;
   public LevelData levelInfo;
   public LevelInfo currentLevel;
   public bool isPlaying = false;
   protected override void Awake()
   {
      base.Awake();
      EventBus.RegisterEvent<GameStatusType>(EventName.GameStatusEvent,EventListener);
   }
   private void Start()
   {
      //starting current level
      EventBus.TriggerEvent(EventName.GameStatusEvent, GameStatusType.PlayLevel);
   }

   private void EventListener(GameStatusType _type)
   {
      switch (_type)
      {
         case GameStatusType.PlayLevel:
            currentLevel = levelInfo.levels[Database.Instance.CurrentLevel];
            gridManager.CreateGrid(currentLevel);
            isPlaying = true;
            break;
      }
   }
}


public enum GameStatusType
{
   Menu,
   PlayLevel,
   CreateGrid,
   Win,
   NextLevel,
   Restart
}
