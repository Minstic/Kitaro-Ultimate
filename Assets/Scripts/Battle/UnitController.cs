using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public StatsObject Stats;
    [HideInInspector] public bool IsEnemy = false;
    private Rigidbody2D Rb;
    private Vector2 Speed;
    private SkeletonAnimation Anim;
    private int Layer;
    private LayerMask EnemyLayer;
    private bool Inrange = false;
    private Vector2 direccion;
    private bool AtkCD = false;
    private float TBA;
    private float AtkRange;
    private int HP;
    private int maxHP;
    private int Atk;
    private int Atk2;
    private int Atk3;
    private RaycastHit2D SingleCast;
    private RaycastHit2D[] AreaCast = null;
    [HideInInspector] public bool IsKnockedBack = false;
    private int KnockbackCount;
    private Coroutine CurrentAttack = null;
    private string SkinName;
    private MoneyManager moneyManager;
    private SpawnUnits unitLimit;
    private AudioController audioManager;
    private float postAtk;
    [SerializeField] private BoxCollider2D LDHitbox;
    private bool hasLD = false;
    private List<string> neutralAbilityList = new();
    private bool hasNeutralAbilities = false;
    public Animator hitAnim;
    public GameObject abilityParent;
    private List<int> activeEffects = new();
    private Coroutine freezeCoroutine;
    private Coroutine slowCoroutine;
    private Coroutine weakenCoroutine;
    private float currentPower = 1;
    private float traitDmgMultiplier = 1;
    private float traitHPMultiplier = 1;
    private float auraBuff = 0;
    private float auraNerf = 0;
    private float berserkDmg = 0; 

    void Start()
    {
        moneyManager = FindFirstObjectByType<MoneyManager>();
        unitLimit = FindFirstObjectByType<SpawnUnits>();
        audioManager = FindFirstObjectByType<AudioController>();
        HP = Stats.HP;
        maxHP = HP;
        Atk = Stats.Atk;
        Atk2 = Stats.Atk2;
        Atk3 = Stats.Atk3;
        Rb = GetComponent<Rigidbody2D>();
        float FloatSpeed = Stats.Speed;
        Speed = new Vector2(FloatSpeed / 25f, 0);
        Layer = Random.Range(Stats.MinLayer, Stats.MaxLayer);
        TBA = Stats.TBA;
        AtkRange = Stats.AtkRange / 500f;
        if (Stats.MaxLD != 0)
        {
            LDHitbox.size = new Vector2((Stats.MaxLD - Stats.MinLD) / 500f, 0.1f);
            LDHitbox.offset = new Vector2((Stats.MaxLD + Stats.MinLD) / (IsEnemy ? 1000f : -1000f), 0);
            hasLD = true;
        }
        LDHitbox.enabled = hasLD;
        GameObject animObj = new GameObject("SkeletonAnimation");
        animObj.transform.SetParent(transform);
        Anim = animObj.AddComponent<SkeletonAnimation>();
        VerifyifEnemy(animObj, IsEnemy);
        Anim.skeletonDataAsset = SearchforAnim(Stats.name);
        MeshRenderer meshRenderer = Anim.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = Layer;
        LoadAnimWithSkin();
        Anim.AnimationState.SetAnimation(0, "move", true);
        Anim.AnimationState.Event += OnAnimationEvent;
        NeutralAbilityCheck();
        CheckAura();
    }
    void FixedUpdate()
    {
        if (!Inrange && Anim.AnimationState.GetCurrent(0).Animation.Name == "move" && !IsKnockedBack)
        {
            Rb.velocity = Speed * -1;
        }
        else if (Inrange && !IsKnockedBack)
        {
            Rb.velocity = Vector2.zero;
            Attack();
        }
        Vector2 origen = transform.position;

        Inrange = false;

        if (Stats.AreaAtk)
        {
            AreaCast = Physics2D.RaycastAll(origen, direccion, AtkRange, EnemyLayer);
            foreach (RaycastHit2D hit in AreaCast)
            {
                if (hit.collider != null && ValidTarget(hit.collider.gameObject))
                {
                    Inrange = true;
                    break;
                }
            }
        }
        else
        {
            SingleCast = Physics2D.Raycast(origen, direccion, AtkRange, EnemyLayer);
            if (SingleCast.collider != null && ValidTarget(SingleCast.collider.gameObject))
            {
                Inrange = true;
            }
        }

        Debug.DrawRay(origen, direccion * AtkRange, Color.red);
    }


    public void VerifyifEnemy(GameObject animObj, bool Enemy)
    {
        if (Enemy)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            Anim.GetComponent<MeshRenderer>().sortingLayerName = "Enemy";
            animObj.transform.localPosition = new Vector3(-Stats.animOffset, (float)(100 - Layer) / 300, 0);
            animObj.transform.localScale = new Vector3(-1, 1, 1);
            abilityParent.transform.localPosition = new Vector3(0, (float)(100 - Layer) / 300, 0);
            Speed *= -1;
            EnemyLayer = 1 << LayerMask.NameToLayer("Unit");
            direccion = Vector2.right;
            SkinName = "Reverse";
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Unit");
            Anim.GetComponent<MeshRenderer>().sortingLayerName = "Unit";
            animObj.transform.localPosition = new Vector3(Stats.animOffset, (float)(100 - Layer) / 300, 0);
            abilityParent.transform.localPosition = new Vector3(0, (float)(100 - Layer) / 300, 0);
            EnemyLayer = 1 << LayerMask.NameToLayer("Enemy");
            direccion = Vector2.left;
            SkinName = "Normal";
            HP = Mathf.FloorToInt(HP * Stats.HPMaginificationLvl30 / 100);
            maxHP = HP;
            Atk = Mathf.FloorToInt(Atk * Stats.AtkMaginificationLvl30 / 100);
            Atk2 = Mathf.FloorToInt(Atk2 * Stats.AtkMaginificationLvl30 / 100);
            Atk3 = Mathf.FloorToInt(Atk3 * Stats.AtkMaginificationLvl30 / 100);
        }
    }
    void Attack()
    {
        if (!AtkCD && !IsKnockedBack)
        {
            AtkCD = true;
            CurrentAttack = StartCoroutine(AttackSequence());
        }
    }

    IEnumerator AttackSequence()
    {
        Anim.AnimationState.SetAnimation(0, "attack", false);
        yield return new WaitForSpineAnimationComplete(Anim.AnimationState.GetCurrent(0));
        float waitTime = TBA - postAtk;
        float elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            if (Inrange && Anim.AnimationState.GetCurrent(0).Animation.Name != "wait" && !IsKnockedBack)
            {
                Anim.AnimationState.SetAnimation(0, "wait", true);
            }
            else if (!Inrange && Anim.AnimationState.GetCurrent(0).Animation.Name != "move" && !IsKnockedBack)
            {
                Anim.AnimationState.SetAnimation(0, "move", true);
            }
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;
        }
        if (Inrange && !IsKnockedBack)
        {
            AtkCD = false;
            Attack();
        }
        else if (!IsKnockedBack)
        {
            Anim.AnimationState.SetAnimation(0, "move", true);
        }
        AtkCD = false;
    }
    void OnAnimationEvent(TrackEntry trackEntry, Spine.Event EventName)
    {
        int critPower = CalcCritAbility();
        postAtk = trackEntry.animation.duration - EventName.time;
        int AtkTemp = Atk;
        if (EventName.data.name == "Hit_2")
        {
            AtkTemp = Atk2;
        }
        else if (EventName.data.name == "Hit_3")
        {
            AtkTemp = Atk3;
        }
        AtkTemp = (int)(AtkTemp * currentPower);
        if (berserkDmg > 0) AtkTemp += (int)(AtkTemp * (berserkDmg/100f));
        string yabaID = YabaCheck();
        int drop = CheckExtraMoney();
        if (hasLD)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(LDHitbox.bounds.center, LDHitbox.size, 0f, EnemyLayer);
            if (colliders.Length > 0)
            {
                audioManager.PlayHit();
                foreach (Collider2D hit in colliders)
                {
                    if (hit != null && ValidTarget(hit.gameObject))
                    {
                        UnitController enemy = hit.GetComponent<UnitController>();
                        if (enemy)
                        {
                            AtkTemp = CalcDefense(AtkTemp, enemy.Stats);
                            enemy.Damage(AtkTemp, yabaID, critPower, drop, Stats);
                        }
                    }
                }
            }
        }
        else
        {
            if (Stats.AreaAtk)
            {
                if (AreaCast.Length > 0)
                {
                    audioManager.PlayHit();
                    foreach (RaycastHit2D hit in AreaCast)
                    {
                        if (hit.collider != null && ValidTarget(hit.collider.gameObject))
                        {
                            UnitController enemy = hit.collider.GetComponent<UnitController>();
                            if (enemy)
                            {
                                AtkTemp = CalcDefense(AtkTemp, enemy.Stats);
                                enemy.Damage(AtkTemp, yabaID, critPower, drop, Stats);
                            }
                        }
                    }
                }
            }
            else
            {
                if (SingleCast.collider != null && ValidTarget(SingleCast.collider.gameObject))
                {
                    audioManager.PlayHit();
                    UnitController enemy = SingleCast.collider.GetComponent<UnitController>();
                    if (enemy)
                    {
                        AtkTemp = CalcDefense(AtkTemp, enemy.Stats);
                        enemy.Damage(AtkTemp, yabaID, critPower, drop, Stats);
                    }
                }
            }
        }
        if (critPower > 0)
        {
            audioManager.PlayAbilitySound("039_critical");
        }
    }

    SkeletonDataAsset SearchforAnim(string AnimName)
    {
        string UnitPath = "Animations/" + AnimName + "/" + AnimName;
        SkeletonDataAsset UnitAnim = Resources.Load<SkeletonDataAsset>(UnitPath + "_SkeletonData");
        TextAsset SKJson = Resources.Load<TextAsset>(UnitPath);

        Anim.skeletonDataAsset = UnitAnim;
        Anim.skeletonDataAsset.skeletonJSON = SKJson;

        return UnitAnim;
    }
    public void Damage(int Dmg, string yabaID, int critPower, int drop, StatsObject attackerStats)
    {
        if (TraitCheck(attackerStats) || IsEnemy == false)
        {
            foreach (AbilityData ability in attackerStats.TraitAbilityData)
            {
                ApplyAbility(ability);
            }
        }
        if ((Stats.SelfTrait & StatsObject.Traits.Metal) != 0 && critPower == 0)
        {
            HP--;
            hitAnim.Play("Hit", 0, 0f);
        }
        else
        {
            if (critPower != 0)
            {
                Dmg = (int)(Dmg * (critPower / 100f + 1));
                hitAnim.Play("Crit", 0, 0f);
            }
            else
            {
                hitAnim.Play("Hit", 0, 0f);
            }
            Dmg = (int)(Dmg * traitDmgMultiplier);
            HP -= Dmg;
        }
        if (HP <= 0)
        {
            StartCoroutine(KB(true, yabaID, drop));
            return;
        }

        int expectedKnockbacks = Stats.HB - Mathf.CeilToInt((float)HP / (maxHP / Stats.HB));
        if (expectedKnockbacks > KnockbackCount)
        {
            KnockbackCount++;
            StartCoroutine(KB(false, null, 0));
        }
        berserkDmg = BerserkCheck();
        traitDmgMultiplier = 1;
    }

    public void CannonAttack(int Dmg)
    {
        if ((Stats.SelfTrait & StatsObject.Traits.Metal) != 0)
        {
            HP--;
        }
        else
        {
            HP -= Dmg;
        }
        if (HP <= 0)
        {
            StartCoroutine(KB(true, null, 0));
            return;
        }
        StartCoroutine(KB(false, null, 0));
    }
    IEnumerator KB(bool death, string yabaID, int drop)
    {
        audioManager.PlayHB();
        IsKnockedBack = true;
        Rb.velocity = Vector2.zero;

        Anim.AnimationState.SetAnimation(0, "damage", true);
        float explosionPower = CheckExplosionPower();
        if (death && explosionPower > 0) Explosion(explosionPower);
        Rb.AddForce(direccion * -2f, ForceMode2D.Impulse);
        if (death && SurviveCheck())
        {
            HP = 1;
            AddEffect(1);
            death = false;
            neutralAbilityList.Remove("Survive");
        }
        yield return new WaitForSeconds(0.5f);

        RemoveEffect(1);
        if (death)
        {
            if (IsEnemy)
            {
                GiveDeathcash(drop);
            }
            AuraRestore(IsEnemy);
            if (auraBuff > 0)
            {
                if (IsEnemy) TeamAbilities.atkPowerEnemies -= auraBuff;
                else TeamAbilities.atkPowerUnits -= auraBuff;
                AuraRestore(IsEnemy);
            }
            if (auraNerf > 0)
            {
                if (IsEnemy)
                {
                    TeamAbilities.atkPowerUnits += auraNerf;
                }
                else
                {
                    TeamAbilities.atkPowerTraitEnemies += auraNerf;
                }
                AuraRestore(!IsEnemy);
            }
            unitLimit.DecreaseLimitCounter(IsEnemy, this);
            ReincarnationCheck(yabaID);
            audioManager.PlayAbilitySound("012_die");
            Destroy(gameObject);
            yield break;
        }
        if (CurrentAttack != null)
        {
            StopCoroutine(CurrentAttack);
            CurrentAttack = null;
            AtkCD = false;
        }
        Anim.AnimationState.SetAnimation(0, "move", true);
        IsKnockedBack = false;
    }
    bool ValidTarget(GameObject Target)
    {
        if (Target.tag == "Base" || !Target.GetComponent<UnitController>().IsKnockedBack)
        {
            return true;
        }
        return false;
    }
    void LoadAnimWithSkin()
    {
        Anim.initialSkinName = "default";
        Anim.Initialize(true);
        bool skinExists = Anim.skeleton.Data.FindSkin(SkinName) != null;
        Anim.initialSkinName = skinExists ? SkinName : "default";
        Anim.Initialize(true);
    }

    bool RNGCalc(int percent)
    {
        int randomTemp = Random.Range(0, 100);
        if (percent >= randomTemp)
        {
            return true;
        }
        return false;
    }

    void GiveDeathcash(float drop)
    {
        if (drop != 0)
        {
            moneyManager.GiveMoney((int)(Stats.Cost / 2f * (drop / 100 + 1)));
            return;
        }
        moneyManager.GiveMoney(Stats.Cost / 2);
    }

    // Abilities

    public void AddEffect(int effect)
    {
        if (activeEffects.Contains(effect)) return;
        activeEffects.Add(effect);
        abilityParent.GetComponent<AbilityIcons>().UpdateIcons(activeEffects, IsEnemy);
    }

    public void RemoveEffect(int effect)
    {
        if (!activeEffects.Contains(effect)) return;
        activeEffects.Remove(effect);
        abilityParent.GetComponent<AbilityIcons>().UpdateIcons(activeEffects, IsEnemy);
    }

    void ApplyAbility(AbilityData ability)
    {
        if (!RNGCalc(ability.Percent) && ability.Percent > 0) return;
        float power = ability.Power;
        switch (ability.Ability)
        {
            case StatsObject.TargetAbilities.KB:
                StartCoroutine(KB(false, null, 0)); // TBD
                break;
            case StatsObject.TargetAbilities.Freeze:
                if (freezeCoroutine != null)
                {
                    StopCoroutine(freezeCoroutine);
                    freezeCoroutine = null;
                }
                freezeCoroutine = StartCoroutine(Freeze(ability.DurationSeconds));
                break;
            case StatsObject.TargetAbilities.Slow:
                if (power == 0) power = 3;
                if (slowCoroutine != null)
                {
                    StopCoroutine(slowCoroutine);
                    slowCoroutine = null;
                }
                slowCoroutine = StartCoroutine(Slow(ability.DurationSeconds, power));
                break;
            case StatsObject.TargetAbilities.Weaken:
                if (power == 0) power = 50;
                if (weakenCoroutine != null)
                {
                    StopCoroutine(weakenCoroutine);
                    weakenCoroutine = null;
                }
                weakenCoroutine = StartCoroutine(Weaken(ability.DurationSeconds, power));
                break;
            case StatsObject.TargetAbilities.Strong:
                traitDmgMultiplier += 0.5f;
                break;
            case StatsObject.TargetAbilities.MassiveDmg:
                if (power == 0) power = 300;
                traitDmgMultiplier += power / 100;
                break;
        }
    }

    void NeutralAbilityCheck()
    {
        foreach (PassiveAbilityData ability in Stats.NeutralAbilityData)
        {
            neutralAbilityList.Add(ability.Ability.ToString());
        }
        if (neutralAbilityList.Count != 0)
        {
            hasNeutralAbilities = true;
        }
    }
    void ReincarnationCheck(string yabaID)
    {
        if (hasNeutralAbilities && neutralAbilityList.Contains("Reincarnation"))
        {
            int index = neutralAbilityList.IndexOf("Reincarnation");
            unitLimit.SpawnUnit(Stats.NeutralAbilityData[index].UnitID, IsEnemy, transform);
        }
        else if (yabaID != null && !neutralAbilityList.Contains("YabayabaImmune"))
        {
            unitLimit.SpawnUnit(yabaID, !IsEnemy, transform);
        }
    }

    bool SurviveCheck()
    {
        if (hasNeutralAbilities && neutralAbilityList.Contains("Survive"))
        {
            int index = neutralAbilityList.IndexOf("Survive");
            if (RNGCalc(Stats.NeutralAbilityData[index].Percent))
            {
                return true;
            }
        }
        return false;
    }

    string YabaCheck()
    {
        if (hasNeutralAbilities && neutralAbilityList.Contains("Yabayaba"))
        {
            int index = neutralAbilityList.IndexOf("Yabayaba");
            string yabaID = FindYabaID(Stats.NeutralAbilityData[index]);
            return yabaID;
        }
        return null;
    }
    string FindYabaID(PassiveAbilityData data)
    {
        string yabaID = null;
        string yabaTrait = null;
        switch (data.yabaSize)
        {
            case PassiveAbilityData.YabaSize.Sm: yabaID = "Character_4001"; break;
            case PassiveAbilityData.YabaSize.M: yabaID = "Character_4002"; break;
            case PassiveAbilityData.YabaSize.L: yabaID = "Character_4003"; break;
            case PassiveAbilityData.YabaSize.King: yabaID = "Character_4104"; break;
        }
        switch (data.yabaTrait)
        {
            case PassiveAbilityData.YabaTrait.Normal: yabaTrait = "0"; break;
            case PassiveAbilityData.YabaTrait.Hercules: yabaTrait = "2"; break;
            case PassiveAbilityData.YabaTrait.Metal: yabaTrait = "5"; break;
        }
        yabaID += yabaTrait;
        return yabaID;
    }

    int CalcCritAbility()
    {
        if (hasNeutralAbilities && neutralAbilityList.Contains("Crit"))
        {
            int index = neutralAbilityList.IndexOf("Crit");
            if (RNGCalc(Stats.NeutralAbilityData[index].Percent))
            {
                return Stats.NeutralAbilityData[index].Power == 0 ? 100 : Stats.NeutralAbilityData[index].Power;
            }
        }
        return 0;
    }

    int CheckExtraMoney()
    {
        if (hasNeutralAbilities && neutralAbilityList.Contains("DoubleBounty"))
        {
            int index = neutralAbilityList.IndexOf("DoubleBounty");
            return Stats.NeutralAbilityData[index].Power == 0 ? 100 : Stats.NeutralAbilityData[index].Power;
        }
        return 0;
    }

    int BerserkCheck()
    {
        if (hasNeutralAbilities && neutralAbilityList.Contains("Berserk"))
        {
            int index = neutralAbilityList.IndexOf("Berserk");
            if (HP <= Stats.HP * (Stats.NeutralAbilityData[index].Percent / 100f))
            {
                AddEffect(0);    
                return Stats.NeutralAbilityData[index].Power;
            }
        }
        return 0;
    }

    float CheckExplosionPower()
    {
        if (hasNeutralAbilities && neutralAbilityList.Contains("DeathExplosion"))
        {
            int index = neutralAbilityList.IndexOf("DeathExplosion");
            return Stats.NeutralAbilityData[index].Power;
        }
        return 0;
    }
    void Explosion(float power)
    {
        foreach (RaycastHit2D hit in AreaCast)
        {
            if (hit.collider != null && ValidTarget(hit.collider.gameObject))
            {
                UnitController enemy = hit.collider.GetComponent<UnitController>();
                if (enemy)
                {
                    float AtkTemp = Atk * (power / 100);
                    AtkTemp = CalcDefense(AtkTemp, enemy.Stats);
                    enemy.Damage((int)AtkTemp, null, 0, 0, Stats);
                }
            }
        }
    }

    bool TraitCheck(StatsObject enemyStats)
    {
        if ((enemyStats.TargetTraits & Stats.SelfTrait) != 0 || ((enemyStats.TargetTraits & StatsObject.Traits.EveryTrait) != 0 && (enemyStats.TargetTraits & StatsObject.Traits.Base) == 0))
            return true;
        return false;
    }

    IEnumerator Freeze(float duration)
    {
        AddEffect(2);
        Anim.timeScale = 0;
        Speed = Vector2.zero;
        yield return new WaitForSeconds(duration);
        Anim.timeScale = 1;
        Speed = IsEnemy ? new Vector2(Stats.Speed / -25f, 0) : new Vector2(Stats.Speed / 25f, 0);
        RemoveEffect(2);
        RemoveEffect(3);
    }

    IEnumerator Slow(float duration, float power)
    {
        AddEffect(3);
        Anim.timeScale = 1 - power / 10;
        Speed = IsEnemy ? new Vector2(Stats.Speed / -25f * (1 - power / 10), 0) : new Vector2(Stats.Speed / 25f * (1 - power / 10), 0);
        yield return new WaitForSeconds(duration);
        Anim.timeScale = 1;
        Speed = IsEnemy ? new Vector2(Stats.Speed / -25f, 0) : new Vector2(Stats.Speed / 25f, 0);
        RemoveEffect(3);
    }
    IEnumerator Weaken(float duration, float power)
    {
        AddEffect(4);
        currentPower = (IsEnemy ? TeamAbilities.atkPowerEnemies : TeamAbilities.atkPowerUnits) - (1 - power / 100);
        yield return new WaitForSeconds(duration);
        currentPower = IsEnemy ? TeamAbilities.atkPowerEnemies : TeamAbilities.atkPowerUnits;
        RemoveEffect(4);
    }

    int CalcDefense(float Atk, StatsObject enemyStats)
    {
        if (TraitCheck(enemyStats))
        {
            foreach (AbilityData ability in enemyStats.TraitAbilityData)
            {
                switch (ability.Ability)
                {
                    case StatsObject.TargetAbilities.Strong:
                        traitHPMultiplier += 0.5f;
                        break;
                    case StatsObject.TargetAbilities.Resist:
                        float power = ability.Power;
                        if (power == 0) power = 400;
                        traitHPMultiplier += (power / 100) - 1;
                        break;
                }
            }
            Atk /= traitHPMultiplier;
            traitHPMultiplier = 1;
        }
        return (int)Atk;
    }

    void CheckAura()
    {
        if (neutralAbilityList.Contains("AtkTeamBuff"))
        {
            int index = neutralAbilityList.IndexOf("AtkTeamBuff");
            if (IsEnemy)
            {
                TeamAbilities.atkPowerEnemies += Stats.NeutralAbilityData[index].Power / 100f;
            }
            else
            {
                TeamAbilities.atkPowerUnits += Stats.NeutralAbilityData[index].Power / 100f;
            }
            auraBuff = Stats.NeutralAbilityData[index].Power / 100f;
            AuraRestore(IsEnemy);
        }
        UpdateAura();
        foreach (AbilityData ability in Stats.TraitAbilityData)
        {
            if (ability.Ability == StatsObject.TargetAbilities.AtkEnemyTeamNerf)
            {
                if (IsEnemy)
                {
                    TeamAbilities.atkPowerUnits -= ability.Power / 100f;
                }
                else
                {
                    TeamAbilities.atkPowerTraitEnemies -= ability.Power / 100f;
                    TeamAbilities.enemyTraitsAffected = Stats.TargetTraits;
                }
                auraNerf = ability.Power / 100f;
                AuraRestore(!IsEnemy);
            }
        }
    }
    void AuraRestore(bool enemy)
    {
        if (enemy)
        {
            foreach (UnitController unit in SpawnUnits.enemyList)
            {
                unit.UpdateAura();
            }
        }
        else
        {
            foreach (UnitController unit in SpawnUnits.unitList)
            {
                unit.UpdateAura();
            }
        }
    }
    void UpdateAura()
    {
        currentPower = IsEnemy ? TeamAbilities.atkPowerEnemies : TeamAbilities.atkPowerUnits;
        if (IsEnemy && (Stats.SelfTrait & TeamAbilities.enemyTraitsAffected) != 0) currentPower += TeamAbilities.atkPowerTraitEnemies;
        if (currentPower < 1)
        {
            AddEffect(6);
            RemoveEffect(5);
        }
        else if (currentPower > 1)
        {
            AddEffect(5);
            RemoveEffect(6);
        }
        else
        {
            RemoveEffect(6);
            RemoveEffect(5);
        }
        print(currentPower + " " + IsEnemy);
        if (currentPower < 0) currentPower = 0;
    }
}