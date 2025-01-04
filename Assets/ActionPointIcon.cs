using UnityEngine;
using UnityEngine.UI;

public class ActionPointIcon : MonoBehaviour
{
    [SerializeField] private Image _image;

    public void UpdateActionPointActive(bool isActive)
    {
        _image.color = isActive ? Color.white : Color.black;
    }
}
