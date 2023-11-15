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

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;


    [SerializeField] public float currentHealth;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = maxHealth.GetValue();
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

        isIgnited = _ignite;
        isChilled = _chill;
        isShocked = _shock;
    }

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
        totalDamge -= _targetStats.armor.GetValue();
        totalDamge = Mathf.Clamp(totalDamge, 0, int.MaxValue);
        return totalDamge;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

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
