using System.Collections;
using UnityEngine;

public class EnemyGiant : EnemyBase {

    Team _team;
    Coroutine lookAtTeam;
    float lookTime = 1f;
    float elapsedTime = 0f;
    bool attackMade = false;

    void OnDestroy() {
        _team = null;
    }
    
    public override void Attack(Team team, float roadSpeed) {
        if (lookAtTeam != null || attackMade || HP <= 0)
            return;

        _team = team;
        lookAtTeam = StartCoroutine(LookAtTeam());
        Attack();
    }

    protected override void Attack() {
        Animator.SetTrigger("attack");
        if (AttackSound != null) {
            AudioSource.clip = AttackSound;
            AudioSource.Play();
        }
        // ВАНШОТ, ёбба!
    }

    IEnumerator LookAtTeam() {
        while (elapsedTime < lookTime) {
            if (HP <= 0)
                attackMade = true;

            elapsedTime += Time.deltaTime;
            if (_team != null && HP > 0) {
                Animator.transform.LookAt(_team.MainGuy.transform);
            } else {
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator LookForward() {
        IsRunningChanged(true);
        while (Vector3.Angle(Animator.transform.forward, -Vector3.forward) > 1f) {
            Animator.transform.rotation = Quaternion.Slerp(Animator.transform.rotation, Quaternion.LookRotation(-Vector3.forward), 3 * Time.deltaTime);
            yield return null;
        }
    }

    public void OnAttackFinish() {
        attackMade = true;

        if (HP > 0)
            _team?.TeamHealth.TakeDamage(30f);
        _team = null;

        StopAllCoroutines();
        StartCoroutine(LookForward());
    }
}
