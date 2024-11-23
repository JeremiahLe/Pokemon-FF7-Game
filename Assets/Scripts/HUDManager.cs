using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Header("Unit Icons")]
    [SerializeField] private GameObject _unitIconPrefab;
    [SerializeField] private GameObject _unitIconHolder;
    [SerializeField] private GameObject _enemyUnitIconHolder;
    
    [Header("Hovered Unit Indicator")]
    [SerializeField] private UnitHoverIcon _UnitHoverIcon;

    [Header("UnitActionOrder")] 
    [SerializeField] private GameObject _unitActionIconPrefab;
    [SerializeField] private GameObject _unitActionTimelineHolder;

    private List<UnitIcon> UnitIcons = new List<UnitIcon>();
    private List<UnitActionIcon> UnitActionIcons = new List<UnitActionIcon>();
    
    public void InitializeComponents()
    {
        SpawnUnitIcons();
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
        
        foreach (var unit in actionOrder)
        {
            var icon = Instantiate(_unitActionIconPrefab, _unitActionTimelineHolder.transform);
            icon.gameObject.SetActive(true);
            var iconComponent = icon.GetComponent<UnitActionIcon>();
            iconComponent.InitializeData(unit);
            
            UnitActionIcons.Add(iconComponent);
        }
    }

    private void ClearUnitActionIcons()
    {
        foreach (var unitActionIcon in UnitActionIcons)
        {
            Destroy(unitActionIcon.gameObject);
        }
        
        UnitActionIcons.Clear();
    }
}
