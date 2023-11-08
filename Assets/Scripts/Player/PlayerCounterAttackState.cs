using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolname) : base(_player, _stateMachine, _animBoolname)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;

        stateTimer = player.counterAttackDuration;

        player.anim.SetBool("SuccessfulCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            Enemy getEnemy = hit.GetComponent<Enemy>();

            if (getEnemy != null && getEnemy.CanBeStunned())
            {
                stateTimer = 10;
                player.anim.SetBool("SuccessfulCounterAttack", true);

                if (canCreateClone)
                {
                    canCreateClone = false;
                    player.skill.clone.CreateCloneOnCouterAttack(hit.transform);
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
