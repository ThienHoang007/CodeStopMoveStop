using System.Data.Common;
using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;
    private void Awake()
    {
        Debug.LogError("Singleton");
        instance = GetComponent<T>();
    }
}
