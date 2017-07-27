using Character;

namespace Item
{
    public abstract class UsableItemData : ItemData
    {
        public virtual void Use(Player player)
        {
        }

        /*
     * 
     * This class is not implemented but use for required features.
     * 
     * Think to create a great architecture with other Usable objects 
     * 
     * */
    }
}