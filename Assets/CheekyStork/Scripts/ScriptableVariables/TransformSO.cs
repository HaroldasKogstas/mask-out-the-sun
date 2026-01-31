using System;
using UnityEngine;

namespace CheekyStork.ScriptableVariables
{
    [CreateAssetMenu(fileName = "SO_NewTransform", menuName = "Scriptable Objects/Variables/Transform")]
    [Serializable]
    public class TransformSO : VariableSO<Transform>
    {
    }
}