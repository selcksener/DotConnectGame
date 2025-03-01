using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance {  get { return instance; } }

    protected virtual void Awake()
    {
        if (instance == null) instance = this as T;
        else Destroy(this);
    }
}