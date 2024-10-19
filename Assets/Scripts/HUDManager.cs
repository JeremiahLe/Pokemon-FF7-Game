using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private GameObject _unitIconPrefab;
    [SerializeField] private GameObject _unitIconHolder;
    [SerializeField] private GameObject _enemyUnitIconHolder;
    [SerializeField] private UnitHoverIcon _UnitHoverIcon;

    private List<UnitIcon> UnitIcons = new List<UnitIcon>();
    
    public void InitializeComponents()
    {
        SpawnIcons();
    }

    private void SpawnIcons()
    {
        foreach (var unit in CombatManagerSingleton.CombatManager().UnitObjectsInScene)
        {
            var icon = Instantiate(_unitIconPrefab, GetUnitIconHolder(unit.unitSideOfField));
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
}
