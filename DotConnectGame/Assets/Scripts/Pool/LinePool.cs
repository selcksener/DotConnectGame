using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePool : ObjectPool<Line>
{
    protected override void EnqueueSettings(Line poolObject)
    {
        poolObject.LineRenderer.positionCount = 0;
        poolObject.gameObject.SetActive(false);
    }

    protected override void DequeueSettings(Line poolObject)
    {
        poolObject.LineRenderer.positionCount = 0;
        poolObject.gameObject.SetActive(true);
    }
    public override void ResetPool()
    { 
        Line poolObject = null;
        while (activeQueue.Count != 0)
        {
            poolObject = activeQueue.Dequeue();
            queue.Enqueue(poolObject);
            EnqueueSettings(poolObject);
        }
    }
}
