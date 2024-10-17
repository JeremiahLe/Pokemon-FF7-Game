using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Affinity", menuName = "Affinity")]
public class AffinityClass : ScriptableObject
{
    public enum AffinityType
    {
        Wind, Fire, Water, Earth, Lightning, Ice, Mystic, Light, Shadow, Physical
    };
    public AffinityType affinityType;

    public List<AffinityType> AffinityWeaknesses;
    public List<AffinityType> AffinityResistances;
}
