using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    private Character charac; // enemy

    private void Awake()
    {
        charac = transform.parent.GetComponent<Character>();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "Player" && charac.State == Character.STATES.Neutral)
        {
            StartCoroutine(charac.CouldownAttack());
            charac.Attack();
        }
    }
}