using CheekyStork.ScriptableVariables;
using UnityEngine;

public class ListResetter : MonoBehaviour
{
    [SerializeField]
    private ResettablesListSO _resettablesList;

    private void Awake()
    {
        _resettablesList.ResetAllResettables();
    }
}