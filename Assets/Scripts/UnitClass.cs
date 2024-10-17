using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Unit Class", menuName = "Unit/UnitClass")]
public class UnitClass : ScriptableObject
{
    public enum UnitCombatClass
    {
        Fighter, Mage, Ranger, Rogue, Tank
    };

    public UnitCombatClass CombatClass;
}
