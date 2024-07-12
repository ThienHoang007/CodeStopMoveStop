using System;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    private static Observer instance = null;
    public static Observer Instance => instance;

    private Dictionary<string, List<Action>> listAction = new Dictionary<string, List<Action>>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (this.GetInstanceID() != instance.GetInstanceID()) Destroy(this);
    }
    public void SupEvent(string key, Action function)
    {
        if (listAction.ContainsKey(key)) listAction[key].Add(function);
        else
        {
            if (listAction.TryAdd(key, new List<Action>() { function })) return;
            Debug.LogError($"Can Not Add {key}");
        }
    }
    public void Notify(string key)
    {
        if (listAction.ContainsKey(key))
        {
            foreach (Action function in listAction[key]) function?.Invoke();
        }
        else Debug.Log("Don't have key " + key);
    }
    public void RemoveEvent(string key)
    {
        if (listAction.ContainsKey(key)) listAction.Remove(key);
        else Debug.LogError("Don't have key " + key);
    }
}
