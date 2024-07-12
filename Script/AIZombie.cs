using System;
using UnityEngine;
using UnityEngine.AI;
using static ZombieCity_SceneManager;
using static AudioManager;
using static GamePlay_SceneManager;
public class AIZombie : AdapterPatern
{
    [Header("-----------------ref----------------")]
    public NavMeshAgent agent_NavMeshZombie;
    public Animator amin_Animator;
    private PlayerController playerController;

    [Space(7)]
    [Header("-------------Value Zombie-----------")]
    public float speedMove;
    private float speed_Move;
    private void Awake()
    {
        speed_Move = speedMove;
        agent_NavMeshZombie.speed = speedMove;
    }
    private void Start()
    {
        playerController = ZombieCity_SceneManager.Instance.playerController;

        Event_ZombieRun();
        Observer.Instance.SupEvent(StringData.ZOMBIEDANCE, Event_ZombieDance);
    }
    private void Update()
    {
        agent_NavMeshZombie.destination = playerController.transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<AdapterPatern>().Dead();
            collision.collider.enabled = false; // tat collider plyaer

            Observer.Instance.Notify(UICuren.GameOver.ToString());
            Observer.Instance.Notify(StringData.ZOMBIEDANCE);
            this.enabled = false;
        }
    }
    private void Event_ZombieDance()
    {
        agent_NavMeshZombie.speed = 0; // zombie khong di chuyen
        amin_Animator.SetBool("Dance", true);
        amin_Animator.SetBool("Run", false);
    }
    private void Event_ZombieRun()
    {
        amin_Animator.SetBool("Dance", false);
        amin_Animator.SetBool("Run", true);
    }
    public override void Dead()
    {
        this.gameObject.SetActive(false);

        //Event
        Observer.Instance.Notify(StringData.ZOMBIEDIE);
        Observer.Instance.Notify(QUANLITYWASKILLED);
        AudioManager.Instance.PlaySound(AudioClipName.ZombieDead, this.transform);

        ZombieCity_SceneManager.Instance.Event_InstacneZombie(this.gameObject);
    }
    public override void ValueScale() { }

    public override void Rank(string name) { } // khnog lam gi ca
}
