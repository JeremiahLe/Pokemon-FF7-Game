using System;
using TMPro;
using UnityEngine;

public class CommandWindowButton : MonoBehaviour
{
    public Command Command;
    public UnitSpecialAction CachedUnitSpecialAction;

    [SerializeField] private TextMeshProUGUI _buttonText;

    public void OnEnable()
    {
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetCachedUnitSpecialAction(UnitSpecialAction unitSpecialAction)
    {
        CachedUnitSpecialAction = unitSpecialAction;
        SetButtonText(CachedUnitSpecialAction.ActionName);
    }
    
    public void SetButtonText(string text)
    {
        _buttonText.text = text;
    }
}
