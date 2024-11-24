using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionIcon : MonoBehaviour
{
    [SerializeField] private Image _unitSprite;
    [SerializeField] private TextMeshProUGUI _unitActionValue;

    public void InitializeData(KeyValuePair<UnitData, float> unit)
    {
        _unitSprite.sprite = unit.Key.UnitBaseSprite;
        _unitActionValue.text = unit.Value.ToString();
    }
    
    public void InitializeData(int currentRound, int roundActionValue)
    {
        _unitActionValue.text = ($"Round {currentRound}: {roundActionValue} AV");
    }
}
