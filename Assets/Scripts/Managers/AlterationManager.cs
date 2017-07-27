using Character;
using UI;
using UnityEngine;

namespace Managers
{
    public class AlterationManager : MonoBehaviour
    {
        private static AlterationManager instance;
        [SerializeField] private GameObject alterationPrefab;
        [SerializeField] private GameObject constitutionPotions;
        [SerializeField] private GameObject dexterityPotions;
        [SerializeField] private GameObject intelligencePotions;
        [SerializeField] private GameObject strenghtPotions;

        public static AlterationManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<AlterationManager>();

                return instance;
            }
        }

        public void AddAlteration(int _strenght, int _constitution, int _intelligence, int _dexterity, float _time,
            Character.Character character)
        {
            GameObject alteration;

            if (_strenght > 0)
            {
                alteration = CreateAlteration(_strenght, 0, 0, 0, _time, character);
                alteration.transform.parent = strenghtPotions.transform;
            }
            if (_constitution > 0)
            {
                alteration = CreateAlteration(0, _constitution, 0, 0, _time, character);
                alteration.transform.parent = constitutionPotions.transform;
            }
            if (_intelligence > 0)
            {
                alteration = CreateAlteration(0, 0, _intelligence, 0, _time, character);
                alteration.transform.parent = intelligencePotions.transform;
            }
            if (_dexterity > 0)
            {
                alteration = CreateAlteration(0, 0, 0, _dexterity, _time, character);
                alteration.transform.parent = dexterityPotions.transform;
            }
        }

        public GameObject CreateAlteration(int _strenght, int _constitution, int _intelligence, int _dexterity,
            float _time,
            Character.Character character)
        {
            GameObject alteration = Instantiate(alterationPrefab);
            alteration.GetComponent<Alteration>().Init(_strenght, _constitution, _intelligence, _dexterity, _time,
                character);

            return alteration;
        }

        public void RemoveAlteration(Alteration alteration)
        {
            alteration.transform.parent = null;
            Destroy(alteration.gameObject);
        }

        public void UpdateFeedBackPotion(string labelName, float time)
        {
            UIGame.Instance.UpdateFeedBackPotion(labelName, time);
        }

        public void UpdateFeedBackNumberPotion(string labelName, int nbAlteration)
        {
            UIGame.Instance.UpdateFeedBackNumberPotion(labelName, nbAlteration);
        }
    }
}