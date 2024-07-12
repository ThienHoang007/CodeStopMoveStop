using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GamePlay_SceneManager;

public class EnemyController : AdapterPatern
{
    [Header("----------------------Value Enemy---------------------")]
    [SerializeField] private float radiusAttach;
    [SerializeField] private float timeAttach;
    [SerializeField] private float speedMove;
    [SerializeField] private float distance;
    private int valueScale = 0;
    private int Scale = 2;

    [Space(7)]
    [Header("----------------------ref component-------------------")]
    [SerializeField] private Transform posBullet;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Animator animator;
    [SerializeField, Tooltip("Pearnt Move")] private GameObject Pearnt;
    [SerializeField, Tooltip("Poiter")] private GameObject poiter; // mui ten tro vao enemy

    public GameObject posTay;
    public Material material_Enemy;
    public SkinnedMeshRenderer skins;
    public SkinnedMeshRenderer plant;
    private GameObject enemyManager;
    private GameObject player;
    public GameObject pointEnemy = null;
    public Text txt_NameEnemy;

    private Action event_Move, event_Attach;
    private Coroutine m_coroutine;
    public GameObject bullet;
    private List<GameObject> bullets = null;
    private PlayerController.StatePlayer StatePlayer = PlayerController.StatePlayer.Run;

    public bool isAttach = true; // co duoc attach khong ?
    public bool isAttaching = false; //co dang attach khong ?
    public bool isMove = true;
    private Vector3 normal;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public Vector3 diriction, posTarget;

    private void Awake()
    {
        event_Move = () =>
        {
            posTarget = new Vector3(UnityEngine.Random.Range(this.transform.position.x - distance, this.transform.position.x + distance + 1),
                this.transform.position.y, UnityEngine.Random.Range(this.transform.position.z - distance, this.transform.position.z + distance + 1));
            diriction = (posTarget - this.transform.position).normalized;
        };
    }
    private void OnEnable()
    {
        this.enabled = true;
        event_Move?.Invoke();
    }
    void Start()
    {
        this.enemyManager = Instance.enemyMaanger;
        this.player = Instance.playerController.gameObject;
        event_Move?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        Move_EnemyMove();
        Rotate_EnemyRotation();
        state();
        updateAnimation(StatePlayer);
        Attach_EnemyAttach();
    }
    #region Enemy Move And Rotate
    private void Move_EnemyMove() // Enemy di chuyen
    {
        if (!isMove) return;
            Pearnt.transform.position += diriction * speedMove * Time.deltaTime;
        if (Vector3.Distance(this.transform.position, posTarget) < 0.5f) event_Move?.Invoke();
    }
    private void Rotate_EnemyRotation() // Enemy xoay
    {
        if (isMove == false) return;

        Quaternion angle = this.transform.rotation;
        angle = Quaternion.Euler(angle.x, Mathf.Atan2(diriction.x, diriction.z) * Mathf.Rad2Deg, angle.z);
        this.transform.rotation = angle;
    }
    #endregion
    #region Enemy Attach
    private void Attach_EnemyAttach()
    {
        GameObject enemy = CheckAttach();
        if(enemy != null && isAttach)
        {
            isAttach = false;
            isMove = false;
            this.transform.LookAt(enemy.transform.position);
            StartCoroutine(ActionAfterTime(1, () =>
            {
                normal = Quaternion.Euler(0,-5,0) * (enemy.transform.position - posTay.transform.position).normalized;
                isAttaching = true;
                StartCoroutine(ActionAfterTime(timeAttach, () =>
                {
                    isAttach = true;
                    isMove = true;
                }));
            }));
        }
    }
    private void setEndAnimationAttch() => isAttaching = false;
    private GameObject CheckAttach() // kiem tra co duoc attach khong ?
    {
        for(int i = 0; i <= enemyManager.transform.childCount; i++)
        {
            if(i < enemyManager.transform.childCount)
            {
                if (!enemyManager.transform.GetChild(i).GetChild(0).GetComponent<EnemyController>().isDead
                    && Vector3.Distance(this.transform.position, enemyManager.transform.GetChild(i).position) < radiusAttach
                    && Vector3.Distance(this.transform.position, enemyManager.transform.GetChild(i).position) > 0.2f
                    && enemyManager.transform.GetChild(i).gameObject.activeSelf) return enemyManager.transform.GetChild(i).gameObject;
            }
            else
            {
                if (Vector3.Distance(this.transform.position, player.transform.position) < radiusAttach && player.activeSelf 
                    && player.GetComponent<PlayerController>().isDead == false) return player;
            }
        }
        return null;
    }
    private void InstanceBullet()
    {
        if (bullets == null)
        {
            bullets = new List<GameObject>();
            var g = Instantiate(bullet, posBullet.position, Quaternion.Euler(90, 0, 0));
            var weaponController = g.GetComponent<WeaponController>();
            weaponController.normal = this.normal;
            weaponController.dad = this.gameObject;
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
                    bullet.GetComponent<WeaponController>().normal = this.normal;
                    return;
                }
            }
            var g = Instantiate(bullet, posBullet.position, Quaternion.Euler(90, 0, 0));
            var weaponController = g.GetComponent<WeaponController>();
            weaponController.normal = this.normal;
            weaponController.dad = this.gameObject;
            bullets.Add(g);
        }
    }
    #endregion
    #region Animation
    private void state()
    {
        if (isDead) StatePlayer = PlayerController.StatePlayer.Dead;
        else if (isAttaching) StatePlayer = PlayerController.StatePlayer.Attach;
        else if (isMove) StatePlayer = PlayerController.StatePlayer.Run;
        else StatePlayer = PlayerController.StatePlayer.Idle;
    }
    private void updateAnimation(PlayerController.StatePlayer State)
    {
        for (int i = 0; i < 5; i++)
        {
            if (State == (PlayerController.StatePlayer)i) animator.SetBool(((PlayerController.StatePlayer)i).ToString(), true);
            else animator.SetBool(((PlayerController.StatePlayer)i).ToString(), false);
        }
    }
    #endregion
    public override void Dead()
    {
        this.isDead = true;
        this.isMove = false;
        if (m_coroutine != null) StopCoroutine(m_coroutine);

        // notify event 
        Observer.Instance.Notify(Enemy.InstanceEnemy.ToString());
        AudioManager.Instance.PlaySound(AudioManager.AudioClipName.EnemyDead, this.transform);
    }
    public override void ValueScale()
    {
        valueScale++;
        if (valueScale == Scale)
        {
            this.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            radiusAttach += 0.7f;
            Scale *= 2;
        }
    }
    public void setPlayerIsDead()
    {
        Observer.Instance.Notify(Enemy.EnemyDie.ToString());
        this.transform.parent.gameObject.SetActive(false);
    }
    private IEnumerator ActionAfterTime(float time, Action function)
    {
        yield return new WaitForSeconds(time);
        function?.Invoke();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ban"))
        {
            diriction = -diriction;
            m_coroutine = StartCoroutine(ActionAfterTime(3f, event_Move));
        }
    }
    private void OnChangePosPoiter()
    {
        poiter.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
    }
    public override void Rank(string name) { }
}
