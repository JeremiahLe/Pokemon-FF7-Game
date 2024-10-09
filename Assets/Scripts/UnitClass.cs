using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Unit Class", menuName = "Unit/UnitClass")]
public class UnitClass : ScriptableObject
{
    public enum UnitCombatClass
    {
        Squire, Mage, Scout, Thief
    };

    public UnitCombatClass CombatClass;
}
