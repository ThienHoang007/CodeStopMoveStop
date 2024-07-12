using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Observer))]
public class GamePlay_SceneManager : MonoBehaviour
{
    private static GamePlay_SceneManager instance = null;
    public static GamePlay_SceneManager Instance => instance;
    [Header("--------------------Player--------------------")]
    public PlayerController playerController;
    [HideInInspector] public List<GameObject> particlaSytems = new List<GameObject>();   

    [Header("--------------------Element-------------------")]
    public WeaponShopController weaponShopController;
    public SkinShopController skinShopController;
    private Data Data;

    [Space(7)]
    [Header("--------------List UI GamePlay----------------")]
    [Tooltip("Game Play")]
    public List<GameObject> listGamePlays = new List<GameObject>();
    public Animation animationLeft, animationRight;
    public GameObject handIcon;
    public UICuren StateUI;

    [Space(7)]
    [Header("-------------------UI-------------------------")]
    public Text txt_SeconsLoading;
    private int in_Secons = 5, valueGold;
    public Text txt_Rank, txt_NameKill;
    public Text txt_Gold; // vang
    public Text txt_Alive; // so luong enemy con song
    public Text txt_NamePlayer; // ten player
    public InputField inputField_NamePlayer; // lay name player
    public Text txt_RankPlayer, txt_NameEnemy;

    [Space(7)]
    [Header("--------------------Sound----------------------")]
    public Button button_Vabition;
    public Button button_Sound;
    public Texture txture_OnSound, txture_OffSound;
    public RawImage rimage_Sound;
    private bool isSound = false, isVabition = false;

    [Space(7)]
    [Header("-------------------Data Enemy-------------------")]
    [Tooltip("Get Enemy in EnemyManager")] public GameObject enemyMaanger;
    public GameObject[] arr_Enemy = new GameObject[5]; // enemy instance
    public int quanlityEnemy; // quanlty enemy tren map
    public string string_NameEnemy, string_Rank;
    private int int_QuanlityCurent;
    private List<GameObject> lis_Enemys = new List<GameObject>();

    public enum UICuren { GamePlay, WeaponShop, Menu, SkinShop, GameWin, GameOver, Setting, Revive, UIRank }
    public enum LoadScene { GamePlay, ZombieCity }
    public enum Enemy { InstanceEnemy, EnemyDie }

    private void Awake()
    {
        if (instance == null) instance = this;

        Data = DataGame.Load();
        if (Data == null) Data = new Data();
        // Load Weapon Shop
        weaponShopController.Start_ShopWeapon(ref Data);
        // Load SKin Shop
        skinShopController.Start_ShopSkin(ref Data);
        //khoi tao ten user
        txt_NamePlayer.text = Data.NameUser;

        int_QuanlityCurent = enemyMaanger.transform.childCount + quanlityEnemy;
        txt_Alive.text = $"Alive: {int_QuanlityCurent}";

        Time.timeScale = 1; // kch hoat lai gane
        Observer.Instance.SupEvent(Enemy.EnemyDie.ToString(), Event_EnemyDie);
        Observer.Instance.SupEvent(Enemy.InstanceEnemy.ToString(), Event_InstanceEnemy); // sup event instance enemy
        Observer.Instance.SupEvent(UICuren.GameOver.ToString(), Event_GameOver);
    }
    public void UpdateUI()
    {
        for (int i = 0; i < listGamePlays.Count; i++)
        {
            if (listGamePlays[i].tag == StateUI.ToString()) listGamePlays[i].SetActive(true);
            else listGamePlays[i].SetActive(false);
        }
    }
    public void Event_Healder() => handIcon.SetActive(false);
    #region Button Set State UI
    public void Event_Revive() => StateUI = UICuren.Revive;
    public void Button_TurnOnSkinShop() // bat shop skin
    {
        playerController.isDance = true;
        Camera.main.GetComponent<Animation>().Play(StringData.ClipStart);
        animationLeft.Play(StringData.ClipMoveLeftLeft);
        animationRight.Play(StringData.ClipMoveRightRight);
        skinShopController.gameObject.SetActive(true);
    }
    public void Button_TurnOffSkinShop() // tat shop skin
    {
        playerController.isDance = false;
        Camera.main.GetComponent<Animation>().Play(StringData.ClipEnd);
        animationLeft.Play(StringData.ClipMoveLeftRight);
        animationRight.Play(StringData.ClipMoveRightLeft);
        skinShopController.gameObject.SetActive(false);
    }
    public void Button_TurnOffWeaponShop() // tat weapon shop
    {
        animationLeft.Play(StringData.ClipMoveLeftRight);
        animationRight.Play(StringData.ClipMoveRightLeft);
        weaponShopController.gameObject.SetActive(false);
    }
    public void Button_TurnOnWeaponShop() // bat weapon shop
    {
        animationLeft.Play(StringData.ClipMoveLeftLeft);
        animationRight.Play(StringData.ClipMoveRightRight);
        weaponShopController.gameObject.SetActive(true);
    }
    public void Button_Play() // Loading Game Play
    {
        enemyMaanger.SetActive(true);
        StateUI = UICuren.GamePlay;
        UpdateUI();

        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.BtnClick, this.transform);
        Observer.Instance.Notify(StringData.StartPlayer);
    }
    #endregion
    public void OnEndClipAnimationLoading() // Event End CLip ANimatiob Loading
    {
        in_Secons--;
        if (in_Secons == 0)
        {
            txt_NameEnemy.text = string_NameEnemy;
            txt_RankPlayer.text = $"# {int_QuanlityCurent}";
            StateUI = UICuren.UIRank;
            UpdateUI();
            return;
        }
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.CountDown, this.transform);
        txt_SeconsLoading.text = in_Secons.ToString();
    }
    public void Event_UIRank()
    {
        StateUI = UICuren.UIRank;
    }
    #region Sound
    public void OnButtonSound() // tat bat am thanh
    {
        isSound = !isSound;
        if (isSound)
        {
            button_Sound.GetComponent<RectTransform>().anchoredPosition = new Vector3(43, 0, 0);
            button_Sound.GetComponent<Image>().color = Color.green;
            OnTurnOnSound();
            UpdateUISoundButton();
        }
        else
        {
            button_Sound.GetComponent<RectTransform>().anchoredPosition = new Vector3(-43, 0, 0);
            button_Sound.GetComponent<Image>().color = Color.white;
            OnTurnOffSound();
            UpdateUISoundButton();
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
    public void Event_InputNamePlayer()
    {
        txt_NamePlayer.text = inputField_NamePlayer.text;
        Data.NameUser = txt_NamePlayer.text;
        DataGame.Save(Data);
    }
    private void UpdateUISoundButton() // update ui sound
    {
        if (AudioManager.Instance.isTurnOffAudio) rimage_Sound.texture = txture_OnSound;
        else rimage_Sound.texture = txture_OffSound;
    }
    public void Button_TurnOnSetting() // bat setting
    {
        StateUI = UICuren.Setting;
        UpdateUI();
        Time.timeScale = 0;
    }
    public void Button_TurnOffSetting()
    {
        StateUI = UICuren.GamePlay;
        UpdateUI();
        Time.timeScale = 1f;
    }
    #endregion
    #region Load Scene
    public void LoadScene_TouchToContinued() => SceneManager.LoadScene(LoadScene.GamePlay.ToString(), LoadSceneMode.Single);
    public void LoadScene_TouchToZombieCity() => SceneManager.LoadScene(LoadScene.ZombieCity.ToString(), LoadSceneMode.Single);
    #endregion
    //-------------------------------------------------------------------------------------------------------------------------
    #region Instance Enemy
    //summary: Instance Enemy Va Kch Hoat Su Kien Khi Player Chet Va Khi Player Win
    //note: sup event observer
    public void DesignPatern_ObjectPool(List<GameObject> objs, GameObject obj_TergetInstance, Vector3 posInstance,
        Transform paern = null, Action<GameObject> function = null)
    {
        if (objs.Count == 0)
        {
            GameObject obj_g = Instantiate(obj_TergetInstance, posInstance, Quaternion.identity, paern);
            function?.Invoke(obj_g);
            objs.Add(obj_g);
        }
        else
        {
            foreach (GameObject obj in objs)
            {
                if (!obj.activeSelf) // KHoi tao lai obj
                {
                    obj.transform.position = posInstance;
                    function?.Invoke(obj);
                    obj.SetActive(true);
                    return;
                }
            }
            GameObject obj_g = Instantiate(obj_TergetInstance, posInstance, Quaternion.identity, paern);
            function?.Invoke(obj_g);
            objs.Add(obj_g);
        }
    }
    public void Event_InstanceEnemy() // instance enemy
    {
        quanlityEnemy--;
        if (quanlityEnemy <= 0) return;

        DesignPatern_ObjectPool(lis_Enemys, GetTarget<GameObject>(arr_Enemy), GetPosEnemy()
            , enemyMaanger.transform, (GameObject obj) => Reset_ValueEnemy(obj.GetComponentInChildren<EnemyController>())); ;

    }
    public void Event_EnemyDie() // su kien khi enemy die
    {
        int_QuanlityCurent--; // Alive
        txt_Alive.text = $"Alive: {int_QuanlityCurent - 1}";

        if (int_QuanlityCurent == 1) Event_GameWin();
    }
    private void Reset_ValueEnemy(EnemyController enemyController)
    {
        enemyController.isDead = false;
        enemyController.isAttach = true;
        enemyController.isAttaching = false;
        enemyController.isMove = true;
    }
    private T GetTarget<T>(T[] values)
    {
        int x = UnityEngine.Random.Range(0, values.Length);
        for (int i = 0; i < values.Length; i++)
        {
            if (i == x) return values[i];
        }
        return values[0];
    }
    private Vector3 GetPosEnemy()
    {
        const float y = -0.04f; // vi tri y is const
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-10f, 0f), y, UnityEngine.Random.Range(-10f, 0f));
        while(Vector3.Distance(playerController.transform.position, pos) < 4f)
        {
            pos = new Vector3(UnityEngine.Random.Range(-10f, 0f), y, UnityEngine.Random.Range(-10f, 0f));
        }
        return pos;
    }
    #endregion
    #region Game Over And Game Win}
    private void Event_GameWin()
    {
        StateUI = UICuren.GameWin;
        UpdateUI();

        playerController.capsuleCollider.enabled = false;
        playerController.Event_PlayerDance();
    }
    private void Event_GameOver()
    {
        StateUI = UICuren.Revive;
        UpdateUI();

        playerController.capsuleCollider.enabled = false;
    }
    public void Event_UpdateUIPlayerRevive()
    {
        StateUI = UICuren.UIRank;
        UpdateUI();
    }
    public void Event_PlayerRevive()
    {
        StateUI = UICuren.GamePlay;
        UpdateUI();
    }
    #endregion
    private void OnDestroy()
    {
        DataGame.Save(Data);
    }
}
