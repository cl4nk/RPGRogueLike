using UnityEngine;

public class Stairs : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
            LevelManager.Instance.LoadNextFloor();
    }
}