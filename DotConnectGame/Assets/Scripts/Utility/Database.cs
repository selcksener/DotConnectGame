using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database 
{
   public static Database Instance { get; } = new Database();

   public Database()
   {
      
   }

   public int CurrentLevel
   {
      get
      {
         return PlayerPrefs.GetInt("CurrentLevel", 0);
         
      }
      set
      {
         PlayerPrefs.SetInt("CurrentLevel",value);
      }
   }

   public void Save() => PlayerPrefs.Save();
}
