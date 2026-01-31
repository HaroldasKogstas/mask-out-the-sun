using System;
using UnityEngine;

namespace CheekyStork.ScriptableVariables
{
    [CreateAssetMenu(fileName = "SO_NewBoolSO", menuName = "Scriptable Objects/Variables/Bool")]
    [Serializable]
    public class BoolSO : VariableSO<bool>
    {
    }
}