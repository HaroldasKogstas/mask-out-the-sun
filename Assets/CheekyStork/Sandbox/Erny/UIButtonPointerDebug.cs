using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class UIButtonPointerDebug : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    [SerializeField]
    private bool _logRaycastBlockers = true;

    private Button _button;
    private Graphic _graphic;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _graphic = GetComponent<Graphic>();
    }

    private void OnEnable()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError($"[{name}] No EventSystem in the scene. UI won't receive hover/click events.");
        }

        if (_button == null)
        {
            Debug.LogWarning($"[{name}] No Button component found on this object.");
        }
        else
        {
            Debug.Log($"[{name}] Button.interactable={_button.interactable}, enabled={_button.enabled}");
        }

        if (_graphic == null)
        {
            Debug.LogWarning($"[{name}] No Graphic (Image/TextMeshProUGUI) found on this object. Color Tint needs a Graphic target.");
        }
        else
        {
            Debug.Log($"[{name}] Graphic={_graphic.GetType().Name}, raycastTarget={_graphic.raycastTarget}");
        }

        if (_logRaycastBlockers)
        {
            LogTopRaycastTargetUnderMouse();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[{name}] Pointer ENTER");
        if (_logRaycastBlockers)
        {
            LogTopRaycastTargetUnderMouse();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"[{name}] Pointer EXIT");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[{name}] Pointer DOWN (button={eventData.button})");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"[{name}] Pointer UP (button={eventData.button})");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[{name}] Pointer CLICK (button={eventData.button})");
    }

    private void LogTopRaycastTargetUnderMouse()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        if (results.Count == 0)
        {
            Debug.Log($"[{name}] RaycastAll found 0 UI hits at mouse position.");
            return;
        }

        RaycastResult top = results[0];
        Debug.Log($"[{name}] Top UI hit: {top.gameObject.name} (module={top.module}, depth={top.depth}, sortingLayer={top.sortingLayer}, sortingOrder={top.sortingOrder})");
    }
}
