using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private Vector3 distnaceWithPlayer;
    private Vector3 positionStart;
    private bool isActive = false;
    private Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        Observer.Instance.SupEvent(StringData.Camera, OnSetPosCameraStart);
        Observer.Instance.SupEvent(StringData.CameraWin, OnCameraWin);

        distnaceWithPlayer = this.transform.position - Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CameraWin();
    }
    private void CameraWin()
    {
        if(isActive) this.transform.position = Vector3.MoveTowards(this.transform.position, target, 0.5f);
        else
        {
            this.transform.position = Player.transform.position + distnaceWithPlayer;
            target = this.transform.position + this.transform.forward * 2.5f;
        }
    }
    private void OnCameraWin()
    {
        isActive = true;
    }
    private void OnSetPosCameraStart() => this.transform.position = positionStart;
}
