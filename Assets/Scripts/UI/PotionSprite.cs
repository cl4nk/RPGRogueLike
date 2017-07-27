using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PotionSprite : MonoBehaviour
    {
        private Image image;
        private Text numberLabel;
        private float time;
        private float timeMax;

        private void Start()
        {
            image = GetComponent<Image>();
            numberLabel = transform.GetChild(0).GetComponent<Text>();
        }

        public void ShowPotion(float _time)
        {
            time = _time;
            timeMax = _time;

            image.fillAmount = 1;
            numberLabel.enabled = true;
        }

        public void UpdateNumberPotion(int nbPotion)
        {
            numberLabel.text = "x" + nbPotion;
        }

        private void Update()
        {
            if (time > 0)
            {
                time -= Time.deltaTime;

                if (time <= 0)
                {
                    numberLabel.enabled = false;
                    image.fillAmount = 0;
                }
                else
                {
                    image.fillAmount = time / timeMax;
                }
            }
        }
    }
}