using System;
using UnityEngine;

namespace CheekyStork.ScriptableVariables
{
    [CreateAssetMenu(fileName = "SO_NewIntSO", menuName = "Scriptable Objects/Variables/Int")]
    [Serializable]
    public class IntSO : VariableSO<int>
    {
    }
}