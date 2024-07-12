using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GamePlay_SceneManager;
using static AudioManager;
using System;
using UnityEngine.SceneManagement;

public class ZombieCity_SceneManager : MonoBehaviour
{
    public static ZombieCity_SceneManager Instance;
    [Header("-----------------Popup----------------------")]
    [SerializeField, Tooltip("GameObject Popup")] private List<GameObject> list_Popups = new List<GameObject>();
    [SerializeField, Tooltip("Image Day Zombie Gamw Win")] private List<RawImage> rawImgae_DayZombieGameWin = new List<RawImage>();
    [SerializeField, Tooltip("Image Day ZOmbie Game Over")] private List<RawImage> rawImage_DayZombieGameOver = new List<RawImage>();
    [Tooltip("Text Gold")] public Text txt_Gold;
    [Tooltip("Player Controller")] public PlayerController playerController;
    private Data Data;
    private int Quanlity = 0;
    public Text txt_QuanlityZombieKilledPlayer; // text cap nhat so kuong zombie da bi tieu diey

    [Space(7)]
    [Header("------------------Data Skin-----------------")]
    public SkinShopController skinShopController;
    public WeaponShopController weaponShopController;

    [Space(7)]
    [Header("-------------------Image Skill---------------"), Tooltip("GameObject Data Skill")]
    [SerializeField] private List<GameObject> list_ObjSkill = new List<GameObject>(); // Img Skill
    private int[] arr_NumberImage = new int[2];
    private bool isChange = true; // bool thay doi sprite
    private DataSkillMapZombie dataSkillMapZombie;
    public Image img_Skill;
    public Text txt_NameSkill;
    public Text txt_CodownTime;
    public Text txt_RadiusAttach, txt_SpeedMove, txt_TimeLive, txt_QuanlityWeapon, txt_QuanlityZombie;

    [Space(7)]
    [Header("-----------------------Zombie-----------------")]
    [SerializeField, Tooltip("Puanlity Enemy")] private int int_QuanlityZombir;
    public Text txt_QuanlityEnemy;
    public GameObject obj_Zombie, ZombieManager;
    [SerializeField, Tooltip("Toa Do Instance enemy")] private List<Vector3> list_PosInstanceZombie = new List<Vector3>();

    [Space(7)]
    [Header("-------------------Sound-----------------------")]
    public Button button_Sound;
    public Button button_Vabition;
    private bool isSound = true, isVabition = true;

    private int int_Seconds = 5;
    private int int_Gold = 0;
    private int int_IndexPlusGold = 1;
    private int int_QuanlityZombieKilled = 0, int_IndexLevelUpPlayer = 8, int_IndexPlusLevelUp = 1, int_QuanlityZOmbieHienTai;

    //string--------------------------------------------------------------------
    public const string PLUSGOLD = "PLUSGOLD";
    public const string QUANLITYWASKILLED = "QUANLITYZOMBIEKILLED";
    private void Awake()
    {
        Instance = this;
        // khoi tao data
        Data = DataGame.Load();
        if (Data == null) Data = new Data();
        skinShopController.Start_ShopSkin(ref Data);
        weaponShopController.Start_ShopWeapon(ref Data);

        Time.timeScale = 1f; // kich hoat lai game
        int_QuanlityZOmbieHienTai = ZombieManager.transform.childCount; // lay so luong zombie hien tai
    }
    private void Start()
    {
        arr_NumberImage[0] = UnityEngine.Random.Range(0, list_ObjSkill.Count);
        arr_NumberImage[1] = UnityEngine.Random.Range(0, list_ObjSkill.Count);
        while (arr_NumberImage[0] == arr_NumberImage[1]) arr_NumberImage[1] = UnityEngine.Random.Range(0, list_ObjSkill.Count);
        Button_ChangeSkill(); // thay skill start
        txt_QuanlityEnemy.text = int_QuanlityZombir.ToString(); // cap nhat so luong zombie
        txt_QuanlityZombie.text = int_QuanlityZombir.ToString(); //cap nhat so luong zombie map chinh

        Observer.Instance.SupEvent(PLUSGOLD, Event_PlusGold);
        Observer.Instance.SupEvent(StringData.ZOMBIEDIE, Event_ZombieDie);
        Observer.Instance.SupEvent(UICuren.GameOver.ToString(), Event_Revive);
        Observer.Instance.SupEvent(QUANLITYWASKILLED, Event_QuanlityZombieKilled);
    }
    public void Button_ChangeSkill()
    {
        if(isChange)
        {
            dataSkillMapZombie = list_ObjSkill[arr_NumberImage[0]].GetComponent<DataSkillMapZombie>();
            img_Skill.sprite = dataSkillMapZombie.Sprite;
            txt_NameSkill.text = dataSkillMapZombie.Name;
        }
        else
        {
            dataSkillMapZombie = list_ObjSkill[arr_NumberImage[1]].GetComponent<DataSkillMapZombie>();
            img_Skill.sprite = dataSkillMapZombie.Sprite;
            txt_NameSkill.text = dataSkillMapZombie.Name;
        }
        isChange = !isChange;
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
    }
    private void Event_SetPopup(UICuren Popup)
    {
        for(int i = 0; i < list_Popups.Count; i++)
        {
            if (list_Popups[i].tag == Popup.ToString()) list_Popups[i].SetActive(true);
            else list_Popups[i].SetActive(false);
        }
    }
    public void Button_PlusRadiusAttachPlayer(int price)
    {
        if(Event_Buy(price))
        {
            Data.indexRadiusAttachMapZombie++;
            Event_PlusRadiusAttach();
            txt_RadiusAttach.text = $"+{10 * Data.indexRadiusAttachMapZombie}% Range";
        }
    }
    public void Button_PlusSpeedMovePlayer(int price)
    {
        if(true)
        {
            Data.indexSpeedMoveMapZombie++;
            Event_PlusSpeedMove();
            txt_SpeedMove.text = $"+{Data.indexSpeedMoveMapZombie * 10}% Speed";
        }
    }
    public void Button_PlusQuanlityWeaponPlayer(int price)
    {
        Data.indexQuanlityWeaponMapZombie++;
        txt_QuanlityWeapon.text = $"+{Data.indexQuanlityWeaponMapZombie - 1} Attach";
    }
    public void Button_PlusTimeALive()
    {

    }
    //event mua vat pham
    private bool Event_Buy(int price)
    {
        if (Data.Gold - price < 0) return false;
        Data.Gold -= price; // tru tien
        DataGame.Save(Data); // luu data
        txt_Gold.text = Data.Gold.ToString();
        return true;
    }
    private void Event_PlusRadiusAttach() => playerController.radiusAttach += playerController.radiusAttach * (Data.indexRadiusAttachMapZombie * 10) / 100;
    private void Event_PlusSpeedMove() => playerController.speedMove += playerController.speedMove * (Data.indexSpeedMoveMapZombie * 10) / 100;
    private void Event_PlusGold() => int_Gold += int_IndexPlusGold; // giet duoc enemy tang 1 vang
    public void Button_GetGold(int indexGold)
    {
        Data.Gold += int_Gold * indexGold;
        DataGame.Save(Data);
        SceneManager.LoadScene(LoadScene.GamePlay.ToString(), LoadSceneMode.Single);
    }
    #region Player Level Up
    // sumery: Player Level Up
    private void Event_QuanlityZombieKilled() // so luong quai da giet de player len levle
    {
        int_QuanlityZombieKilled++;
        if(int_QuanlityZombieKilled == int_IndexLevelUpPlayer)
        {
            int_IndexLevelUpPlayer *= Data.indexQuanlityWeaponMapZombie;

            Observer.Instance.Notify(StringData.PlusAttach);
            AudioManager.Instance.PlaySound(AudioClipName.LevelUp, this.transform);
        }
    }
    #endregion

    #region Zombie
    public void Event_InstacneZombie(GameObject Zombie)
    {
        if (int_QuanlityZombir - int_QuanlityZOmbieHienTai <= 0) return;
        Zombie.transform.position = OnGetPosInstanceZombeie();
        Zombie.SetActive(true);
    }
    private Vector3 OnGetPosInstanceZombeie() // lay tao do instance zpmbie
    {
        switch(UnityEngine.Random.Range(0, list_PosInstanceZombie.Count))
        {
            case 0: return list_PosInstanceZombie[0];
            case 1: return list_PosInstanceZombie[1];
            case 2: return list_PosInstanceZombie[2];
            case 3: return list_PosInstanceZombie[3];
            default: return list_PosInstanceZombie[0];
        }
    }
    private void Event_ZombieDie()
    {
        //Quanlity ZOmbie
        int_QuanlityZombir--;
        txt_QuanlityEnemy.text = int_QuanlityZombir.ToString();
        //Quanlity Player
        Quanlity++;
        txt_QuanlityZombieKilledPlayer.text = Quanlity.ToString();
        if(int_QuanlityZombir == 1) // game win
        {
            Event_SetPopup(UICuren.GameWin);
            Event_OnSetColorDayZombieGameWin(Data.Day, rawImgae_DayZombieGameWin, Data, true); 
        }
    }
    private void Event_OnSetColorDayZombieGameWin(int day, List<RawImage> rawImgae_DayZombie, Data Data, bool isWin /* true is win, false is thua */)
    {
        int j = 0;
        for(int i = Data.DayMax - 4; i < Data.DayMax; i++)
        {
            if (i < day) rawImgae_DayZombie[j].color = Color.green;
            else if (i == day)
            {
                if (isWin) rawImgae_DayZombie[j].color = Color.green;
                else rawImgae_DayZombie[j].color = Color.red;
            }
            else rawImgae_DayZombie[j].color = Color.gray;
            j++; // lay phan tu tieps theo trong list trong vong for tiep theo
        }
    }
    private void Event_Revive() => Event_SetPopup(UICuren.Revive);
    public void Event_EndSeconsLoading()
    {
        AudioManager.Instance.PlaySound(AudioClipName.CountDown, this.transform);
        int_Seconds--;
        txt_CodownTime.text = int_Seconds.ToString();
    }
    public void Event_GameOver()
    {
        Event_SetPopup(UICuren.GameOver);
        Event_OnSetColorDayZombieGameWin(Data.Day, rawImage_DayZombieGameOver, Data, false);
    }
    #endregion
    #region Event Skill
    public void Event_AttachBehild() => playerController.AttachBehild();
    public void Event_WeaponXuyen()
    {
        Debug.LogError("da bam nut");
        playerController.bullet.GetComponent<WeaponController>().isXuyen = true; // khong active wepoan khi va cam
    }
    public void Event_SpeedMove() => playerController.speedMove *= 2f; // tang toc do di cuyen cho player
    public void Event_AttachPlus() => Observer.Instance.Notify(StringData.PlusAttach); // danh ra nhieu phis cung 1 luc
    public void Button_SelectSkillPlayer() => dataSkillMapZombie.Event?.Invoke(); // kich hoat skill
    public void Button_NoThank() // buttonn choi game
    {
        ZombieManager.SetActive(true);
        Event_SetPopup(UICuren.GamePlay);
    }
    public void Button_BackToHome() => SceneManager.LoadScene(LoadScene.GamePlay.ToString(), LoadSceneMode.Single);
    public void Button_PopupSetting() // bBatSetting trong ZombieCity
    {
        Event_SetPopup(UICuren.Setting);
        Time.timeScale = 0f; // dung game lai
    }
    public void Button_Continued()
    {
        Event_SetPopup(UICuren.GamePlay);
        Time.timeScale = 1f;
    }
    #endregion
    #region Sound
    public void Button_OnTurnOnSetting()
    {
        Event_SetPopup(UICuren.Setting);
        Time.timeScale = 0f;
    }
    public void Button_OnTurbOffSetting()
    {
        Event_SetPopup(UICuren.GamePlay);
        Time.timeScale = 1f;
    }
    public void OnButtonSound() // tat bat am thanh
    {
        isSound = !isSound;
        if (isSound)
        {
            button_Sound.GetComponent<RectTransform>().anchoredPosition = new Vector3(43, 0, 0);
            button_Sound.GetComponent<Image>().color = Color.green;
            OnTurnOnSound();
        }
        else
        {
            button_Sound.GetComponent<RectTransform>().anchoredPosition = new Vector3(-43, 0, 0);
            button_Sound.GetComponent<Image>().color = Color.white;
            OnTurnOffSound();
        }
    }
    public void OnButtonVibation() // tat bat rung
    {
        isVabition = !isVabition;
        if (isVabition)
        {
            button_Vabition.GetComponent<RectTransform>().anchoredPosition = new Vector3(43, 0, 0);
            button_Vabition.GetComponent<Image>().color = Color.green;
        }
        else
        {
            button_Vabition.GetComponent<RectTransform>().anchoredPosition = new Vector3(-43, 0, 0);
            button_Vabition.GetComponent<Image>().color = Color.white;
        }
    }
    private void OnTurnOnSound() => AudioManager.Instance.isTurnOffAudio = false; // bat am thanh
    private void OnTurnOffSound() => AudioManager.Instance.isTurnOffAudio = true; // tat am thanh
    #endregion
}
