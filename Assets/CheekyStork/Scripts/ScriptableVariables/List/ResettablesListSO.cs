using UnityEngine;

namespace CheekyStork.ScriptableVariables
{
    /// <summary>
    /// This ListSO holds a list of ResettableSOs and provides a method to reset all of them.
    /// </summary>
    [CreateAssetMenu(fileName = "SO_ResettablesListSO", menuName = "Scriptable Objects/Variables/Lists/Resettables List")]
    public class ResettablesListSO : ListSO<ResettableSO>
    {
        public void ResetAllResettables()
        {
            foreach (ResettableSO resettableVariableSO in List)
            {
                resettableVariableSO.ResetResettable();
            }
        }
    }
}
