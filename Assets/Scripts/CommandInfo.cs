using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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
        var newText = _actionDescription.text;

        var amount = new Regex(Regex.Escape("{amount}"));
        var stat = new Regex(Regex.Escape("{stat}"));
        var target = new Regex(Regex.Escape("{target}"));

        foreach (var scalar in _damageSource.DamageScalars)
        {
            newText = amount.Replace(newText, new StringBuilder().Append(scalar.ScalingMultiplier).Append("%").ToString(), 1);
            newText = stat.Replace(newText, GetStatText(scalar.ScalingStat), 1);
        }

        newText = target.Replace(newText, GetTargetText(_damageSource.TargetingData));

        _actionDescription.text = newText;
    }

    public static string GetStatText(Stat scalingStat)
    {
        return scalingStat switch
        {
            Stat.CurrentHealth => "Current Health",
            Stat.MaxHealth => "Max Health",
            Stat.PhysicalAttack => "Physical Attack",
            Stat.SpecialAttack => "Special Attack",
            Stat.PhysicalDefense => "Physical Defense",
            Stat.SpecialDefense => "Special Defense",
            Stat.Speed => "Speed",
            _ => "None"
        };
    }

    public static string GetTargetText(TargetingData targetingData)
    {
        var text = targetingData.TargetCount == 1 ? "a single {0}." : $"{targetingData.TargetCount} {0}s.";

        var targetType = new Regex(Regex.Escape("{0}"));
        text = targetType.Replace(text, GetTargetTypeText(targetingData.TargetRestrictions));
        
        return text;
    }

    public static string GetTargetTypeText(TargetRestrictions targetRestrictions)
    {
        return targetRestrictions switch
        {
            TargetRestrictions.Any => "target",
            TargetRestrictions.Self => "self",
            TargetRestrictions.Allies => "ally",
            TargetRestrictions.Enemies => "enemy",
            _ => "None"
        };
    }
}
