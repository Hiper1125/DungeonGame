namespace Dungeon
{
    public class Player
    {
        public string name;
        public int level;
        public int steps;
        public double health;
        public double stamina;
        public int mana;
        public int gold;
        public int attackMaxDamage;
        public int attackMinDamage;
        public double dodgeChance;
        public double defense;
        public Item equipped;
        public Conditions currentCondition;

        public Player(string nickname)
        {
            name = nickname;
            
            level = 0;
            steps = 0;
            
            health = 100;
            stamina = 100;

            mana = 10;

            gold = 0;
            
            attackMaxDamage = 15;
            attackMinDamage = 5;
            
            dodgeChance = 13f;
            defense = 10;
            
            equipped = Item.BasicAxe;
            currentCondition = Conditions.None;
        }
    }
}