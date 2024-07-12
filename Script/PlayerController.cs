using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : AdapterPatern
{
    [Header("--------------ChiSoPlayer---------------")]
    [Tooltip("Value Player")]
    public float radiusAttach;
    public float timeAttach;
    public float speedMove;
    public bool isScale = true;
    private int Quanlity = 0;

    [Space(10)]
    [Header("-------------------ref-------------------")]
    [SerializeField] private GameObject crileOutline;
    [SerializeField] private Transform posBullet;
    [SerializeField] public CapsuleCollider capsuleCollider;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private GameObject parentPlayer; // Cha cua this
    [SerializeField] private GameObject enemyManager;
    [SerializeField] private Animator animatorPlayer;

    [Space(10)]
    [Header("------------------------------------------")]
    public Animation clip;
    public StatePlayer State;

    private List<GameObject> bullets = null;
    private Action eventInstance = null;
    private Action eventPlayerDead = null;
    private Coroutine corou_Attach = null;

    [Space(10)]
    [Header("------------GameObject Player Start----------")]
    public GameObject cirle;
    public GameObject canvasPlayer;
    public GameObject bullet;
    public GameObject posTay;

    [Space(10)]
    [Header("-----------------------UI---------------------")]
    public Text txt_QuanlityPlayer; // so luong enemy ma player da tieu diet
    public GameObject canvas_Player;
    private int int_QunaltyPlayer = 0;

    [HideInInspector] public bool CheckDead = true; // player co duoc chet hay khong // ham Dead
    [HideInInspector] public bool isBatTu = false;
    [HideInInspector] public bool isHoiSinh = false;

    //Value Start
    private int valueScale = 0;
    private int Scale = 2;
    private float speedStart;
    private Vector3 normal, posStart;
    private Vector2 posMove;

    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isDance = false;
    [HideInInspector] public bool isUnti;
    [HideInInspector] private bool isAttach = false;
    private bool isPlayerAttach = true;

    private const string PlayerDance = "PlayerDance";

    public enum StatePlayer { Idle, Run, Attach, Dead, Win, Dance, Ulti };
    private void OnEnable()
    {
        capsuleCollider.enabled = true;
    }
    private void OnDisable()
    {
        canvasPlayer.SetActive(false);
    }
    private void Awake()
    {
        posStart = this.transform.parent.position;
        speedStart = speedMove;
        eventInstance = null;
        eventPlayerDead = () => SetEventPlayerDead_01();
    }
    // Start is called before the first frame update
    void Start()
    {
        Observer.Instance.SupEvent(StringData.StartPlayer, StateStartPlayer);
        Observer.Instance.SupEvent(StringData.PlusAttach, EventAttachQuanlity);
        Observer.Instance.SupEvent(StringData.PLYAERUPDATEUI, Event_UpdateUI);
        Observer.Instance.SupEvent(StringData.SCALEPLAYER, ValueScale);

        EventAttachQuanlity();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        state();
        PlayerAttach(ref corou_Attach);
        RotationPlayer();
        updateAnimation(State); 
    }
    #region Player Move And Rotate
    //Player Move
    private void PlayerMove()
    {
        posMove = joystick.Direction;
        parentPlayer.transform.position += new Vector3(posMove.x, 0, posMove.y) * speedMove * Time.deltaTime;
    }
    //Player xoay
    private void RotationPlayer()
    {
        if (!CheckPlayerMove())
        {
            GameObject enemy = GetEnemy(enemyManager);
            if(enemy != null) this.transform.LookAt(enemy.transform.position);
        }
        else this.transform.rotation = Quaternion.Euler(0, Mathf.Atan2(joystick.Horizontal, joystick.Vertical) * Mathf.Rad2Deg, 0);
    }
    #endregion
    #region Player Attach
    // Duyet Enemy  
    private GameObject GetEnemy(GameObject enemyManager)
    {
        float distance;
        for(int i = 0; i < enemyManager.transform.childCount; i++)
        {
            GameObject obj = enemyManager.transform.GetChild(i).gameObject;
            distance = Vector3.Distance(obj.transform.position, this.transform.position);
            try { if (distance < radiusAttach && !obj.GetComponentInChildren<EnemyController>().isDead && obj.activeSelf) return enemyManager.transform.GetChild(i).gameObject; }
            catch { if (distance < radiusAttach && obj.activeSelf) return enemyManager.transform.GetChild(i).gameObject; }
        }
        return null;
    }
    // Player Tan Cong 
    private void PlayerAttach(ref Coroutine crou_Attach)
    {
        GameObject enemy = GetEnemy(enemyManager);
        if (CheckPlayerMove()) isAttach = false;
        if (enemy != null && isPlayerAttach && !CheckPlayerMove())
        {
            // set player attch enemy
            isPlayerAttach = false; // player khong duoc danh
            isAttach = true; // cap nhat state attach for player
            normal = (enemy.transform.position - posTay.transform.position).normalized;
            this.transform.LookAt(enemy.transform.position);
            crou_Attach = StartCoroutine(ActionAfterTime(2f, () => isPlayerAttach = true));

            //cirle outline 
            crileOutline.SetActive(true);
            crileOutline.transform.position = enemy.transform.position;
        }
        if(enemy == null) crileOutline.SetActive(false);
    }
    //Player co di chuyeng khong ?
    private bool CheckPlayerMove()
    {
        if (posMove == Vector2.zero) return false;
        return true;
    }
    private void state()
    {
        if (isDead) this.State = StatePlayer.Dead;
        else if (isAttach) this.State = StatePlayer.Attach;
        else if (isDance) this.State = StatePlayer.Dance;
        else if (isUnti) this.State = StatePlayer.Ulti;
        else if (CheckPlayerMove()) this.State = StatePlayer.Run;
        else this.State = StatePlayer.Idle; 
    }
    public void Event_PlayerDance()
    {
        isDance = true;
        speedMove = 0f;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    private void InstanceBullet()
    {
        if(eventInstance != null) print($"EventInstance {eventInstance}");
        eventInstance?.Invoke();
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.NemVuKhi, this.transform);
    }
    private void setEndAnimationAttch() => isAttach = false;
    #endregion
    #region Player Dead
    public override void Dead() => eventPlayerDead?.Invoke();
    public void SetEventPlayerDead_01() // su kien player mac dinh
    {
        isDead = true;
        speedMove = 0f;

        Observer.Instance.Notify(GamePlay_SceneManager.UICuren.GameOver.ToString());
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.MainDead, this.transform);
    }
    public void SetEventPlayerDead_02() // su kien khi player doi sinh
    {
        isDead = true;
        speedMove = 0f;
        GamePlay_SceneManager.Instance.Event_UpdateUIPlayerRevive();
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.MainDead, this.transform);
    }
    public void Event_PlayerRevive()
    {
        isDead = false;
        this.enabled = true;
        isAttach = false;
        speedMove = speedStart;
        capsuleCollider.enabled = true;

        eventPlayerDead = () => SetEventPlayerDead_02();
        GamePlay_SceneManager.Instance.Event_PlayerRevive();
    }
    public void setPlayerIsDead() => this.enabled = false;
    private void StateStartPlayer()
    {
        canvasPlayer.SetActive(true);
        cirle.SetActive(true);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
        clip.Play();
    }
    #endregion
    #region Player Phong To
    public override void ValueScale()
    {
        if (!isScale) return;
        valueScale++;
        if(valueScale == Scale)
        {
            this.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            Camera.main.transform.position += -Camera.main.transform.forward;
            radiusAttach += 0.7f;
            Scale *= 2;
        }
    }
    #endregion
    #region Event value
    //Set Value Player
    public void OnRadiusAttach(int value) => radiusAttach += value;
    public void OnSpeedMove(int value)
    {
        speedMove += value;
        speedStart = speedMove;
    }
    #endregion
    #region Event Attach
    //Attach Default
    private void AttachDfault(Vector3 x)
    {
        if (bullets == null)
        {
            bullets = new List<GameObject>();
            var g = Instantiate(bullet, posBullet.position, Quaternion.Euler(90, 0, 0));
            g.GetComponent<WeaponController>().normal = x;
            g.GetComponent<WeaponController>().dad = this.gameObject;
            bullets.Add(g);
        }
        else
        {
            foreach (var bullet in bullets)
            {
                if (!bullet.activeSelf)
                {
                    bullet.SetActive(true);
                    bullet.transform.position = posBullet.position;
                    bullet.transform.rotation = Quaternion.Euler(90, 0, 0);
                    bullet.GetComponent<WeaponController>().normal = x;
                    return;
                }
            }
            var g = Instantiate(bullet, posBullet.position, Quaternion.Euler(90, 0, 0));
            g.GetComponent<WeaponController>().normal = x;
            g.GetComponent<WeaponController>().dad = this.gameObject;
            bullets.Add(g);
        }
    }
    public void AttachBehild() => eventInstance += () => AttachDfault(-normal); // Attach Behild
    public void EventAttachQuanlity () // Ham lay so luong attach
    {
        int angle = 12;
        int Quanlity = this.Quanlity;

        if (Quanlity % 2 != 0) eventInstance += () => AttachDfault(Quaternion.Euler(0, (Quanlity - (int)(Quanlity / 2)) * angle, 0) * normal);
        else eventInstance += () => AttachDfault(Quaternion.Euler(0, -(Quanlity / 2) * angle, 0) * normal);
        this.Quanlity++;
    }
    #endregion
    #region Animation
    private void updateAnimation(StatePlayer State)
    {
        for (int i = 0; i < getValueState(); i++)
        {
            if (State == (StatePlayer)i) animatorPlayer.SetBool(((StatePlayer)i).ToString(), true);
            else animatorPlayer.SetBool(((StatePlayer)i).ToString(), false);
        }
    }
    private int getValueState()
    {
        StatePlayer[] states = (StatePlayer[])Enum.GetValues(typeof(StatePlayer));
        return states.Length;
    }
    #endregion
    public void Event_UpdateUI()
    {
        int_QunaltyPlayer++;
        txt_QuanlityPlayer.text = int_QunaltyPlayer.ToString();
    }
    private IEnumerator ActionAfterTime(float time, Action function)
    {
        yield return new WaitForSeconds(time);
        function?.Invoke();
    }

    public override void Rank(string name) => GamePlay_SceneManager.Instance.string_NameEnemy = name;
}