using UnityEngine;

public class TeamMember : MonoBehaviour {
    public Animator Animator;
    public void SetRun(bool run) {
        Animator.SetBool("run", run);
    }
    public void SetStandVictorious(bool stand) {
        Animator.SetBool("stand", stand);
    }
}
