using System.Collections;
using UnityEngine;

public class EnemySimple : EnemyBase {

    float attackPointOffsetWithinTeam;
    float roadSpeed;
    float maxHeight = 1.5f;
    float travelTime = .3f;
    float elapsedTime = 0f;
    Vector3 startPosition;
    Vector3 targetPosition;
    Coroutine attacking;
    Team _team;

    void OnDestroy() {
        _team = null;
    }

    public override void Attack(Team team, float roadSpeed) {
        this.roadSpeed = roadSpeed;
        if (attacking != null || HP <= 0)
            return;
        
        _team = team;
        CalcOffsetOfAttackPoint();
        Attack();
        startPosition = transform.position;

        CanBeMoved = false;
        maxHeight = Random.Range(.3f, 1.5f);
        travelTime = Random.Range(.2f, .5f);
        attacking = StartCoroutine(JumpToTeam());
    }

    protected override void Attack() {
        Animator.SetTrigger("attack");
        if (AttackSound != null) {
            AudioSource.clip = AttackSound;
            AudioSource.Play();
        }
    }

    void CalcOffsetOfAttackPoint() {
        float teamWidth = _team.MostRightMate.transform.position.x - _team.MostLeftMate.transform.position.x;
        float rand = Random.Range(0.01f, teamWidth);
        float percentage = rand / teamWidth;
        attackPointOffsetWithinTeam = percentage > .5f
            ? (percentage - .5f) * teamWidth
            : -(.5f - percentage) * teamWidth;
    }

    IEnumerator JumpToTeam() {
        while (elapsedTime < travelTime) {
            elapsedTime += Time.deltaTime;
            RoadPosition -= Time.deltaTime * roadSpeed;
            float t = Mathf.Clamp01(elapsedTime / travelTime);
            targetPosition = new Vector3(
                _team.MainGuy.transform.position.x + attackPointOffsetWithinTeam,
                0,
                _team.MainGuy.transform.position.z
            );

            transform.position = CalculateParabolicPoint(startPosition, targetPosition, maxHeight, t);
            yield return null;
        }
        CanBeMoved = true;
    }

    Vector3 CalculateParabolicPoint(Vector3 start, Vector3 end, float jumpHeight, float t) {
        Vector3 midPoint = Vector3.Lerp(start, end, t);
        float parabola = 4 * jumpHeight * (t - t * t);
        midPoint.y = Mathf.Lerp(start.y, end.y, t) + parabola;
        return midPoint;
    }

    public void OnAttackFinish() {
        _team?.TeamHealth.TakeDamage(Random.Range(1f, 5f));
        _team = null;
    }
}
