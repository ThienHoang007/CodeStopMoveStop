using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private PlayerController Player;
    private Animator animator;

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    void Update()
    {
        updateAnimation(this.Player.State);
    }
    private void updateAnimation(PlayerController.StatePlayer State)
    {
        for(int i = 0; i < getValueState(); i++)
        {
            if (State == (PlayerController.StatePlayer)i) animator.SetBool(((PlayerController.StatePlayer)i).ToString(), true);
            else animator.SetBool(((PlayerController.StatePlayer)i).ToString(), false);
        }
    }
    private int getValueState()
    {
        PlayerController.StatePlayer[] states = (PlayerController.StatePlayer[])Enum.GetValues(typeof(PlayerController.StatePlayer));
        return states.Length;
    }
}
