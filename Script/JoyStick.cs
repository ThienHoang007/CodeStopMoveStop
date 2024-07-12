using UnityEngine;

public class JoyStick : MonoBehaviour
{
    public GameObject imaeg01;
    public GameObject imaeg02;
    public float radiusJoy;
    private float angle;
    private Vector2 diriction;
    public float Angle => angle;
    public Vector2 Diriction => diriction;
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            diriction = Vector2.zero;
            angle = 0f;
        }
        else if(Input.GetMouseButtonDown(0))
        {
            imaeg01.transform.position = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0))
        {
            if (Vector3.Distance(imaeg01.transform.position, Input.mousePosition) > radiusJoy)
            {
                imaeg02.transform.position = imaeg01.transform.position + (Input.mousePosition - imaeg01.transform.position).normalized * radiusJoy;
            }
            else imaeg02.transform.position = Input.mousePosition;
           
            diriction = (imaeg02.transform.position - imaeg01.transform.position);
            angle = Vector3.SignedAngle(Vector3.up, (imaeg02.transform.position - imaeg01.transform.position).normalized, Vector3.back);
        }
    }
}
