using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacElement : MonoBehaviour
    {
        private Text characNbLabel;

        private Characteristics characWindow;
        [SerializeField] [TextArea(3, 10)] private string description;
        private Text modifierLabel;
        private string modifierText;

        private int valueCharac;

        public string CharacText { get; set; }

        public int ValueCharac
        {
            get { return valueCharac; }
            set
            {
                valueCharac = value;
                RefreshNbLabel();
            }
        }

        public int AllocatedPoints { get; set; }

        private void Awake()
        {
            characWindow = transform.parent.GetComponent<Characteristics>();
            characNbLabel = transform.Find("CharacNbLabel").GetComponent<Text>();
            modifierLabel = transform.Find("ModifierLabel").GetComponent<Text>();

            CharacText = transform.Find("CharacLabel").GetComponent<Text>().text;
            modifierText = modifierLabel.text;
        }

        public void RefreshNbLabel()
        {
            characNbLabel.text = valueCharac.ToString();
        }

        public void RefreshModifier()
        {
            modifierLabel.text = string.Format(modifierText, AllocatedPoints);
        }

        public void PlusValue()
        {
            if (characWindow.CanSpent())
            {
                ValueCharac++;
                AllocatedPoints++;

                RefreshModifier();
                characWindow.OnePointSpent();
            }
        }

        public void MinusValue()
        {
            if (AllocatedPoints > 0)
            {
                ValueCharac--;
                AllocatedPoints--;

                RefreshModifier();
                characWindow.OnePointRefund();
            }
        }

        public void OnMouseEnter()
        {
            characWindow.ChangeDescription(description);
        }

        public void OnMouseExit()
        {
            characWindow.ChangeDescription("");
        }
    }
}