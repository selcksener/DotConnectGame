using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;

public class Line : MonoBehaviour,IPoolableObject<Line>
{
    // Start is called before the first frame upd
    [SerializeField] private LineRenderer lineRenderer;
    public LineRenderer LineRenderer => lineRenderer;
    public IObjectPool<Line> PoolParent { get; set; }
}
