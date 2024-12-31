using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionName;
    [SerializeField] private TextMeshProUGUI _actionDescription;
    [SerializeField] private GameObject _window;
    
    private Action OnTargetEnd = delegate { };
    private IDealsDamage _damageSource;

    private void Awake()
    {
        _window.SetActive(false);
        OnTargetEnd = () =>
        {
            ToggleVisibility(false);
            _damageSource = null;
        };
        
        CombatManager.OnUnitTargetingBegin += ToggleVisibilityInterim;
        CombatManager.OnUnitTargetingEnd += OnTargetEnd;
        CombatManager.OnDamageSourceSet += (b) => { _damageSource = b;};
    }

    private void OnDestroy()
    {
        CombatManager.OnUnitTargetingBegin -= ToggleVisibilityInterim;
        CombatManager.OnUnitTargetingEnd -= OnTargetEnd;
    }

    private void ToggleVisibilityInterim(List<UnitObject> unitObjects)
    {
        ToggleVisibility(true);
    }

    private void ToggleVisibility(bool isVisible)
    {
        _window.SetActive(isVisible);

        if (!isVisible) return;
        if (_damageSource == null) return;
        
        SetText();
    }
    
    private void SetText()
    {
        _actionDescription.text = _damageSource.ActionDescription;
    }
}
