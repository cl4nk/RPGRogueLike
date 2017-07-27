using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            LevelManager levelMgr = LevelManager.Instance;

            if (levelMgr.Place == LevelManager.PLACES.CampFire)
                levelMgr.Place = LevelManager.PLACES.Dungeon;
            else
                levelMgr.Place = LevelManager.PLACES.CampFire;
        }
    }
}