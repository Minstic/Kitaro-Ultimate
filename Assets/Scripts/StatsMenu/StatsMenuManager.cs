using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Spine.Unity;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class StatsMenuManager : MonoBehaviour
{
    public Image statsIcon;
    public Image previousIcon;
    private StatsObject stats;
    public TMP_Text ID;
    public TMP_Text rarity;
    public TMP_Text selfTrait;
    public TMP_Text targetTrait;
    public TMP_InputField LVL;
    public TMP_Text HP;
    public TMP_Text cost;
    public TMP_Text CD;
    public TMP_Text speed;
    public TMP_Text atk;
    public TMP_Text range;
    public TMP_Text area;
    public TMP_Text DPS;
    public TMP_Text atkFrequency;
    public TMP_Text TBA;
    public TMP_Text preAtk;
    public TMP_Text postAtk;
    public TMP_Text roles;
    public TMP_Text type;
    public TMP_InputField magnifications;
    private SkeletonDataAsset anim;
    private float atkFrequencyValue;
    public Toggle enemyToggle;
    private int level = 30;
    private int plus = 0;
    private float magHP = 100;
    private float magAtk = 100;
    public GameObject effectRowPrefab;
    public Transform statsTable;

    void OnEnable()
    {
        statsIcon.sprite = previousIcon.sprite;
        stats = Resources.Load<StatsObject>("Stats/" + statsIcon.sprite.name);
        anim = Resources.Load<SkeletonDataAsset>("Animations/" + statsIcon.sprite.name + "/" + statsIcon.sprite.name + "_SkeletonData");
        DisplayStats();
    }

    void DisplayStats()
    {
        ID.text = stats.name;
        rarity.text = stats.rarity.ToString();
        selfTrait.text = stats.SelfTrait.ToString();
        targetTrait.text = stats.TargetTraits.ToString();
        cost.text = stats.Cost.ToString();
        CD.text = stats.CD.ToString();
        speed.text = stats.Speed.ToString();
        cost.text = stats.Cost.ToString();
        range.text = stats.AtkRange + " (" + stats.MinLD + " - " + stats.MaxLD + ")";
        area.text = stats.AreaAtk? "Yes" : "No";
        TBA.text = stats.TBA.ToString();
        roles.text = stats.UnitRole.ToString();
        type.text = stats.SecondaryType.ToString();
        Spine.EventTimeline eventTimeline = GetAnimationEventTimeline();
        preAtk.text = "";
        postAtk.text = "";
        atkFrequencyValue = 0;
        for (int i = 0; i < eventTimeline.FrameCount; i++)
        {
            float postAtkTemp = anim.GetSkeletonData(true).FindAnimation("attack").duration - eventTimeline.Frames[i];
            preAtk.text += eventTimeline.Frames[i];
            preAtk.text += i!=eventTimeline.FrameCount-1? "/":"";
            postAtk.text = postAtkTemp.ToString();
            atkFrequencyValue = eventTimeline.Frames[i];
        }
        atkFrequencyValue += stats.TBA;
        atkFrequency.text = atkFrequencyValue.ToString();
        LVL.text = $"LvL{level} +{plus}";
        magnifications.text = $"{magHP}%/{magAtk}%";
        foreach (Transform child in statsTable)
        {
            if (child.name == "EffectRow")
                Destroy(child.gameObject);
        }
        CalcMagnification();
        EffectDisplay();
    }

    public void UpdateLvl(string text)
    {
        string[] parts = text.Split(new char[] { ' ', '/' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
		    parts[i] = new string(parts[i].Where(c => char.IsDigit(c) || c == '.').ToArray());
        if (parts.Length == 1 && int.TryParse(parts[0], out int value))
        {
            if (text.StartsWith("/")||text.StartsWith(" "))
                plus = value<0? 0 : value;
            else
                level = value<0? 30 : value;
        }
        else if (parts.Length >= 2)
        {
            int.TryParse(parts[0], out level);
            int.TryParse(parts[1], out plus);
        }
        level = level>stats.MaxLevel? stats.MaxLevel : level;
        plus = plus>stats.MaxPlusLevel? stats.MaxPlusLevel : plus;
        LVL.text = $"LvL{level} +{plus}";
    }


    public void UpdateMagnification(string text)
{
	text = text.Replace(',', '.');
	string[] parts = text.Split(new char[] { ' ', '/' }, System.StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < parts.Length; i++)
		parts[i] = new string(parts[i].Where(c => char.IsDigit(c) || c == '.').ToArray());
	if (parts.Length == 1 && float.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
	{
		if (text.StartsWith("/") || text.StartsWith(" "))
			magAtk = value<0? 100 : value;
		else
			magHP = value<0? 100 : value;
	}
	else if (parts.Length >= 2)
	{
		float.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out magHP);
		float.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out magAtk);
	}
	magnifications.text = $"{magHP}%/{magAtk}%";
    CalcMagnification();
}

    public void CalcMagnification()
    {
        if (enemyToggle.isOn)
        {
            DisplayHPAtk(stats.HP, stats.Atk, stats.Atk2, stats.Atk3, magHP, magAtk);
        }
        else
        {
            DisplayHPAtk(stats.HP, stats.Atk, stats.Atk2, stats.Atk3, stats.HPMaginificationLvl30, stats.AtkMaginificationLvl30);
        }
    }

    Spine.EventTimeline GetAnimationEventTimeline()
    {
        var animationFound = anim.GetSkeletonData(true).FindAnimation("attack");
        var eventTimeline = animationFound.Timelines.OfType<Spine.EventTimeline>().FirstOrDefault();
        return eventTimeline;
    }

    void DisplayHPAtk(int actualHP, int actualAtk, int actualAtk2, int actualAtk3, float magHP, float magAtk)
    {
        actualHP = Mathf.FloorToInt(actualHP * magHP/100);
        actualAtk = Mathf.FloorToInt(actualAtk * magAtk/100);
        actualAtk2 = Mathf.FloorToInt(actualAtk2 * magAtk/100);
        actualAtk3 = Mathf.FloorToInt(actualAtk3 * magAtk/100);     
        HP.text = actualHP + "/" + stats.HB;
        atk.text = actualAtk.ToString();
        atk.text += actualAtk2>0 ? "/" + actualAtk2 : "";
        atk.text += actualAtk3>0 ? "/" + actualAtk3 : "";
        DPS.text = Mathf.CeilToInt((actualAtk + actualAtk2 + actualAtk3)/atkFrequencyValue).ToString();
    }

    void EffectDisplay()
    {
        List<string> effects = new List<string>();;
        foreach (AbilityData ability in stats.TraitAbilityData)
        {
            effects.Add(GenerateAbilityDescription(ability));
        }
        foreach (PassiveAbilityData ability in stats.NeutralAbilityData)
        {
            effects.Add(GeneratePAbilityDescription(ability));
        }
        foreach (BoostStats boostedStat in stats.StatsBoost)
        {
            effects.Add(GenerateStatBoostDescription(boostedStat));
        }
        int count = 0;
        for (int i = 0; i < Mathf.CeilToInt(effects.Count/3f); i++)
        {
            GameObject effectRowInstance = Instantiate(effectRowPrefab, statsTable);
            effectRowInstance.name = "EffectRow";
            EffectModule effectModule = effectRowInstance.GetComponent<EffectModule>();
            count++;
            effectModule.effectDescription1 = effects.Count>=count? effects[count-1] : "";
            count++;
            effectModule.effectDescription2 = effects.Count>=count? effects[count-1] : "";
            count++;
            effectModule.effectDescription3 = effects.Count>=count? effects[count-1] : "";
            effectModule.DrawEffects();
        }
    }

    string GenerateAbilityDescription(AbilityData ability)
    {
        string description = ""; 
        int percent = ability.Percent;
        int plusUnlock = ability.UnlockAtPlus;
        int power = ability.Power;
        float duration = ability.DurationSeconds;
        int distance = ability.Distance;
        switch (ability.Ability)
        {
            case StatsObject.TargetAbilities.Strong: description = $"|+{plusUnlock} | Strong against (x1.5)"; break;
            case StatsObject.TargetAbilities.MassiveDmg: 
                power = power!=0? power : 300; 
                description = $"|+{plusUnlock} | Massive Damage against (x{power}%)."; break;
            case StatsObject.TargetAbilities.Resist: 
                power = power!=0? power : 400; 
                description = $"|+{plusUnlock} | Resist trait (x{power}%)."; break;
            case StatsObject.TargetAbilities.AtkEnemyTeamNerf: 
                description = $"|+{plusUnlock} | Perma Weakens all the enemy targets by {power}%."; break;
            case StatsObject.TargetAbilities.Weaken: 
                power = power!=0? power : 50; 
                description = $"|+{plusUnlock} | {percent}% to Weaken to {power}% for {duration} seconds."; break;
            case StatsObject.TargetAbilities.Freeze: description = $"|+{plusUnlock} | {percent}% to Freeze for {duration} seconds."; break;
            case StatsObject.TargetAbilities.Slow: 
                power = power!=0? power : 3;
                description = $"|+{plusUnlock} | {percent}% to Slow 1/{power} of speed for {duration} seconds."; break;
            case StatsObject.TargetAbilities.KB: 
                duration = duration!=0? duration : 0.3f;
                distance = distance!=0? distance : 165;
                description = $"|+{plusUnlock} | {percent}% to Knock Back a distance of {distance} for {duration} seconds."; break;
            case StatsObject.TargetAbilities.Seal: description = $"|+{plusUnlock} | {percent}% to Seal for {duration} seconds."; break;
            case StatsObject.TargetAbilities.Metastasis: description = $"|+{plusUnlock} | {percent}% to Warp a distance of {distance} for {duration} seconds."; break;
            case StatsObject.TargetAbilities.TargetOnly: description = $"|+{plusUnlock} | Only attacks certain targets."; break;
            case StatsObject.TargetAbilities.Toxic: description = $"|+{plusUnlock} | {percent}% to Toxic {power}% HP from the target."; break;
        }
        return description;
    }

    string GeneratePAbilityDescription(PassiveAbilityData ability)
    {
        string description = ""; 
        int percent = ability.Percent;
        int plusUnlock = ability.UnlockAtPlus;
        int power = ability.Power;
        float duration = ability.DurationSeconds;
        int distance = ability.Distance;
        int numberofTimes = ability.NumberOfTimes;
        string unitID = ability.UnitID;
        string type = ability.subTarget.ToString();
        switch (ability.Ability)
        {
            case StatsObject.NeutralAbilities.AtkTeamBuff: description = $"|+{plusUnlock} | Boost all units Atk stat by {power}%."; break;
            case StatsObject.NeutralAbilities.TimeAlive: description = $"|+{plusUnlock} | Dies after {duration} seconds."; break;
            case StatsObject.NeutralAbilities.Barrier: description = $"|+{plusUnlock} | Has a Barrier."; break;
            case StatsObject.NeutralAbilities.BarrierBreaker: description = $"|+{plusUnlock} | {percent}% to break the enemy Barrier."; break;
            case StatsObject.NeutralAbilities.Shadowing: description = $"|+{plusUnlock} | Turns into a Shadow and travels {distance} Distance {numberofTimes} Times."; break;
            case StatsObject.NeutralAbilities.Survive: description = $"|+{plusUnlock} | {percent}% to survive a Lethal Attack."; break;
            case StatsObject.NeutralAbilities.Reincarnation: description = $"|+{plusUnlock} | Reincarnates into {unitID} after dying."; break;
            case StatsObject.NeutralAbilities.Crit: power = power!=0? power : 100; description = $"|+{plusUnlock} | {percent}% to deal a Critical Hit with {power}% extra damage."; break;
            case StatsObject.NeutralAbilities.Wave: description = $"|+{plusUnlock} | {percent}% to release a Wave of LvL {power} after attacking."; break;
            case StatsObject.NeutralAbilities.Berserk: description = $"|+{plusUnlock} | Boosts attack by {power}% when at {percent}% HP."; break;
            case StatsObject.NeutralAbilities.DeathExplosion: description = $"|+{plusUnlock} | Explodes after dying and deals {power}% Damage."; break;
            case StatsObject.NeutralAbilities.Yabayaba: description = $"|+{plusUnlock} | After killing an enemy, generate a YabaYaba of {ability.yabaSize} size with {ability.yabaTrait} Trait."; break;
            case StatsObject.NeutralAbilities.DoubleBounty: power = power!=0? power : 100; description = $"|+{plusUnlock} | After killing an enemy you get {power}% more spirit."; break;
            case StatsObject.NeutralAbilities.WaveBlocker: description = $"|+{plusUnlock} | {percent}% chance to nullify waves."; break;
            case StatsObject.NeutralAbilities.ReduceWaveDMG: description = $"|+{plusUnlock} | Reduce wave Dmg by {percent}%."; break;
            case StatsObject.NeutralAbilities.WeakenReduction: description = $"|+{plusUnlock} | Reduce the Weaken effect duration by {percent}%."; break;
            case StatsObject.NeutralAbilities.SlowReduction: description = $"|+{plusUnlock} | Reduce the Slow effect duration by {percent}%."; break;
            case StatsObject.NeutralAbilities.SealReduction: description = $"|+{plusUnlock} | Reduce the Seal effect duration by {percent}%."; break;
            case StatsObject.NeutralAbilities.ToxicReduction: description = $"|+{plusUnlock} | Reduce the Toxic effect power by {percent}%."; break;
            case StatsObject.NeutralAbilities.FreezeReduction: description = $"|+{plusUnlock} | Reduce the Freeze effect duration by {percent}%."; break;
            case StatsObject.NeutralAbilities.MetastasisReduction: description = $"|+{plusUnlock} | Reduce the Metastasis (Warp) effect distance by {percent}%."; break;
            case StatsObject.NeutralAbilities.KBReduction: description = $"|+{plusUnlock} | Reduce the Knockback effect distance by {percent}%."; break;
            case StatsObject.NeutralAbilities.YabayabaImmune: description = $"|+{plusUnlock} | Immune to the YabaYaba effect."; break;
            case StatsObject.NeutralAbilities.TypeKiller: power = power!=0? power : 100; description = $"|+{plusUnlock} | Deals {power}% extra damage to {type} enemies."; break;
            case StatsObject.NeutralAbilities.TypeKillerUp: power = power!=0? power : 100; description = $"|+{plusUnlock} | Increases Atk to all allies by {power}% to {type} enemies."; break;
            case StatsObject.NeutralAbilities.AtkUpType: description = $"|+{plusUnlock} | All allies who are {type} receive a {power}% Atk boost."; break;
            case StatsObject.NeutralAbilities.HPUpType: description = $"|+{plusUnlock} | All allies who are {type} receive a {power}% HP boost."; break;
            case StatsObject.NeutralAbilities.CritUpType: description = $"|+{plusUnlock} | All allies who are {type} receive a {power}% Critical chance boost."; break;
            case StatsObject.NeutralAbilities.SpeedUpType: description = $"|+{plusUnlock} | All allies who are {type} receive a {power} Speed boost."; break;
            case StatsObject.NeutralAbilities.CDReductionType: description = $"|+{plusUnlock} | All allies who are {type} receive a {power} seconds Cooldown reduction."; break;
            case StatsObject.NeutralAbilities.CannonStartUp: description = $"|+{plusUnlock} | Cannon is recharged by {percent}% at the start of the battle."; break;
            case StatsObject.NeutralAbilities.SoulMaxUp: description = $"|+{plusUnlock} | Your wallet has +{power}% max Souls."; break;
            case StatsObject.NeutralAbilities.SoulStart: description = $"|+{plusUnlock} | Your wallet starts with {power} extra Souls."; break;
            case StatsObject.NeutralAbilities.SoulLvlStart: description = $"|+{plusUnlock} | Your wallet starts with +{power} extra levels."; break;
            case StatsObject.NeutralAbilities.CannonReduction: description = $"|+{plusUnlock} | Cannon Damage is reduced by {percent}%."; break;
            case StatsObject.NeutralAbilities.SurgeImmune: description = $"|+{plusUnlock} | Immune to Secret Arts."; break;
            case StatsObject.NeutralAbilities.Surge: description = $"|+{plusUnlock} | {percent}% to release a Secret Art with {power} Atk for {duration} seconds after attacking."; break;
        }
        return description;
    }

    string GenerateStatBoostDescription(BoostStats statBoost)
    {
        string description = ""; 
        int plusUnlock = statBoost.UnlockAtPlus;
        int boost = statBoost.StatBoost;
        switch (statBoost.stat)
        {
            case StatsObject.Stat.HP: description = $"|+{plusUnlock} | Boosts HP by {boost}."; break;
            case StatsObject.Stat.Atk: description = $"|+{plusUnlock} | Boosts Atk by {boost}."; break;
            case StatsObject.Stat.CD: description = $"|+{plusUnlock} | Lowers Cooldown by {boost}."; break;
            case StatsObject.Stat.Speed: description = $"|+{plusUnlock} | Boosts Speed by {boost}."; break;
            case StatsObject.Stat.Range: description = $"|+{plusUnlock} | Boosts Standing Range by {boost}."; break;
            case StatsObject.Stat.Cost: description = $"|+{plusUnlock} | Lowers cost by {boost}."; break;
            case StatsObject.Stat.HB: description = $"|+{plusUnlock} | Increases HB count by {boost}."; break;
        }
        return description;
    }
}