using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Header("Unit Icons")]
    [SerializeField] private GameObject _unitIconPrefab;
    [SerializeField] private GameObject _unitIconHolder;
    [SerializeField] private GameObject _enemyUnitIconHolder;

    [Header("UnitActionOrder")] 
    [SerializeField] private GameObject _unitActionIconPrefab;
    [SerializeField] private GameObject _roundActionIconPrefab;
    [SerializeField] private GameObject _unitActionTimelineHolder;

    [Header("Combat")] 
    [SerializeField] private GameObject _combatPopupPrefab;
    
    [Header("Other Components")]
    [SerializeField] private UnitHoverIcon _unitHoverIcon;
    [SerializeField] private RectTransform _commandWindowTransform;
    [SerializeField] private Vector2 _commandWindowOffset;
    [SerializeField] private Canvas _canvas;

    private Camera _camera;

    private List<UnitIcon> UnitIcons = new List<UnitIcon>();
    private List<UnitActionIcon> UnitActionIcons = new List<UnitActionIcon>();
    private List<UnitHoverIcon> UnitHoverIcons = new List<UnitHoverIcon>();
    
    public void InitializeComponents()
    {
        _camera = Camera.main;
        
        SpawnUnitIcons();

        CombatManager.OnUnitActionStart += UpdateCommandWindowPosition;
        CombatManager.OnUnitTargetingBegin += SpawnTargeters;
        CombatManager.OnUnitTargetingEnd += ResetTargeters;
        UnitObject.OnDamageTaken += SpawnCombatPopup;
    }

    private void OnDestroy()
    {
        CombatManager.OnUnitActionStart -= UpdateCommandWindowPosition;
        CombatManager.OnUnitTargetingBegin -= SpawnTargeters;
        CombatManager.OnUnitTargetingEnd -= ResetTargeters;
        UnitObject.OnDamageTaken -= SpawnCombatPopup;
    }

    private void SpawnUnitIcons()
    {
        foreach (var unit in CombatManagerSingleton.CombatManager().UnitObjectsInScene)
        {
            var icon = Instantiate(_unitIconPrefab, GetUnitIconHolder(unit.unitSideOfField));
            icon.gameObject.SetActive(true);
            var iconComponent = icon.GetComponent<UnitIcon>();
            iconComponent.InitializeData(unit.UnitData);

            unit.BoundUnitIcon = iconComponent;
            UnitIcons.Add(iconComponent);
        }
    }

    private Transform GetUnitIconHolder(UnitOrientation unitOrientation)
    {
        switch (unitOrientation)
        {
            case UnitOrientation.Right:
                return _enemyUnitIconHolder.transform;
            
            case UnitOrientation.Left:
            case UnitOrientation.Center:
            default:
                return _unitIconHolder.transform;
        }
    }

    public void AdjustActionOrderIcons(Dictionary<UnitData, float> actionOrder)
    {
        ClearUnitActionIcons();

        var unitsActNextRound = new List<KeyValuePair<UnitData, float>>();
        
        foreach (var unit in actionOrder)
        {
            if (unit.Key.CurrentActionValue < CombatManagerSingleton.CombatManager().CurrentRoundActionValue)
            {
                AddActionOrderIcon(unit);
            }
            else
            {
                unitsActNextRound.Add(unit);
            }
        }

        AddRoundActionIcon();

        // These units act next round
        foreach (var unit in unitsActNextRound)
        {
            AddActionOrderIcon(unit);
        }
    }

    private void AddActionOrderIcon(KeyValuePair<UnitData, float> unit)
    {
        var icon = Instantiate(_unitActionIconPrefab, _unitActionTimelineHolder.transform);
        icon.gameObject.SetActive(true);
        var iconComponent = icon.GetComponent<UnitActionIcon>();
        iconComponent.InitializeData(unit);
            
        UnitActionIcons.Add(iconComponent);
    }
    
    private void AddRoundActionIcon()
    {
        var roundActionValueIcon = Instantiate(_roundActionIconPrefab, _unitActionTimelineHolder.transform);
        roundActionValueIcon.gameObject.SetActive(true);
        var roundActionValueIconComponent = roundActionValueIcon.GetComponent<UnitActionIcon>();
        roundActionValueIconComponent.InitializeData(CombatManagerSingleton.CombatManager().CurrentRound, CombatManagerSingleton.CombatManager().CurrentRoundActionValue);
        UnitActionIcons.Add(roundActionValueIconComponent);
    }

    private void ClearUnitActionIcons()
    {
        foreach (var unitActionIcon in UnitActionIcons)
        {
            Destroy(unitActionIcon.gameObject);
        }
        
        UnitActionIcons.Clear();
    }

    private void UpdateCommandWindowPosition(UnitObject unitObject)
    {
        var canvasRect = _canvas.GetComponent<RectTransform>();

        var viewportPosition = _camera.WorldToViewportPoint(unitObject.gameObject.transform.position);
        var screenPosition = new Vector2(((viewportPosition.x*canvasRect.sizeDelta.x)-(canvasRect.sizeDelta.x*0.5f)), ((viewportPosition.y*canvasRect.sizeDelta.y)- (canvasRect.sizeDelta.y*0.5f)));

        _commandWindowTransform.anchoredPosition = screenPosition + _commandWindowOffset;
    }

    private void SpawnTargeters(List<UnitObject> unitObjects)
    {
        _unitHoverIcon.gameObject.SetActive(false);
        ClearTargeters();
        
        foreach (var target in unitObjects)
        {
            var hoverIcon = Instantiate(_unitHoverIcon);
            hoverIcon.gameObject.SetActive(true);
            hoverIcon.UnitHoverState = UnitHoverIcon.UnitHoverIconState.Targeting;
            hoverIcon.HoverUnitObject(target);
            
            UnitHoverIcons.Add(hoverIcon);
        }
    }

    private void ResetTargeters()
    {
        ClearTargeters();
        _unitHoverIcon.gameObject.SetActive(true);
    }

    private void ClearTargeters()
    {
        foreach (var targeter in UnitHoverIcons)
        {
            Destroy(targeter.gameObject);
        }
        
        UnitHoverIcons.Clear();
    }

    private void SpawnCombatPopup(UnitObject unitObject, float amount)
    {
        //
    }
}
