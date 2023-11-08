using UnityEngine;

public class BlackHole_Skill : Skill
{
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCoolDown;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;


    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

        BlackHole_Skill_Controller newBlackHoleScript = newBlackHole.GetComponent<BlackHole_Skill_Controller>();

        newBlackHoleScript.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCoolDown);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
