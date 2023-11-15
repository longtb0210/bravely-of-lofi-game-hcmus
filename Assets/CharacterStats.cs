using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major stats")]
    public Stat strength;   // increase damge
    public Stat agility;    //increase evasion
    public Stat intelligence;   //increase magic damage
    public Stat vitality;   //increase health

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;  // default value 150%

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited; // do damage over time
    public bool isChilled; // reduce armor by 20%
    public bool isShocked; //reduce accuracy by 20%

    private float ignitedTimer;
    private float chillTimer;
    private float shockTimer;


    private float ignitedDamgeCoolDown = .3f;
    private float ignitedDamageTimer;
    private int igniteDamge;


    [SerializeField] public float currentHealth;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = maxHealth.GetValue();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chillTimer -= Time.deltaTime;
        shockTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if(chillTimer < 0)
            isChilled = false;

        if(shockTimer < 0)
            isShocked = false;

        if(ignitedDamageTimer < 0 && isIgnited)
        {
            currentHealth -= igniteDamge;

            if (currentHealth < 0)
                Die();

            ignitedDamageTimer = ignitedDamgeCoolDown;
        }
    }

    public virtual void DoDamge(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamge = damage.GetValue() + strength.GetValue();

        if (CanCrit())
            totalDamge = CaculateCriticalDamage(totalDamge);

        totalDamge = CheckTargetArmor(_targetStats, totalDamge);
        //_targetStats.TakeDamge(totalDamge);
        doMagicalDamage(_targetStats);
    }

    public virtual void doMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamge(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0) return;

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _fireDamage < _iceDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _fireDamage < _lightingDamage && _iceDamage < _lightingDamage;

        while(!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if(Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if (_ignite || _chill || _shock) return;

        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = 2;
        }

        if (_chill)
        {
            isChilled = _chill;
            chillTimer = 2;
        }

        if(_shock)
        {
            isShocked = _shock;
            shockTimer = 2;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamge = _damage;

    public virtual void TakeDamge(int _damage)
    {
        currentHealth -= _damage;

        if (currentHealth < 0)
            Die();
    }

    protected virtual void Die()
    {

    }

    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamge)
    {
        if (_targetStats.isChilled)
            totalDamge -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamge -= _targetStats.armor.GetValue();

        totalDamge = Mathf.Clamp(totalDamge, 0, int.MaxValue);
        return totalDamge;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("ATTACK AVOIDED");
            return true;

        }
        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCriticalChance)
            return true;

        return false;
    }

    private int CaculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }
}
