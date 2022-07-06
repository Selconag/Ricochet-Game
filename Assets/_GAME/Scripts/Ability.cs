using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum AbilityTypes { BulletBounce, BulletPierce, PlayerAttackSpeed, RainbowMode, BulletLifeTime, Portals }

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities", order = 1)]
public class Ability : ScriptableObject
{
    public AbilityTypes AbilityType;
    public Sprite AbilityImage;
    public string AbilityText;
    public bool Activated = false, OneTimeUse = true;

}
