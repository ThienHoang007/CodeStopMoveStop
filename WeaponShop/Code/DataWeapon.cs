using System.Collections.Generic;
using UnityEngine;

public class DataWeapon : MonoBehaviour
{
    public List<Material> materials = new List<Material>();
    public Material[] MaterialColors;
    public MeshRenderer Reander;
    public GameObject WeaponMaterial;
    public GameObject WeaponRender;
    public GameObject Weapon;
    public int id;
    public float Price;
    public string ContentWeapon;
    public string NameWeapon;
}
