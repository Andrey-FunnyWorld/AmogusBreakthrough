using UnityEngine;

public class MainGuy : MonoBehaviour
{
    public Team Team;

    void Start()
    {
        Team.CreateTeam();
    }

    public void ApplyMovement(Vector3 newPosition)
    {
        transform.position = newPosition;
        Team.ApplyMovement(newPosition);
    }

    public void StartMove()
    {
        // start move animation for the guy and team
    }

    public void VisualiseAttack(Vector3 position)
    {
        Debug.DrawRay(transform.position, position - transform.position, Color.red);
        Team.VisualiseAttack(position);
    }

}
