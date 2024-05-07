using UnityEngine;

[RequireComponent(typeof(AttackController))]
public class AttackArea : MonoBehaviour
{
    private AttackController AttackController;
    [SerializeField] private BoxCollider box;

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = new Color(1, 0, 0.2f, 0.25f);
    //     Gizmos.DrawCube(transform.position + box.center, box.size);
    // }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0.2f, 0.25f);
        Gizmos.DrawCube(transform.position + box.center, box.size);
    }

    void Start()
    {
        AttackController = GetComponent<AttackController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            AttackController.AddNewEnemy(other.gameObject.GetComponent<EnemyBase>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            AttackController.RemoveEnemy(other.gameObject.GetComponent<EnemyBase>());
        }
    }
}
