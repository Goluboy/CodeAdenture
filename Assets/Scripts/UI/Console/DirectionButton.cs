using UnityEngine;

public class DirectionButton : MonoBehaviour
{
    [SerializeField] private DirectionMenu _directionMenu;
    private void OnMouseDown()
    {
        _directionMenu.OnMouse(transform);
    }
}