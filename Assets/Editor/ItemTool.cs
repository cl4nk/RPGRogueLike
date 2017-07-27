using UnityEditor;
using UnityEngine;

public class ItemTool : EditorWindow
{
    public enum ITEM_TYPE
    {
        POTION
    }

    public ITEM_TYPE itemType = ITEM_TYPE.POTION;
    public PotionItemData potionItem;

    [MenuItem("Tools/Item Editor %#i")]
    private static void Init()
    {
        GetWindow(typeof(ItemTool));
    }


    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Item Editor", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();

        itemType = (ITEM_TYPE) EditorGUILayout.EnumPopup("Item to create", itemType);
        GUILayout.Space(10);


        if (EditorGUI.EndChangeCheck())
            Debug.Log("TYPE : " + itemType);

        ShowField();

        GUILayout.BeginVertical();
        if (GUILayout.Button("Create new Item", GUILayout.ExpandWidth(false)))
            CreateNewItem();
        GUILayout.EndVertical();
    }

    private void ShowField()
    {
        switch (itemType)
        {
            case ITEM_TYPE.POTION:
                PotionField();
                break;
        }
    }

    private void PotionField()
    {
        if (potionItem == null)
            potionItem = CreateInstance<PotionItemData>();

        GUILayout.BeginHorizontal();
        potionItem.name = EditorGUILayout.TextField("Potion name", potionItem.name);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.description = EditorGUILayout.TextField("Potion description", potionItem.description);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.isQuestItem = EditorGUILayout.Toggle("Is quest item", potionItem.isQuestItem);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        potionItem.weight = EditorGUILayout.IntSlider("Weight", potionItem.weight, 0, 999,
            GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.value = EditorGUILayout.IntSlider("Value", potionItem.value, 0, 999, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.levelRequired = EditorGUILayout.IntSlider("Level required", potionItem.levelRequired, 0, 999,
            GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        //GUILayout.BeginHorizontal();
        //potionItem.numberOfUse = EditorGUILayout.IntSlider("Number of use", potionItem.numberOfUse, 1, 99, GUILayout.ExpandWidth(false));
        //GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        potionItem.strenght = EditorGUILayout.IntSlider("Strenght", potionItem.strenght, 0, 99,
            GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.constitution = EditorGUILayout.IntSlider("Constitution", potionItem.constitution, 0, 99,
            GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.intelligence = EditorGUILayout.IntSlider("Intelligence", potionItem.intelligence, 0, 99,
            GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.dexterity = EditorGUILayout.IntSlider("Dexterity", potionItem.dexterity, 0, 99,
            GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.life = EditorGUILayout.IntSlider("Life", potionItem.life, 0, 99, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        potionItem.mana = EditorGUILayout.IntSlider("Mana", potionItem.mana, 0, 99, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.Space(50);
    }

    private void CreateNewItem()
    {
        switch (itemType)
        {
            case ITEM_TYPE.POTION:
                AssetDatabase.CreateAsset(potionItem, "Assets/Datas/Potions/" + potionItem.name + ".asset");
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = potionItem;
                break;
        }
    }

    private void ResetField()
    {
        // TODO ResetField 
    }
}