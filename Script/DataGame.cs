using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class DataGame
{
    private const string StringData = "DataGamePlay";
    public static void Save(Data Data)
    {
        PlayerPrefs.SetString(StringData, JsonUtility.ToJson(Data));
    }
    public static Data Load()
    {
        return JsonUtility.FromJson<Data>(PlayerPrefs.GetString(StringData));
    }

    public static void SaveDataMaterial(MaterialData MaterialData, int id)
    {
        PlayerPrefs.SetString($"Material{id}", JsonUtility.ToJson(MaterialData));
    }
    public static MaterialData LoadMaterial(int id)
    {
        try
        {
            return JsonUtility.FromJson<MaterialData>(PlayerPrefs.GetString($"Material{id}"));
        }
        catch { return null; }
        
    }
}
public class Data
{
    public string NameUser = "You";
    public int Gold = 5000;
    public int Golds = 5000;
    public int Day = 0;
    public int DayMax = 4;

    public GameObject Player;

    public List<int> idHeader = new List<int>();
    public int isHeadUse = -1;
    public int idHeadTry = -1;

    public List<int> isWeapon = new List<int>();
    public int idWeaponUse = -1;
    public int idWeaponSkin = -1;

    public List<int> idPlants = new List<int>();
    public int idPlantUse = -1;
    public int idPlantTry = -1;

    public List<int> idKhiens = new List<int>();
    public int idKhienUse = -1;
    public int idKhienTry = -1;

    public List<int> idSkins = new List<int>();
    public int idSkinsUse = -1;
    public int idSkinTry = -1;

    public float radiusAttach;
    public float speedMove;

    public int indexRadiusAttachMapZombie = 0;
    public int indexSpeedMoveMapZombie = 0;
    public int indexQuanlityWeaponMapZombie = 1;
    public int indexTimeAliveMapZombie = 0;
}
public class MaterialData
{
    public int[] idColor = null;
    public int indexColor = -1;
}
