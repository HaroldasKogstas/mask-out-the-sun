using System.Collections.Generic;
using CheekyStork.Logging;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using ListExtensions = CheekyStork.Extensions.ListExtensions;

// This class makes sure that all of its child classes are put into a resettable list.
// The purpose of this functionality is to allow SO's to be reset at well defined times instead of unity's ambiguous OnEnable or similar calls.
namespace CheekyStork.ScriptableVariables
{
    public abstract class ResettableSO : ScriptableObject
    {
        [field: SerializeField]
        protected bool IsResetting { get; private set; }

        [ShowIf("IsResetting")]
        [SerializeField]
        private List<ResettablesListSO> _resettablesLists;

        [ShowIf("IsResetting")]
        [ReadOnly]
        [SerializeField]
        private List<ResettablesListSO> _previousResettablesLists;

        [ShowIf("IsResetting")]
        [Button("Update Resettables Lists")]
        public void UpdateResettablesLists()
        {
            ManageResettablesLists();
        }

        public abstract void ResetResettable();

        private void ManageResettablesLists()
        {
            if (IsResetting)
            {
                HandleInspectorRemovedLists();

                if (_resettablesLists.IsNullOrEmpty())
                {
                    this.LogError("Resettable list not assigned.", LogTag.Core);
                    return;
                }

                AddToResettableLists();
            }
            else
            {
                RemoveAllResettableLists();
            }
        }

        private void HandleInspectorRemovedLists()
        {
            for (int i = _previousResettablesLists.Count - 1; i >= 0; i--)
            {
                ResettablesListSO resettablesList = _previousResettablesLists[i];

                if (!_resettablesLists.Contains(resettablesList))
                {
                    HandleListRemoval(resettablesList, _previousResettablesLists);
                }
            }
        }

        private void AddToResettableLists()
        {
            foreach (ResettablesListSO resettablesList in _resettablesLists)
            {
                if (resettablesList != null)
                {
                    resettablesList.TryAddUnique(this);

                    ListExtensions.TryAddUnique(_previousResettablesLists, resettablesList);
                    //this.LogInfo($"{this} Added to resettable list {resettablesList}.");
                }
                else
                {
                    this.LogError("ResettableList reference missing.", LogTag.Core);
                }
            }
        }

        private void RemoveAllResettableLists()
        {
            for (int i = _resettablesLists.Count - 1; i >= 0; i--)
            {
                ResettablesListSO resettablesList = _resettablesLists[i];

                HandleListRemoval(resettablesList, _resettablesLists);
            }
        }

        private void HandleListRemoval(ResettablesListSO listToRemove, List<ResettablesListSO> listToRemoveFrom)
        {
            if (listToRemove != null)
            {
                listToRemove.Remove(this);
            }

            listToRemoveFrom.Remove(listToRemove);
        }
    }
}