using System;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CommandInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionName;
    [SerializeField] private TextMeshProUGUI _actionDescription;

    [SerializeField] private UnitSpecialAction _unitSpecialAction;

    private void Awake()
    {
        SetText();
    }

    public void SetDamageSource()
    {
        SetText();
    }

    private void SetText()
    {
        _actionDescription.text = _unitSpecialAction.ActionDescription;
        var newText = _actionDescription.text;

        var amount = new Regex(Regex.Escape("{0}"));
        var stat = new Regex(Regex.Escape("{1}"));
        var target = new Regex(Regex.Escape("{2}"));

        foreach (var scalar in _unitSpecialAction.DamageScalars)
        {
            newText = amount.Replace(newText, new StringBuilder().Append(scalar.ScalingMultiplier).Append("%").ToString(), 1);
            newText = stat.Replace(newText, GetStatText(scalar.ScalingStat), 1);
        }

        newText = target.Replace(newText, GetTargetText(_unitSpecialAction.TargetingData));

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
