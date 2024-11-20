using UnityEngine;
using UnityEngine.UI;

public class UnitActionIcon : MonoBehaviour
{
    [SerializeField] private Image _unitSprite;

    public void InitializeData(UnitData unitData)
    {
        _unitSprite.sprite = unitData.UnitBaseSprite;
    }
}
