using UnityEngine;

namespace Character
{
    public class EnemyAnimators : MonoBehaviour
    {
        private Animator anim;

        [SerializeField] private Enemy enemy;

        [SerializeField] private PathEnemy pathEnemy;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            WalkAnimator();
        }

        private void WalkAnimator()
        {
            if (enemy.State == Character.STATES.Dead)
                Dead();
            else if (enemy.State == Character.STATES.CantAttack)
                Attack();
            else if (pathEnemy.IsMoving)
                Walk();
            else
                Idle();
        }

        private void UndergoAttack()
        {
            anim.SetBool("isWalk", false);
            anim.SetBool("isRun", false);
            anim.SetBool("isAnother", false);
            anim.SetBool("Attack", false);
            anim.SetBool("LowKick", false);
            anim.SetBool("isDeath", false);
            anim.SetBool("isDeath2", false);
            anim.SetBool("HitStrike", true);
        }

        private void Dead()
        {
            anim.SetBool("isWalk", false);
            anim.SetBool("isRun", false);
            anim.SetBool("isAnother", false);
            anim.SetBool("Attack", false);
            anim.SetBool("LowKick", false);
            anim.SetBool("isDeath", true);
            anim.SetBool("isDeath2", false);
            anim.SetBool("HitStrike", false);
        }

        private void Walk()
        {
            anim.SetBool("isWalk", true);
            anim.SetBool("isRun", false);
            anim.SetBool("isAnother", false);
            anim.SetBool("Attack", false);
            anim.SetBool("LowKick", false);
            anim.SetBool("isDeath", false);
            anim.SetBool("isDeath2", false);
            anim.SetBool("HitStrike", false);
        }

        private void Attack()
        {
            anim.SetBool("isWalk", false);
            anim.SetBool("isRun", false);
            anim.SetBool("isAnother", false);
            anim.SetBool("Attack", true);
            anim.SetBool("LowKick", false);
            anim.SetBool("isDeath", false);
            anim.SetBool("isDeath2", false);
            anim.SetBool("HitStrike", false);
        }

        private void Idle()
        {
            anim.SetBool("isAnother", true);
            anim.SetBool("isWalk", false);
            anim.SetBool("isRun", false);
            anim.SetBool("Attack", false);
            anim.SetBool("LowKick", false);
            anim.SetBool("isDeath", false);
            anim.SetBool("isDeath2", false);
            anim.SetBool("HitStrike", false);
        }
    }
}