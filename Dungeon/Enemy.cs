﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    class Enemy
    {
        #region Values
        public string name;
        public double health;
        public int attackMaxDamage;
        public int attackMinDamage;
        public double dodgeChance;
        public double defense;
        #endregion      

        #region EnemyConstructor
        //Costruttore della classe Enemy
        public Enemy(Player player)
        {
            //Creo un array contente tutti i nomi possibili
            var values = Enum.GetValues(typeof(EnemyName));
            Random random = new Random();
            //Ne scelgo a caso uno
            EnemyName name = (EnemyName)values.GetValue(random.Next(values.Length));

            //Lo assegno alla varibiale name della classe Enemy
            this.name = name.ToString();
            this.health = random.Next(player.level, player.level + 5);
            this.attackMaxDamage = random.Next(player.attackMaxDamage, player.attackMaxDamage + 5);
            this.attackMinDamage = random.Next(player.attackMinDamage - 5, player.attackMinDamage);
            this.dodgeChance = 5;
            this.defense = random.Next(Convert.ToInt32(player.defense) - 2, Convert.ToInt32(player.defense) + 5);
        }
        #endregion
    }
}