using CheekyStork.ScriptableVariables;
using UnityEngine;

public class GroundLevelChanger : MonoBehaviour
{
    [SerializeField]
    private BoolSO _isBelowGround;

    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0f)
        {
            _isBelowGround.Value = true;
        }

        else if (Input.mouseScrollDelta.y < 0f)
        {
            _isBelowGround.Value = false;
        }
    }
}