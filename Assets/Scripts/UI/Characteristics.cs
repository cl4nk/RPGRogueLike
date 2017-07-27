using Character;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Characteristics : MonoBehaviour
    {
        private Text availablePointsLabel;
        private string availablePointsText;
        private CharacElement[] characElements;
        private Text descriptionLabel;
        private bool locked;
        private Text playerLevelLabel;

        private string playerLevelText;
        private int pointsSpent;
        private int pointsToSpend;
        private int prevPlayerLevel = 1;
        private UIGame uiGame;

        private void Awake()
        {
            uiGame = transform.parent.GetComponent<UIGame>();
            characElements = transform.GetComponentsInChildren<CharacElement>();

            playerLevelLabel = transform.Find("PlayerLevelLabel").GetComponent<Text>();
            playerLevelText = playerLevelLabel.text;

            availablePointsLabel = transform.Find("AvailablePointsLabel").GetComponent<Text>();
            availablePointsText = availablePointsLabel.text;

            descriptionLabel = transform.Find("CharacDescriptionLabel").GetComponent<Text>();
        }

        private void OnEnable()
        {
            playerLevelLabel.text = playerLevelText + uiGame.player.Level;

            foreach (CharacElement characElement in characElements)
            {
                string characName = characElement.CharacText;
                if (characName == "Strength")
                    characElement.ValueCharac = uiGame.player.Strength;
                else if (characName == "Dexterity")
                    characElement.ValueCharac = uiGame.player.Dexterity;
                else if (characName == "Constitution")
                    characElement.ValueCharac = uiGame.player.Constitution;
                else if (characName == "Intelligence")
                    characElement.ValueCharac = uiGame.player.Intelligence;

                characElement.RefreshModifier();
            }

            if (prevPlayerLevel < uiGame.player.Level)
            {
                uiGame.LockCharacWindow();
                locked = true;
                pointsToSpend = (uiGame.player.Level - prevPlayerLevel) * uiGame.player.CharacPointsOnLevelUp;
                prevPlayerLevel = uiGame.player.Level;
            }

            RefreshAvailablePoints();
        }

        public void RefreshAvailablePoints()
        {
            availablePointsLabel.text = availablePointsText + (pointsToSpend - pointsSpent);
        }

        public void ConfirmPressed()
        {
            if (pointsToSpend == pointsSpent)
            {
                foreach (CharacElement characElement in characElements)
                {
                    string characName = characElement.CharacText;
                    if (characName == "Strength")
                        uiGame.player.Strength = characElement.ValueCharac;
                    else if (characName == "Dexterity")
                        uiGame.player.Dexterity = characElement.ValueCharac;
                    else if (characName == "Constitution")
                        uiGame.player.Constitution = characElement.ValueCharac;
                    else if (characName == "Intelligence")
                        uiGame.player.Intelligence = characElement.ValueCharac;

                    characElement.AllocatedPoints = 0;
                }

                pointsToSpend = 0;
                pointsSpent = 0;

                if (locked)
                    uiGame.UnlockCharacWindow();

                uiGame.player.RecalculateCharacterStats();
                uiGame.RefreshPlayerInfo();
                uiGame.ToggleCrosshair();
                uiGame.ToggleCharacWindow();

                InputManager.Instance.UnlockKey();
                TimeManager.Instance.State = TimeManager.StateGame.Running;

                uiGame.player.transform.Find("CamHolder/PlayerCamera").GetComponent<PlayerCamera>().ToggleCursorLock();
            }
        }

        public void ChangeDescription(string description)
        {
            descriptionLabel.text = description;
        }

        public void OnePointSpent()
        {
            pointsSpent++;
            RefreshAvailablePoints();
        }

        public void OnePointRefund()
        {
            pointsSpent--;
            RefreshAvailablePoints();
        }

        public bool CanSpent()
        {
            return pointsToSpend - pointsSpent > 0;
        }
    }
}