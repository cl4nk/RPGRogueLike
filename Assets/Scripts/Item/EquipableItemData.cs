namespace Item
{
    public abstract class EquipableItemData : ItemData
    {
        public int constitution;
        public int dexterity;
        public int intelligence;
        public int strenght;

        public override string ToString()
        {
            return "Strenght : " + strenght + "\nDexterity : " + dexterity + "\nConstitution : " + constitution +
                   "\nIntelligence : " + intelligence;
        }
    }
}