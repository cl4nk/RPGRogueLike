using UnityEngine;

public class EnemyDetectionZone : MonoBehaviour
{
    private Transform player;

    private void Awake()
    {
        transform.parent.GetComponent<PathEnemy>().RangeOfView = GetComponent<SphereCollider>().radius;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && !player)
            player = collider.transform;
    }

    private void Update()
    {
        if (player)
            transform.parent.GetComponent<PathEnemy>().CheckIfPlayerIsSeen(player);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform == player)
            player = null;
    }
}