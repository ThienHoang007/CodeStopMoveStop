using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponShopController : MonoBehaviour
{
    [Header("------------------------Weapons-------------------------")]
    [SerializeField] private List<GameObject> Weapons = new List<GameObject>();
    [SerializeField] private List<GameObject> WeaponRender = new List<GameObject>();
    public PlayerController Player;
    private DataWeapon dataWeapon;
    private Data Data;

    [Space(10)]
    [Header("------------------------Text--------------------------")]
    public Text NameWeapon;
    public Text Look;
    public Text ContentWeapon;
    public Text PriceWeapon;

    [Space(10)]
    [Header("------------------------GameObject---------------------")]
    public GameObject Buy;
    public GameObject Use;
    public GameObject Table;
    public GameObject TableColor;
    public GameObject TableChosseMaterial;

    [SerializeField] private Material materialDefault;
    private Material[] materials;
    private Material[] ColorWeapon;
    private int index = 0;
    private int NumberWeapom = 0;
    private void OnEnable()
    {
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
    }
    // khoi tao weapon shop
    public void Start_ShopWeapon(ref Data Data)
    { 
        if (Data.idWeaponUse == -1)
        {
            dataWeapon = Weapons[0].GetComponent<DataWeapon>();
            Data.idWeaponUse = dataWeapon.id;
            Data.isWeapon.Add(dataWeapon.id);
            DataGame.Save(Data);
        }
        for (int i = 0; i < Weapons.Count; i++)
        {
            if (Weapons[i].GetComponent<DataWeapon>().id == Data.idWeaponUse)
            {
                dataWeapon = Weapons[i].GetComponent<DataWeapon>();
                dataWeapon.WeaponRender.SetActive(true);
                Player.bullet = dataWeapon.Weapon;
            }
            else Weapons[i].GetComponent<DataWeapon>().WeaponRender.SetActive(false);
        }

        MaterialData DataMaterial = DataGame.LoadMaterial(dataWeapon.id);
        if (DataMaterial != null)
        {
            Debug.Log(DataMaterial.indexColor + "Index Color");
            OnSetColorWeaponGamePlay(DataMaterial.indexColor, DataMaterial);
        }
        else Debug.Log("Don't Have Data Material");
        this.Data = Data;
        EventBtnNext();
    }
    // event next
    public void EventBtnNext()
    {
        if (NumberWeapom == Weapons.Count - 1) NumberWeapom = 0;
        else NumberWeapom++;
        Weapons[NumberWeapom].SetActive(true);
        try
        {
            Weapons[NumberWeapom - 1].SetActive(false);
            Weapons[NumberWeapom - 1].GetComponent<DataWeapon>().WeaponMaterial.SetActive(false);
        }
        catch
        {
            Weapons[Weapons.Count - 1].SetActive(false);
            Weapons[Weapons.Count - 1].GetComponent<DataWeapon>().WeaponMaterial.SetActive(false);
        }
        dataWeapon = Weapons[NumberWeapom].GetComponent<DataWeapon>();
        UpdateDataWeaponForUI(dataWeapon);
        this.Table.SetActive(false);
        InstanceButton(false);

        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
    }
    // event sau
    public void EventBtnAgain()
    {
        if(NumberWeapom == 0) NumberWeapom = Weapons.Count - 1;
        else NumberWeapom--;
        Weapons[NumberWeapom].SetActive(true);
        try
        {
            Weapons[NumberWeapom + 1].SetActive(false);
            Weapons[NumberWeapom + 1].GetComponent<DataWeapon>().WeaponMaterial.SetActive(false);
        }
        catch
        {
            Weapons[0].SetActive(false);
            Weapons[0].GetComponent<DataWeapon>().WeaponMaterial.SetActive(false);
        }
        dataWeapon = Weapons[NumberWeapom].GetComponent<DataWeapon>();
        UpdateDataWeaponForUI(dataWeapon);
        this.Table.SetActive(false);
        InstanceButton(false);

        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
    }
    // cap nhat sao dien weapon shop
    private void UpdateDataWeaponForUI(DataWeapon DataWeapon)
    {
        this.NameWeapon.text = DataWeapon.NameWeapon;
        this.ContentWeapon.text = DataWeapon.ContentWeapon;
        this.PriceWeapon.text = DataWeapon.Price.ToString();
        foreach(int id in Data.isWeapon)
        {
            if(id == DataWeapon.id)
            {
                this.Look.text = "Uplock";
                Buy.SetActive(false);
                Use.SetActive(true);
                if(dataWeapon.WeaponMaterial != null) dataWeapon.WeaponMaterial.SetActive(true);
                return;
            }
        }
        this.Look.text = "Lock";
        Buy.SetActive(true);
        Use.SetActive(false);
        if(dataWeapon.WeaponMaterial != null) dataWeapon.WeaponMaterial.SetActive(false);
    }
    public void OnBuy() //mua
    {
        if (true)
        {
            Data.isWeapon.Add(dataWeapon.id);
            Data.idWeaponUse = dataWeapon.id;
            DataGame.Save(Data);
            OnChangeColorCandy(0);
            Player.bullet = dataWeapon.Weapon;
            ChangeColorWeeaponGamePlay(ref dataWeapon.Weapon, ColorWeapon);
            OnSetWeaponPlay();
            dataWeapon.WeaponRender.SetActive(true);
            UpdateDataWeaponForUI(dataWeapon);
            AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
        }
    }
    public void OnUse()
    {
        OnSetWeaponPlay();
        if(dataWeapon != null) dataWeapon.WeaponRender.SetActive(true);
        Data.idWeaponUse = dataWeapon.id;
        Player.bullet = dataWeapon.Weapon;
        ChangeColorWeeaponGamePlay(ref dataWeapon.Weapon, ColorWeapon);
        //dataWeapon.Weapon.GetComponent<Renderer>().materials = this.materials;
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
        DataGame.Save(Data);
    }
    private void OnSetWeaponPlay()
    {
        for(int i = 0; i < Weapons.Count; i++)
        {
            WeaponRender[i].SetActive(false);
        }
    }
    public void OnChangeColorCandy(int id)
    {
        materials = new Material[dataWeapon.Reander.materials.Length];
        if(id >= dataWeapon.materials.Count)
        {
            Debug.LogError("IdKhongHopLe/weaponController");
            return;
        }
        for (int i = 0; i < dataWeapon.Reander.materials.Length; i++) materials[i] = dataWeapon.materials[id];
        dataWeapon.Reander.materials = this.materials;
        dataWeapon.WeaponRender.GetComponent<Renderer>().materials = this.materials;
        ColorWeapon = this.materials;
        this.Table.SetActive(false);
        this.Use.SetActive(true);
        InstanceButton(false);
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);

        MaterialData DataMaterial = DataGame.LoadMaterial(dataWeapon.id);
        if (DataMaterial == null) DataMaterial = new MaterialData();

        DataMaterial.indexColor = id;
        DataGame.SaveDataMaterial(DataMaterial, dataWeapon.id);
    }
    private void OnSetColorWeaponGamePlay(int id, MaterialData DataMaterial)
    {
        if(DataMaterial.indexColor >= 0)
        {
            materials = new Material[dataWeapon.Reander.materials.Length];
            if (id >= dataWeapon.materials.Count)
            {
                Debug.LogError("IdKhongHopLe/weaponController");
                return;
            }
            for (int i = 0; i < dataWeapon.Reander.materials.Length; i++) materials[i] = dataWeapon.materials[id];
            dataWeapon.Reander.materials = this.materials;
            dataWeapon.WeaponRender.GetComponent<Renderer>().materials = this.materials;
        }
        else if(DataMaterial.indexColor == -2)
        {
            materials = new Material[dataWeapon.Reander.materials.Length];
            for (int i = 0; i < dataWeapon.Reander.materials.Length; i++)
            {
                materials[i] = materialDefault;
            }
            dataWeapon.Reander.materials = this.materials;

            if (DataMaterial.idColor != null)
            {
                for (int j = 0; j < DataMaterial.idColor.Length; j++)
                {
                    OnGetMaterialWeapon(j);
                    ChangeColor(DataMaterial.idColor[index]);
                }
            }
            else Debug.Log("Don't Have Data");
        }
    }
    private void ChangeColor(int id)
    {
        Color color;
        Material[] x = dataWeapon.Reander.materials;
        for (int i = 0; i < TableColor.transform.childCount; i++)
        {
            if (id == i)
            {
                color = TableColor.transform.GetChild(i).GetComponent<Image>().color;
                x[index].color = color;
            }
        }
        dataWeapon.WeaponRender.GetComponent<MeshRenderer>().materials = x;
    }
    public void OnActiveTableCustomColor()
    {
        Buy.SetActive(false);
        Use.SetActive(false);
        Table.SetActive(true);
        InstanceButton(true);
    }
    private void InstanceButton(bool active)
    {
        if (active)
        {
            materials = new Material[dataWeapon.Reander.materials.Length];
            for (int i = 0; i < dataWeapon.Reander.materials.Length; i++)
            {
                materials[i] = materialDefault;
            }
            dataWeapon.Reander.materials = this.materials;

            MaterialData DataMaterial = DataGame.LoadMaterial(dataWeapon.id);
            if (DataMaterial != null && DataMaterial.idColor != null)
            {
                for(int j = 0; j < DataMaterial.idColor.Length; j++)
                {
                    OnGetMaterialWeapon(j);
                    OnChangeColorWeapon(DataMaterial.idColor[index]);
                }
            }
            else Debug.Log("Don't Have Data");
        }
        for (int i = 0; i < dataWeapon.Reander.materials.Length; i++)
        {
            TableChosseMaterial.transform.GetChild(i).gameObject.SetActive(active);                               
        }
    }
    public void OnChangeColorWeapon(int idColor)
    {
        MaterialData DataMaterial = DataGame.LoadMaterial(dataWeapon.id);
        if(DataMaterial == null) DataMaterial = new MaterialData();
        
        Color color;
        Material[] x = dataWeapon.Reander.materials;
        for (int i = 0; i < TableColor.transform.childCount; i++)
        {
            if (idColor == i)
            {
                color = TableColor.transform.GetChild(i).GetComponent<Image>().color;
                x[index].color = color;
                TableChosseMaterial.transform.GetChild(index).GetComponent<Image>().color = color;

                if (DataMaterial.idColor == null)
                {
                    DataMaterial.idColor = new int[dataWeapon.Reander.materials.Length];
                    Debug.Log("Khoi Tao DataMaterial");
                }
                
                try { DataMaterial.idColor[index] = idColor; }
                catch
                {
                    DataMaterial.idColor = new int[dataWeapon.Reander.materials.Length];
                    DataMaterial.idColor[index] = idColor;
                }
                DataMaterial.indexColor = -2;
                DataGame.SaveDataMaterial(DataMaterial, dataWeapon.id);
            }
        }
        ColorWeapon = x;
        dataWeapon.WeaponRender.GetComponent<MeshRenderer>().materials = x;
    }
    private void ChangeColorWeeaponGamePlay(ref GameObject Weapon, Material[] Materials)
    {
        Weapon.GetComponent<MeshRenderer>().materials = Materials;
    }
    public void OnGetMaterialWeapon(int index) => this.index = index;

}
