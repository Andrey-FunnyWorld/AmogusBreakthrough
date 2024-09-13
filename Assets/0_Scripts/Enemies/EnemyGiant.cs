using System.Collections;
using UnityEngine;

public class EnemyGiant : EnemyBase {

    Team _team;
    Coroutine lookAtTeam;
    float lookTime = 1f;
    bool attackMade = false;

    void OnDestroy() {
        _team = null;
    }
    
    public override void Attack(Team team, float roadSpeed) {
        if (lookAtTeam != null || attackMade || HP <= 0)
            return;

        _team = team;
        lookAtTeam = StartCoroutine(LookAtTeam());
        Animator.SetTrigger("attack");
    }

    IEnumerator LookAtTeam() {
        float timer = 0f;
        while (timer < lookTime) {
            if (HP <= 0)
                attackMade = true;

            timer += Time.deltaTime;
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
            Animator.transform.rotation = Quaternion.Slerp(Animator.transform.rotation, Quaternion.LookRotation(-Vector3.forward), 2 * Time.deltaTime);
            yield return null;
        }
    }

    void PlayAttackSound() {
        if (AttackSound != null) {
            AudioSource.clip = AttackSound;
            AudioSource.Play();
        }
    }

    public void OnAttackFinish() {
        attackMade = true;
        PlayAttackSound();

        if (HP > 0)
            _team?.TeamHealth.TakeDamage(Damage);
        _team = null;

        StopAllCoroutines();
        StartCoroutine(LookForward());
    }
}
