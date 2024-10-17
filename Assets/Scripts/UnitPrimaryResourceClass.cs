using System;
using UnityEngine;

public class UnitPrimaryResourceClass : MonoBehaviour
{
    public enum UnitPrimaryResource { Stamina, Mana, Ammo, None }

    public static UnitPrimaryResource GetUnitPrimaryResourceByClass(UnitClass.UnitCombatClass unitCombatClass)
    {
        switch (unitCombatClass)
        {
            case UnitClass.UnitCombatClass.Fighter:
                return UnitPrimaryResource.Stamina;
            
            case UnitClass.UnitCombatClass.Mage:
                return UnitPrimaryResource.Mana;
            
            case UnitClass.UnitCombatClass.Ranger:
                return UnitPrimaryResource.Ammo;
            
            case UnitClass.UnitCombatClass.Rogue:
            case UnitClass.UnitCombatClass.Tank:
            default:
                return UnitPrimaryResource.None;
        }
    }
}
