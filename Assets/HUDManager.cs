using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private GameObject _unitIconPrefab;
    [SerializeField] private GameObject _unitIconHolder;

    private List<UnitIcon> UnitIcons = new List<UnitIcon>();
    
    public void InitializeComponents()
    {
        SpawnIcons();
    }

    private void SpawnIcons()
    {
        foreach (var unit in CombatManagerSingleton.CombatManager().UnitObjectsInScene)
        {
            if (unit.unitSideOfField != UnitOrientation.Left) continue;
            
            var icon = Instantiate(_unitIconPrefab, _unitIconHolder.transform);
            var iconComponent = icon.GetComponent<UnitIcon>();
            iconComponent.InitializeData(unit.UnitData);

            unit.BoundUnitIcon = iconComponent;
            UnitIcons.Add(iconComponent);
        }
    }
}
