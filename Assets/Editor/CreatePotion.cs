using Item;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CreatePotion : ScriptableWizard
    {
        public int constitution;
        public string description = "Potion Description";
        public int dexterity;
        public int intelligence;
        public bool isQuestItem = true;

        public int life;
        public int mana;

        public new string name = "Potion name";

        public int strenght;

        [MenuItem("Tools/CreatePotion")]
        private static void CreateWizard()
        {
            DisplayWizard<CreatePotion>("Create Potion", "Create new", "Update selected");
        }

        private void OnWizardCreate()
        {
            PotionItemData potion = CreateInstance<PotionItemData>();
            AssetDatabase.CreateAsset(potion, "Assets/Datas/Potions/" + name + ".asset");

            potion.name = name;
            potion.description = description;
            potion.isQuestItem = isQuestItem;

            potion.strenght = strenght;
            potion.constitution = constitution;
            potion.intelligence = intelligence;
            potion.dexterity = dexterity;

            potion.life = life;
            potion.mana = mana;

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = potion;
        }

        private void OnWizardOtherButton()
        {
            Object selected = Selection.activeObject;

            if (Selection.activeObject.GetType() == typeof(PotionItemData))
            {
                PotionItemData potion = selected as PotionItemData;

                potion.name = name;
                potion.description = description;
                potion.isQuestItem = isQuestItem;

                potion.strenght = strenght;
                potion.constitution = constitution;
                potion.intelligence = intelligence;
                potion.dexterity = dexterity;

                potion.life = life;
                potion.mana = mana;
            }
        }
    }
}