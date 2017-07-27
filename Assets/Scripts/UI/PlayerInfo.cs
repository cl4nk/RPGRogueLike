using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerInfo : MonoBehaviour
    {
        private Slider expBar;
        private Text expLabel;
        private Slider lifeBar;

        private Text lifeLabel;
        private Slider manaBar;
        private Text manaLabel;

        private void Awake()
        {
            lifeBar = transform.Find("LifeBar").GetComponent<Slider>();
            manaBar = transform.Find("ManaBar").GetComponent<Slider>();
            expBar = transform.Find("ExperienceBar").GetComponent<Slider>();

            lifeLabel = lifeBar.transform.Find("LifeLabel").GetComponent<Text>();
            manaLabel = manaBar.transform.Find("ManaLabel").GetComponent<Text>();
            expLabel = expBar.transform.Find("ExperienceLabel").GetComponent<Text>();
        }

        public void RefreshLife(int curLife, int maxLife)
        {
            lifeBar.value = curLife * 100 / maxLife;
            lifeLabel.text = curLife + "/" + maxLife;
        }

        public void RefreshMana(int curMana, int maxMana)
        {
            manaBar.value = curMana * 100 / maxMana;
            manaLabel.text = curMana + "/" + maxMana;
        }

        public void RefreshExp(int exp, int expNextLevel)
        {
            expBar.value = exp * 100 / expNextLevel;
            expLabel.text = exp + "/" + expNextLevel;
        }
    }
}