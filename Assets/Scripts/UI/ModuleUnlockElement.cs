using UnityEngine;
using UnityEngine.UI;

public class ModuleUnlockElement : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Room _room;
    
    private void Start()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }
    
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }
    
    private void OnButtonClicked()
    {
        _room.TryUnlock();
    }
}
