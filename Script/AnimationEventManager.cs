using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    public void OnEndClipAnimationLoading() => GamePlay_SceneManager.Instance.OnEndClipAnimationLoading();
    public void OnSetEndHaldAnimation() => GamePlay_SceneManager.Instance.Event_Healder();
    public void Event_OnSetPopupGameOver() => ZombieCity_SceneManager.Instance.Event_GameOver();
    public void Event_Audio() => ZombieCity_SceneManager.Instance.Event_EndSeconsLoading();
}
