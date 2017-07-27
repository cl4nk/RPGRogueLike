using Character;
using UnityEngine;

namespace Managers
{
    public class AlterationList : MonoBehaviour
    {
        [SerializeField] private string alterateStat;
        private int nbAlteration;

        private void Update()
        {
            if (transform.childCount > 0)
            {
                Alteration alteration = transform.GetChild(0).GetComponent<Alteration>();

                if (alteration.PotionState == Alteration.POTIONSTATES.Neutral)
                {
                    AlterationManager.Instance.UpdateFeedBackPotion(alterateStat, alteration.Time);
                    StartCoroutine(alteration.TimeAlteraction());
                }

                if (nbAlteration != transform.childCount)
                {
                    nbAlteration = transform.childCount;
                    AlterationManager.Instance.UpdateFeedBackNumberPotion(alterateStat, nbAlteration);
                }
            }
        }
    }
}