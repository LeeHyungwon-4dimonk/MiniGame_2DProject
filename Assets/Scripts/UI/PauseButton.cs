using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.TryPause(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.TryPause(false);
    }
}