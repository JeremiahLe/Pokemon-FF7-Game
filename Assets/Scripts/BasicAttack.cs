using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Damage Source", menuName = "Damage Sources/Basic Attacks")]
public class BasicAttack : DamageSource
{
    [field: SerializeField] public TargetingData TargetingData;
}