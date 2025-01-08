using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    private UnitData _unitData;
    private List<ActionPointIcon> _actionPoints = new List<ActionPointIcon>();
    private int _lastActionPointCount = -1;

    [SerializeField] private Image _unitIconSprite;
    [SerializeField] private GameObject _actionPointIconPrefab;
    [SerializeField] private Transform _actionPointHolder;
    [field: SerializeField] public ResourceBar HealthBar { get; private set; }
    public Image UnitIconSprite => _unitIconSprite;

    private static readonly int TextDamageReceived = Animator.StringToHash("TextDamageReceived");

    private void Awake()
    {
        UnitObject.OnResourceUpdated += UpdateResourceBar;
    }

    private void Update()
    {
        if (!_unitData) return;

        UpdateMaxActionPointIcons();
        UpdateCurrentActionPointIcons();
    }
    
    public void InitializeData(UnitData unitData)
    {
        _unitData = unitData;
        
        _unitIconSprite.sprite = _unitData.UnitStaticData.UnitBaseSprite;

        InitializeResourceBars();
    }

    private void InitializeResourceBars()
    {
        HealthBar.InitializeResourceBar(_unitData.CurrentHealth, _unitData.BaseMaxHealthStat.CurrentTotalStat);
    }

    private void UpdateResourceBar(ResourceBar resourceBar, UnitData unitData, bool isInit = false)
    {
        if (unitData != _unitData) return;
        
        resourceBar.UpdateResourceBar(GetCurrentResourceAmount(resourceBar), GetMaxResourceAmount(resourceBar));
    }

    private float GetCurrentResourceAmount(ResourceBar resourceBar)
    {
        switch (resourceBar.CurrentResourceBarType)
        {
            case ResourceBar.ResourceBarType.Health:
                return _unitData.CurrentHealth;
            
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
                return _unitData.BaseMaxHealthStat.CurrentTotalStat;
            
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

    private void UpdateMaxActionPointIcons()
    {
        while (_actionPoints.Count > _unitData.BaseMaxActionPoints.CurrentTotalStat)
        {
            var removedItem = _actionPoints[_actionPoints.Count - 1];
            _actionPoints.Remove(removedItem);
            Destroy(removedItem.gameObject);
        }
        
        while (_actionPoints.Count < _unitData.BaseMaxActionPoints.CurrentTotalStat)
        {
            var ap = Instantiate(_actionPointIconPrefab, _actionPointHolder);
            ap.SetActive(true);
            var apComponent = ap.GetComponent<ActionPointIcon>();
            _actionPoints.Add(apComponent);
        }
    }

    private void UpdateCurrentActionPointIcons()
    {
        if (_unitData.UnitCurrentActionPoints == _lastActionPointCount) return;
        
        foreach (var ap in _actionPoints)
        {
            ap.UpdateActionPointActive(false);
        }
            
        var maxAmount = _unitData.UnitCurrentActionPoints > _actionPoints.Count ? _actionPoints.Count : _unitData.UnitCurrentActionPoints;
        for (var i = 0; i < maxAmount; i++)
        {
            _actionPoints[i].UpdateActionPointActive(true);
        }

        _lastActionPointCount = _unitData.UnitCurrentActionPoints;
    }
}
