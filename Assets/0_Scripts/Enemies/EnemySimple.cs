using UnityEngine;

public class EnemySimple : EnemyBase {

    float attackDuration = 1f;
    float offsetX;
    float timeElapsed;
    bool canAttack;
    bool isAttacking;
    bool madeAttack;
    Team _team;

    void Update() {
        if (canAttack && _team != null)
            HandleMovingWhileAttackAnimation();
    }

    void OnDestroy() {
        _team = null;
    }

    void HandleMovingWhileAttackAnimation() {
        if (timeElapsed < attackDuration) {
            timeElapsed += Time.deltaTime;

            var point = _team.MainGuy.transform.position.x + offsetX;
            var distance = point > transform.position.x ? point - transform.position.x : transform.position.x - point;

            if (Mathf.Abs(distance) < 0.001f) {
                canAttack = false;
                isAttacking = false;
                madeAttack = true;
                return;
            }
            float factor = GetMoveValue(timeElapsed / attackDuration) * (point > transform.position.x ? 1 : -1);
            float offset = distance * factor * .3f;
            if (Mathf.Abs(offset) < 0.0001f) {
                canAttack = false;
                isAttacking = false;
                madeAttack = true;
                return;
            }

            transform.Translate(offset, 0, 0);
        } else {
            canAttack = false;
            _team = null;
        }
    }

    public override void Attack(Team team) {
        if (madeAttack || isAttacking || HP <= 0)
            return;
        
        isAttacking = true;

        _team = team;
        CalcOffsetOfAttackPoint();
        timeElapsed = 0f;
        canAttack = true;
        Attack();
    }

    protected override void Attack() {
        Animator.SetTrigger("attack");
        if (AttackSound != null) {
            AudioSource.clip = AttackSound;
            AudioSource.Play();
        }
    }

    float GetMoveValue(float value) {
        // return value == 1 ? 1 : 1 - Mathf.Pow(2, -10 * value);
        return 1 - Mathf.Pow(1 - value, 5);
    }

    void CalcOffsetOfAttackPoint() {
        float teamWidth = _team.MostRightMate.transform.position.x - _team.MostLeftMate.transform.position.x;
        float rand = Random.Range(0.01f, teamWidth);
        float percentage = rand / teamWidth;
        offsetX = percentage > .5f
            ? (percentage - .5f) * teamWidth
            : -(.5f - percentage) * teamWidth;
    }

    public void OnAttackFinish() {
        _team?.TeamHealth.TakeDamage(Random.Range(.1f, 5f));
        _team = null;
    }
}
