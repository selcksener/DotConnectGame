using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PoolManager : SingletonBehaviour<PoolManager>
{

    public CellPool cellPool;
    public LinePool linePool;

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EventBus.RegisterEvent<GameStatusType>(EventName.GameStatusEvent, GameStateListener);
    }
    
    private void GameStateListener(GameStatusType obj)
    {
        switch (obj)
        {
            case GameStatusType.Win:
                int currentLevel = Database.Instance.CurrentLevel;
                currentLevel = (currentLevel + 1) % (GameManager.Instance.levelInfo.levels.Count);
                Database.Instance.CurrentLevel = currentLevel;
                Database.Instance.Save();
                StartCoroutine(ResetPool());
                break;
            case GameStatusType.Restart:
                StartCoroutine(ResetPool());
                break;
            case GameStatusType.NextLevel:
                break;
        }
    }

    private void OnDestroy()
    {
        EventBus.UnregisterEvent<GameStatusType>(EventName.GameStatusEvent, GameStateListener);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator ResetPool()
    {
        cellPool.ResetPool();
        linePool.ResetPool();
        yield return new WaitForSeconds(0.5f);
        EventBus.TriggerEvent(EventName.GameStatusEvent, GameStatusType.PlayLevel);
    }

}