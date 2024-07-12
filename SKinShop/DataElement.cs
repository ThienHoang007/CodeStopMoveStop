using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataElement : UnityEngine.MonoBehaviour
{
    public int id;
    public int Price;

    [Space(10)]
    public List<GameObject> Elements;
    public Material Plant;
    public Material Skin;
    public bool isSet = false;

    [Space(10)]
    public string introduce;
    public int indexAdd;
    public int idIndex;
}
