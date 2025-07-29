using System.Collections.Generic;
using UnityEngine;
using static StatsObject;

[CreateAssetMenu(fileName = "New Unit", menuName = "Create Unit Stats")]
public class StatsObject : ScriptableObject
{
    [System.Flags]
    public enum Traits {Normal = 0, Gorgeous = 1 << 0, Hercules = 1 << 1, Amaterasu = 1 << 2, Abyss = 1 << 3, Metal = 1 << 4, Serious = 1 << 5, Death = 1 << 6, Virus = 1 << 7, Noise = 1 << 8, Base = 1 << 9, EveryTrait = 1 << 10}
    public enum TargetAbilities {Strong, MassiveDmg, Resist, AtkEnemyTeamNerf, Weaken, Freeze, Slow, KB, Seal, Metastasis, TargetOnly, Toxic}
    public enum NeutralAbilities {AtkTeamBuff, TimeAlive, Barrier, Shadowing, Survive, Reincarnation, Crit, Wave, Berserk, DeathExplosion, Yabayaba, DoubleBounty, BarrierBreaker, WaveBlocker, ReduceWaveDMG, WeakenReduction, FreezeReduction, SlowReduction, KBReduction, YabayabaImmune, SealReduction, MetastasisReduction, ToxicReduction, TypeKiller, TypeKillerUp, AtkUpType, HPUpType, CritUpType, SpeedUpType, CDReductionType, CannonStartUp, SoulLvlStart, SoulStart, SoulMaxUp, Surge, SurgeImmune, CannonReduction}
    [System.Flags]
    public enum SubTypes {None = 0, Ghost = 1 << 0, Cat = 1 << 1, SnowWoman = 1 << 2, Baketanuki = 1 << 3, FoxSpirit = 1 << 4, Tengu = 1 << 5, Kappa = 1 << 6, Dragon = 1 << 7, Kidoshu = 1 << 8, Demons = 1 << 9, Vampire = 1 << 10, SouthernYokai = 1 << 11, WesternYokai = 1 << 12, Water = 1 << 13, Fire = 1 << 14, Nature = 1 << 15, Flying = 1 << 16, Spirit = 1 << 17, Beast = 1 << 18, Tsukumogami = 1 << 19, YuruFamily = 1 << 20, HellsKey = 1 << 21, SixthAnime = 1 << 22, YokaiRonin = 1 << 23, Cyber = 1 << 24, FourGenerals = 1 << 25, BattleCat = 1 << 26, YokaiWatch = 1 << 27}
    public enum Rarity {Normal, SP, Rare, SuperRare, UberRare, LegendRare, Enemy}
    [System.Flags]
    public enum Roles {None = 0, Attacker = 1 << 0, LongDistance = 1 << 1, Tank = 1 << 2, MassProduction = 1 << 3, Support = 1 << 4, Unique = 1 << 5, Balance = 1 << 6}
    public enum Stat {HP, HB, Speed, Atk, Range, Cost, CD}
    public enum Element {Dark, Fire, Water, Nature, Light, Cyber, Poison}
    
    [Header("Basic Stats")]
    [Min(0)]
    public int HP;
    [Min(1)]
    public int HB = 1;
    [Min(0)]
    public int Speed;
    [Min(1)]
    public int AtkRange;
    [Min(0)]
    public int MinLD;
    [Min(0)]
    public int MaxLD;

    [Header("Attacking Stats")]
    public int Atk;
    public int Atk2;
    public int Atk3;
    public bool AreaAtk = false;
    [Min(0)]
    public float TBA;
    public bool AttacksAllies = false;

    [Header("Unit Stuff")]
    public Rarity rarity = Rarity.Normal;
    [Min(0)]
    public int Cost;
    [Min(0)]
    public float CD;
    public Roles UnitRole = Roles.None;
    public int MaxLevel = 40;
    public int MaxPlusLevel = 80;
    [Min (0)]
    public float HPMaginificationLvl30 = 100;
    [Min (0)]
    public float AtkMaginificationLvl30 = 100;

    [Header("Visuals")]
    [Range(0,100)]
    public int MinLayer = 0;
    [Range(0,100)]
    public int MaxLayer = 100;
    public float animOffset = 0.5f;
    
    [Header("Traits")]
    public Traits SelfTrait = Traits.Normal;
    public Traits TargetTraits = Traits.Normal;
    public SubTypes SecondaryType = SubTypes.None;

    [Header("Abilities")]
    public List<AbilityData> TraitAbilityData;
    public List<PassiveAbilityData> NeutralAbilityData;
    public List<BoostStats> StatsBoost;
}
[System.Serializable]
public class AbilityData
{
    [Min(0)]
    public int UnlockAtPlus = 0;
    public TargetAbilities Ability;
    [Range(0,100)]
    public int Percent;
    [Min(0)]
    public int Power;
    [Min(0)]
    public float DurationSeconds;
    public int Distance;
}
[System.Serializable]
public class PassiveAbilityData
{
    [Min(0)]
    public int UnlockAtPlus = 0;
    public NeutralAbilities Ability;
    [Range(0,100)]
    public int Percent;
    [Min(0)]
    public int Power;
    [Min(0)]
    public float DurationSeconds;
    public int Distance;
    [Min(-1)]
    public int NumberOfTimes = 0;
    public string UnitID;
    public enum YabaSize {Sm, M, L, King}
    public enum YabaTrait {Normal, Hercules, Metal}
    public YabaSize yabaSize = YabaSize.Sm;
    public YabaTrait yabaTrait = YabaTrait.Normal;
    public SubTypes subTarget = SubTypes.None;
    public Element SurgeElement = Element.Dark;
    public List<AbilityData> SurgeAbilities;
}

[System.Serializable]
public class BoostStats
{
    [Min(1)]
    public int UnlockAtPlus = 1;
    public Stat stat;
    public int StatBoost;
}