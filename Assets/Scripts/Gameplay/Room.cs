using UnityEngine;

public class ResourceExchangingRoom: MonoBehaviour
{
    [SerializeField] private RoomType _type;
    [SerializeField] private ResourceBundle _inputResources;
    [SerializeField] private ResourceBundle _outputResources;
    [SerializeField] private float _exchangeInterval = 5f;
}
