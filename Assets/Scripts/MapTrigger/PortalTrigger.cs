using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
         {
            SceneChanger.Instance.SceneChange(SceneName.Stage2Scene);
        }
    }
}
