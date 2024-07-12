using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Net.NetworkInformation;
public class SkinShopController : MonoBehaviour
{
    [Header("-----------------------ref----------------------")]
    public GameObject btnBuy;
    public GameObject btnSelect, btnUnequip,btnQC ; // Button Event
    public Text priceElement, introduc; // Text update price
    private int IdData, IdDataLast; // Id Event
    private Data Data; // Data Skin Shop
    private PlayerController playerController;
    [HideInInspector] public DataElement DataElement;

    [Space(7)]
    [Header("-------------------------------------------------")]
    [SerializeField] private List<GameObject> Skins = new List<GameObject>();
    [SerializeField] private List<RawImage> ColorImages = new List<RawImage>();
    private List<List<GameObject>> SkinList;

    [Space(7)]
    [Header("----------------------Header----------------------")]
    [SerializeField] private List<GameObject> Header = new List<GameObject>();

    [Space(7)]
    [Header("----------------------Plant-----------------------")]
    [SerializeField] private List<GameObject> Plants = new List<GameObject>();

    [Space(7)]
    [Header("----------------------Khien------------------------")]
    [SerializeField] private List<GameObject> Khiens = new List<GameObject>();

    [Space(7)]
    [Header("---------------------SetSkin-----------------------")]
    [SerializeField] private List<GameObject> SetSkins = new List<GameObject>();

    [Space(7)]
    [Header("----------------------ref skin---------------------")]
    public SkinnedMeshRenderer skinMesh_SkinPlant;
    public SkinnedMeshRenderer skinMesh_SkinPlayer;
    private Material materialPlantDefault;
    private Material materialSkinDefault;
    private void OnEnable()
    {
        OnActiveContent(IdData);
        OnChangeColorImage(IdData);
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
    }
    private void Start()
    {
        // Set Default Shop Skin
        OnActiveContent(IdData);
        OnChangeColorImage(IdData);

        this.playerController = GamePlay_SceneManager.Instance.playerController;
    }
    public void Start_ShopSkin(ref Data Data)
    {
        if(Data == null)
        {
            Debug.LogError("Don't Have Data");
            return;
        }
        this.Data = Data;
        materialPlantDefault = skinMesh_SkinPlant.sharedMaterial;
        materialSkinDefault = skinMesh_SkinPlayer.sharedMaterial;

        IdData = 0;
        SkinList = new List<List<GameObject>>() { Header, Plants, Khiens, SetSkins };

        OnDeleate(ref Data.idHeader, ref Data.idHeadTry, ref Data.isHeadUse);
        OnDeleate(ref Data.idPlants, ref Data.idPlantTry, ref Data.idPlantUse);
        OnDeleate(ref Data.idKhiens, ref Data.idKhienTry, ref Data.idKhienUse);
        OnDeleate(ref Data.idSkins, ref Data.idSkinTry, ref Data.idSkinsUse);
        DataGame.Save(Data);

        //Khoi Tao Skin
        SetSkinUsingForPlayer(Data.isHeadUse, Header);
        SetPlantUsingForPlayer(Data.idPlantUse);
        SetSkinUsingForPlayer(Data.idKhienUse, Khiens);
        OnChangeSetSkinPlay(Data.idSkinsUse);
    }
    // Button Mua Skin
    public void Button_OnBuy()
    {
        try
        {
            if (true)
            {
                GetListData(IdData).Add(DataElement.id);
                ChangeidUseItem(IdData, DataElement.id);
                if (IdData != 3) Data.isHeadUse = -1;
                DataGame.Save(Data);
                btnBuy.SetActive(false);
                btnSelect.SetActive(true);
                btnQC.SetActive(false);
                btnUnequip.SetActive(false);
            }
        }
        catch
        {
            Debug.Log("Buy Not Done!");
        }
    }
    public void Button_OnSelect()
    {
        btnSelect.SetActive(false);
        btnUnequip.SetActive(true);
        ChangeidUseItem(IdData, DataElement.id);
        if (IdData != 3) Data.idSkinsUse = -1;
        DataGame.Save(Data);
    }
    public void Button_OnUnequip()
    {
        btnSelect.SetActive(true);
        btnUnequip.SetActive(false);

        ChangeidUseItem(IdData, -1);
        SetSkinForPlayer();                                                                                                        
    }
    public void Button_SetActionButton(int id) // set button se nao se duoc active
    {
        foreach(int idHead in GetListData(IdData))
        {
            if(id == idHead)
            {
                btnBuy.SetActive(false);
                btnQC.SetActive(false);
                if(!(id == GetIntData(IdData)))
                {
                    btnUnequip.SetActive(false);
                    btnSelect.SetActive(true);
                }
                else
                {
                    btnUnequip.SetActive(true);
                    btnSelect.SetActive(false);
                }
                introduc.text = this.DataElement.introduce;
                SetIndexAddForPlayer(DataElement.indexAdd, DataElement.idIndex);
                return;
            }
        }
        btnBuy.SetActive(true);
        btnQC.SetActive(true);
        btnSelect.SetActive(false);
        btnUnequip.SetActive(false);
        if (DataElement != null)
        {
            priceElement.text = DataElement.Price.ToString();
            introduc.text = this.DataElement.introduce;
        }
        else print("Don't Have DataElement !");
    }
    public void SetSkinForPlayer()
    {
        try
        {
            OnDextroySetSkinWhenChange();
            SetSkinUsingForPlayer(Data.isHeadUse, Header);
            SetPlantUsingForPlayer(Data.idPlantUse);
            SetSkinUsingForPlayer(Data.idKhienUse, Khiens);
            OnChangeSetSkinPlay(Data.idSkinsUse);
        }
        catch { Debug.LogError("Khong Khoi Tao Duoc Data"); }
    }
    public void OnChangeColorImage(int idImage)
    {
        for(int i = 0; i < ColorImages.Count; i++)
        {
            if (idImage == i) ColorImages[i].color = Color.white;
            else ColorImages[i].color = Color.black;
        }
    }
    public void OnActiveHeader(int id)
    {
        foreach(GameObject gameObject in SkinList[IdData])
        {
            if (gameObject.GetComponent<DataElement>().id == id)
            {
                gameObject.SetActive(true);
                this.DataElement = gameObject.GetComponent<DataElement>(); 
            }
            else gameObject.SetActive(false);
        }
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
        DataGame.Save(Data);
    }
    public void OnActiveContent(int id)
    {
        for(int i = 0; i < Skins.Count; i++)
        {
            if (i == id) Skins[i].SetActive(true);
            else Skins[i].SetActive(false);
        }
        IdDataLast = IdData;
        IdData = id;
        try
        {
            if(OnDestroySetSkinWhenActiveContent())
            {
                SetSkinUsingForPlayer(Data.isHeadUse, Header);
                SetPlantUsingForPlayer(Data.idPlantUse);
                SetSkinUsingForPlayer(Data.idKhienUse, Khiens);
            }
        }
        catch { };

        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
    }
    private void SetSkinUsingForPlayer(int id, List<GameObject> skins)
    {
        foreach(GameObject gameObject in skins)
        {
            if (gameObject.GetComponent<DataElement>().id == id) gameObject.SetActive(true);
            else gameObject.SetActive(false);
        }
    }
    public void OnSetSkinPlayerStartPlay(Data Data)
    {
        if (Data == null) return;
        OnActivePlant(Data.idPlantUse);
        SetSkinUsingForPlayer(GetIntData(IdDataLast), SkinList[IdDataLast]);
    }
    public void OnActivePlant(int id)
    {
        foreach (GameObject gameObject in Plants)
        {
            if (gameObject.GetComponent<DataElement>().id == id)
            {
                skinMesh_SkinPlant.material = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                this.DataElement = gameObject.GetComponent<DataElement>();
                return;
            }
            skinMesh_SkinPlant.material = materialPlantDefault;
            if (id < 0) DataElement = null;
            AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
            DataGame.Save(Data);
        }
    }
    private void SetPlantUsingForPlayer(int id)
    {
        foreach (GameObject gameObject in Plants)
        {
            if (gameObject.GetComponent<DataElement>().id == id)
            {
                skinMesh_SkinPlant.material = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                return;
            }
            skinMesh_SkinPlant.material = materialPlantDefault;
        }
    }
    private void OnDeactiveSkin(List<GameObject> skins)
    {
        foreach (GameObject Element in skins) Element.SetActive(false);
    }
    public void OnChangeSetSkinPlay(int id)
    {
        if (id < 0)
        {
            Debug.Log("Don't Have Skin");
            return;
        }

        foreach(GameObject gameobjects in SetSkins)
        {
            if(gameobjects.GetComponent<DataElement>().id == id)
            {
                this.DataElement = gameobjects.GetComponent<DataElement>();
                break;
            }
        }
        skinMesh_SkinPlayer.material = this.DataElement.Skin;
        skinMesh_SkinPlant.material = this.DataElement.Plant;

        foreach (GameObject Element in this.DataElement.Elements) Element.SetActive(true);
        for (int i = 0; i < 3; i++) ChangeidUseItem(i, -1);
        //Destroy Skin Khc
        OnDeactiveSkin(Header);
        OnDeactiveSkin(Plants);
        OnDeactiveSkin(Khiens);
    }
    //Set Last (DataElement Not Change)
    public bool OnDestroySetSkinWhenActiveContent()
    {
        if (!DataElement.isSet) return true;

        foreach(int id in Data.idSkins)
        {
            if (this.DataElement.id == id) return false;
        }
        skinMesh_SkinPlayer.material = materialSkinDefault;
        skinMesh_SkinPlant.material = materialPlantDefault;

        foreach (GameObject Element in this.DataElement.Elements) Element.SetActive(false);
        return true;
    }
    // Set first (Dataelemnt CHange)
    public void OnDextroySetSkinWhenChange()
    {
        if (DataElement == null || !DataElement.isSet) return;

        skinMesh_SkinPlayer.material = materialSkinDefault;
        skinMesh_SkinPlant.material = materialPlantDefault;

        foreach (GameObject Element in this.DataElement.Elements) Element.SetActive(false);
    }
    public void OnAds()
    {
        GetListData(IdData).Add(DataElement.id);
        ChangeidUseItem(IdData, DataElement.id); ;
        if (IdData != 3) Data.idSkinsUse = -1;
        OnChangeWeaponTry(IdData, DataElement.id);
        DataGame.Save(Data);

        //ste btn 
        btnUnequip.SetActive(true);
        btnQC.SetActive(false);
        btnBuy.SetActive(false);
        btnSelect.SetActive(false);
        //play sound
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
    }
    private void OnDeleate(ref List<int> ids, ref int id, ref int idUse)
    {
        for(int  i = 0; i < ids.Count; i++)
        {
            if(id == ids[i])
            {
                ids.RemoveAt(i);
                id = -1;
                idUse = -1;
                return;
            }
        }
    }
    private void SetIndexAddForPlayer(int value, int id)
    {
        switch(id)
        {
            case 0:
                playerController.OnRadiusAttach(1);
                break;
            case 1:
                playerController.OnSpeedMove(1);
                break;
            default:
                Debug.Log("id coming soon !");
                break;
        }
    }
    private List<int> GetListData(int id)
    {
        switch(id)
        {
            case 0: return Data.idHeader;
            case 1: return Data.idPlants;
            case 2: return Data.idKhiens;
            case 3: return Data.idSkins;
            default: return null;
        }
    }
    private void ChangeidUseItem(int id, int value)
    {
        switch(id)
        {
            case 0:
                Data.isHeadUse = value;
                break;
            case 1:
                Data.idPlantUse = value;
                break;
            case 2:
                Data.idKhienUse = value;
                break;
            case 3:
                Data.idSkinsUse = value;
                break;
        }
    }
    private void OnChangeWeaponTry(int id, int value)
    {
        switch (id)
        {
            case 0:
                Data.idHeadTry = value;
                break;
            case 1:
                Data.idPlantTry = value;
                break;
            case 2:
                Data.idKhienTry = value;
                break;
            case 3:
                Data.idSkinTry = value;
                break;
        }
    }
    private int GetIntData(int id)
    {
        switch(id)
        {
            case 0: return Data.isHeadUse;
            case 1: return Data.idPlantUse;
            case 2: return Data.idKhienUse;
            case 3: return Data.idSkinsUse;
            default: return -1;
        }
    }
}
