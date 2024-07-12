using UnityEngine;

public class PoterController : MonoBehaviour
{
    private Transform Player;
    public UnityEngine.UI.Image Image;
    public GameObject Target;
    private Color color;

    private RectTransform RectTransform;
    private Vector2 posPlayerAndTarget;
    private void Start()
    {
        RectTransform = this.GetComponent<RectTransform>();
        Player = GamePlay_SceneManager.Instance.playerController.transform;

        color = Target.GetComponentInChildren<EnemyController>().skins.material.color;
        Image.color = this.color;
    }
    private void Update()
    {
        if (!Target.activeSelf) Destroy(this.gameObject);
        if (Target == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        RectTransform.rotation = Quaternion.Euler(0, 0, -getNormal());
        RectTransform.anchoredPosition = getPosition();

        if (CheckObjectCamera(Target))
        {
            this.Image.enabled = false;
        }
        else
        {
            this.Image.enabled = true;
        }
    }
    private float getNormal()
    {
        posPlayerAndTarget = new Vector2(Target.transform.position.x - Player.position.x, Target.transform.position.z - Player.position.z).normalized;
        return Mathf.Atan2(posPlayerAndTarget.x, posPlayerAndTarget.y) * Mathf.Rad2Deg;
    }
    private Vector2 getPosition()
    {
        float distance;
        float angle = Vector2.Angle(Vector2.right, posPlayerAndTarget);
        if(angle > 90) angle = 180 - angle;
        if (angle > 45) distance = (Screen.height / 2) / Mathf.Cos((90 - angle) * Mathf.Deg2Rad);
        else distance = (Screen.width / 2) / Mathf.Cos(angle * Mathf.Deg2Rad);
        Vector2 posScrenn = distance * posPlayerAndTarget;
        if (posScrenn.x >= 540) posScrenn.x = 540;
        else if (posScrenn.x < -540) posScrenn.x = -540;
        return posScrenn;
    }
    private bool CheckObjectCamera(GameObject gameObject)
    {
        Vector3 posView = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        if ((posView.x > 0 && posView.x < 1) && (posView.y > 0 && posView.y < 1) && posView.z > 0) return true;
        else return false;
    }
}

