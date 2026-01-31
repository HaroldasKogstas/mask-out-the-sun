using System;
using UnityEngine;

namespace CheekyStork.ScriptableVariables
{
    [CreateAssetMenu(fileName = "SO_NewFloatSO", menuName = "Scriptable Objects/Variables/Float")]
    [Serializable]
    public class FloatSO : VariableSO<float>
    {
    }
}