using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static AudioManager;
using static GamePlay_SceneManager;
using System.Linq;
using UnityEngine.UIElements;

public class WeaponController : MonoBehaviour
{
    [Header("--------------------Value Weapon--------------")]
    public float speedMove;
    public float speedRotate;
    public float speedScale;
    public Rigidbody rb;
    public GameObject particalSytem;
    [HideInInspector] public Vector3 normal;

    [HideInInspector] public GameObject dad;
    private AdapterPatern adapterPatern;
    private Coroutine coroutine_DestroyAfterTime;
    private Action ac_Weapon = null;
    private Action ac_DestroyWeapon = null;
    private Action ac_WeaponHit = null;

    [Space(7)]
    [Header("---------------------Chosse Default-----------------")]
    [Tooltip("Is Stright Weapon ?")] public bool isStright = true;
    [Tooltip("Is Rotate Weapon ?")] public bool isRoate = true;
    [Tooltip("Is Destroy When Hit ?")] public bool isDestroy = true; // khi khong bomberang
    [Header("------------------------Chosse-----------------------")]
    public bool isScale = false;
    public bool isBomberang = false; // tat isDestroy
    public bool isXuyen = false;
    private void Awake()
    {
        if (isStright) ac_Weapon += WeaponStright;
        if (isRoate) ac_Weapon += WeaponRotate;
        else ac_Weapon += WeapomLookAtTarget;

        if (isScale) ac_Weapon += WeaponScale;
        if(!isXuyen) ac_WeaponHit = WeaponDestroy;
        // sub event destroy weapon
        if (isDestroy) ac_DestroyWeapon = WeaponDestroy;
        else if (isBomberang)
        {
            ac_DestroyWeapon = () =>
            {
                ac_Weapon -= WeaponStright;
                ac_Weapon += WeaponReturn;
            };
        }
        else ac_DestroyWeapon = null; // do not do anythinga
    }
    private void OnEnable()
    {
        if (!CheckMethodExistsInAction(ac_Weapon, nameof(WeaponStright)) && isBomberang) ac_Weapon += WeaponStright;
        ac_Weapon -= WeaponReturn;

        coroutine_DestroyAfterTime = StartCoroutine(DestroyAfterTime(1.75f, ac_DestroyWeapon));
    }
    private void Update()
    {
        ac_Weapon.Invoke();
    }
    private void WeaponStright() => rb.velocity = normal * speedMove;
    private void WeaponRotate() => this.transform.Rotate(0, 0, speedRotate * Time.deltaTime);
    private void WeaponScale() => this.transform.localScale += Vector3.one * speedScale * Time.deltaTime;
    private void WeaponDestroy() => this.gameObject.SetActive(false);
    private void WeapomLookAtTarget() => this.transform.up = -normal;
    private void WeaponReturn()
    {
        normal = (dad.transform.position - this.transform.position).normalized;
        rb.velocity = normal * speedMove * 1.5f;

        if(Vector3.Distance(this.transform.position, dad.transform.position) < 1f) this.gameObject.SetActive(false);    
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetInstanceID() == dad.GetInstanceID()) return; // khong giet chet thang cha
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.TryGetComponent<AdapterPatern>(out adapterPatern))
            {
                adapterPatern.Dead();
                adapterPatern.Rank(dad.name);
            }
            if(dad.CompareTag("Player")) // su li ngoia le voi player
            {
                Observer.Instance.Notify(StringData.PLYAERUPDATEUI);
                Observer.Instance.Notify(StringData.SCALEPLAYER);
            }

            try { SetParticalSytem(collision.transform.position + new Vector3(0, 0.75f, 0), Color.green); }
            catch { Debug.Log("Don't have partical sytem !"); }
            StopCoroutine(coroutine_DestroyAfterTime);
            ac_WeaponHit?.Invoke();
            AudioManager.Instance.PlaySound(AudioClipName.WeoponHit, this.transform);
        }
    }
    private void SetParticalSytem(Vector3 pos, Color color)
    {
        GamePlay_SceneManager.Instance.DesignPatern_ObjectPool(GamePlay_SceneManager.Instance.particlaSytems, particalSytem, pos, null, null);
    }
    private IEnumerator DestroyAfterTime(float time, Action functtion)
    {
        yield return new WaitForSeconds(time);
        functtion?.Invoke();
    }
    private bool CheckMethodExistsInAction(Action function, string name)
    {
        var invocationList = ac_Weapon.GetInvocationList();
        return invocationList.Any(x => x.Method.Name == name);
    }
}
