using System.Collections.Generic;
using UnityEngine;

public class CellPool : ObjectPool<Cell>
{
   
    protected override void EnqueueSettings(Cell cell)
    {
        cell.ResetCell();
        cell.gameObject.SetActive(false);
        
    }

    protected override void DequeueSettings(Cell cell)
    {
        cell.gameObject.SetActive(true);
    }

    public override void ResetPool()
    {
        Cell poolObject = null;
        while (activeQueue.Count != 0)
        {
            poolObject = activeQueue.Dequeue();
            queue.Enqueue(poolObject);
            EnqueueSettings(poolObject);
        }
    }
}
