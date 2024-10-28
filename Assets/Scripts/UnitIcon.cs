using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    public UnitData UnitData;

    [SerializeField] private Image _unitIconSprite;
    [field: SerializeField] public ResourceBar HealthBar { get; private set; }
    [field: SerializeField] public ResourceBar StaminaBar { get; private set; }
    [field: SerializeField] public ResourceBar ManaBar { get; private set; }
    
    private static readonly int TextDamageReceived = Animator.StringToHash("TextDamageReceived");

    private void Awake()
    {
        UnitObject.OnResourceUpdated += UpdateResourceBar;
    }
    
    public void InitializeData(UnitData unitData)
    {
        UnitData = unitData;
        
        _unitIconSprite.sprite = UnitData.UnitBaseSprite;

        InitializeResourceBars();
    }

    private void InitializeResourceBars()
    {
        HealthBar.InitializeResourceBar(UnitData.MaxHealth);
        HealthBar.UpdateResourceBar(UnitData.CurrentHealth, UnitData.MaxHealth, true);
    }

    private void UpdateResourceBar(ResourceBar resourceBar, UnitData unitData, bool isInit = false)
    {
        if (unitData != UnitData) return;
        
        resourceBar.UpdateResourceBar(GetCurrentResourceAmount(resourceBar), GetMaxResourceAmount(resourceBar), isInit);
    }

    private float GetCurrentResourceAmount(ResourceBar resourceBar)
    {
        switch (resourceBar.CurrentResourceBarType)
        {
            case ResourceBar.ResourceBarType.Health:
                return UnitData.CurrentHealth;
            
            case ResourceBar.ResourceBarType.Mana:
                break;
            
            case ResourceBar.ResourceBarType.Stamina:
                break;
            
            case ResourceBar.ResourceBarType.Ammo:
                break;
            
            case ResourceBar.ResourceBarType.Shield:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        return 0;
    }
    
    private float GetMaxResourceAmount(ResourceBar resourceBar)
    {
        switch (resourceBar.CurrentResourceBarType)
        {
            case ResourceBar.ResourceBarType.Health:
                return UnitData.MaxHealth;
            
            case ResourceBar.ResourceBarType.Mana:
                break;
            
            case ResourceBar.ResourceBarType.Stamina:
                break;
            
            case ResourceBar.ResourceBarType.Ammo:
                break;
            
            case ResourceBar.ResourceBarType.Shield:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        return 0;
    }
    
    public void AnimationTextDamageReceivedStart()
    {
        GetComponent<Animator>().SetBool(TextDamageReceived, true);  
    }
    
    public void AnimationTextDamageReceivedEnd()
    {
        GetComponent<Animator>().SetBool(TextDamageReceived, false);  
    }
}
