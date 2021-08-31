using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public static class Gameplay
    {
        public static (Player, List<Items>) Explore(Player player, List<Items> Inventory)
        {
            //Il player percorre x passi
            Random random = new Random();         
            int steps = random.Next(1, 500);

            Console.WriteLine(String.Empty);
            Console.Write($"[-] {player.name} has walked {steps} steps ");

            //Se il player non è implicato in una condizione gliene do una
            if(player.currentCondition == Conditions.None)
            {               
                var cc = random.Next(1, 4);

                switch (cc)
                {
                    case 1:
                        player.currentCondition = Conditions.Fight; //Si è scontrato con un nemico
                        var a = Fight(player, Inventory);
                        player = a.Item1;
                        Inventory = a.Item2;
                        break;
                    case 2:
                        player.currentCondition = Conditions.Loot; //Si è scontrato con un oggetto
                        var b = Loot(player, Inventory);
                        player = b.Item1;
                        Inventory = b.Item2;
                        break;
                    case 3:
                        Console.Write("and nothing happen"); //Non si è scontrato
                        Console.WriteLine(String.Empty);
                        break;
                }              
                
            }

            player.currentCondition = Conditions.None;

            return (player , Inventory);

        }//Explore Function
        public static (Player, List<Items>) Fight(Player player, List<Items> Inventory)
        {
            //Genero un nuovo nemico
            Enemy enemy = new Enemy(player);

            Console.Write($"and has ran into {enemy.name}" + "\n");

            Random random = new Random();

            //Se hanno sufficente vita combattono
            while(player.health != 0 || enemy.health != 0)
            {

                Menu.Continue();

                //Proabilità di mancare il nemico
                if (random.Next(0, 80) < enemy.dodgeChance)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("\n" + $"[X]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" You missed { enemy.name}");
                }//Probabilità di ridurre i danni inflitti
                else if(random.Next(0, 40) < enemy.defense)
                {                
                    var a = random.Next(player.attackMinDamage, player.attackMaxDamage) / enemy.defense;
                    enemy.health -= a;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("\n" + $"[X]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {enemy.name} has dodged {enemy.defense} damages");
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write("\n" + $"[V]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {player.name.ToLower()} has done {a} damage to {enemy.name}");
                }
                else//Danni normali
                {                    
                    var b = random.Next(player.attackMinDamage, player.attackMaxDamage);
                    enemy.health -= b;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write("\n" + $"[V]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {player.name.ToLower()} has done {b} damage to {enemy.name}");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                }

                //Controllo se l'ho uscciso
                if(enemy.health < 0)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.Write("\n" + $"[#]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.Write($" {player.name.ToLower()} killed {enemy.name} ");
                    //Ricompensa casuale
                    if(random.Next(0,2) == 1)
                    {
                        var reward = Loot(player, Inventory);
                        player = reward.Item1;
                        Inventory = reward.Item2;
                        Console.WriteLine(String.Empty);
                    }
                    else
                    {
                        //Incremento di esperienza
                        player.level++;
                        Console.Write($"and now is level {player.level}");
                        Console.WriteLine(String.Empty);
                        Console.WriteLine(String.Empty);
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"[!]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write($" {player.name.ToLower()} has ");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write($"{player.health} HP left");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(String.Empty);
                    break;
                }

                Menu.Continue();

                //Proabilità di schivare l'attacco
                if (random.Next(0, 80) < player.dodgeChance)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write("\n" + $"[V]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {enemy.name} has missed you");
                }
                else if (random.Next(0, 40) < player.defense)//Probabilità di ridurre i danni subiti
                {
                    var a = random.Next(enemy.attackMinDamage, enemy.attackMaxDamage) / player.defense;
                    player.health -= a;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write("\n" + $"[V]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {player.name} has dodged {player.defense} damages");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("\n" + $"[X]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {enemy.name} has done {a} damage to {player.name.ToLower()}");
                }
                else//Danno normale
                {
                    var b = random.Next(enemy.attackMinDamage, enemy.attackMaxDamage);
                    player.health -= b;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("\n" + $"[X]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {enemy.name} has done {b} damage to {player.name.ToLower()}");
                }

                //Controllo se sono morto
                if (player.health < 0)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("\n" + $"[X]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {player.name.ToUpper()} DIED");
                    break;
                }

            }

            return (player, Inventory);

        }//Fight Function
        public static (Player, List<Items>) Loot(Player player, List<Items> Inventory)
        {

            Console.Write($"and has found a ");

            var values = Enum.GetValues(typeof(Items));
            Random random = new Random();

            var rarity = random.Next(0, 11);
            string[] controls = null;

            Items item = Items.Basic_Axe;

            switch (rarity)
            {
                case 10:
                    do
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        item = (Items)values.GetValue(random.Next(values.Length));
                        controls = item.ToString().Split('_');
                    }
                    while (controls[0] != "Legendary");
                    break;

                case 7:
                    do
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        item = (Items)values.GetValue(random.Next(values.Length));
                        controls = item.ToString().Split('_');
                    }
                    while (controls[0] != "Epic");
                    break;

                case 5:
                    do
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        item = (Items)values.GetValue(random.Next(values.Length));
                        controls = item.ToString().Split('_');
                    }
                    while (controls[0] != "Uncommon");
                    break;

                case 3:
                    do
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        item = (Items)values.GetValue(random.Next(values.Length));
                        controls = item.ToString().Split('_');
                    }
                    while (controls[0] != "Rare");
                    break;

                case 1:
                    do
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        item = (Items)values.GetValue(random.Next(values.Length));
                        controls = item.ToString().Split('_');
                    }
                    while (controls[0] != "Basic");
                    break;

                default:
                    int type = random.Next(1, 2);
                    if(type == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        item = Items.Health_Potion;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        item = Items.Mana_Potion;
                    }
                    break;                  
            }
       
            var names = ItemName(item);

            Console.Write($"{names[0].ToLower()}"); 
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($" {names[1].ToLower()}" + "\n");

            //Lo aggiungo al suo inventario
            Inventory.Add(item);

            return (player, Inventory);
        }//Loot Function
        public static (Player, List<Items>) UseItem(Player player, List<Items> Inventory, Items item)//Use Item Function
        {

            switch (item)
            {
                case Items.Basic_Axe:
                    var a = ItemName(item);
                    Console.Write($"[!] You equipped {a[0]} {a[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 15;
                    player.attackMinDamage = 5;
                    break;

                case Items.Rare_Axe:
                    var b = ItemName(item);
                    Console.Write($"[!] You equipped {b[0]} {b[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 40;
                    player.attackMinDamage = 10;
                    break;

                case Items.Uncommon_Axe:
                    var c = ItemName(item);
                    Console.Write($"[!] You equipped {c[0]} {c[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 50;
                    player.attackMinDamage = 10;
                    break;

                case Items.Epic_Axe:
                    var d = ItemName(item);
                    Console.Write($"[!] You equipped {d[0]} {d[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 60;
                    player.attackMinDamage = 20;
                    break;

                case Items.Legendary_Axe:
                    var e = ItemName(item);
                    Console.Write($"[!] You equipped {e[0]} {e[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 70;
                    player.attackMinDamage = 20;
                    break;

                case Items.Basic_Sword:
                    var f = ItemName(item);
                    Console.Write($"[!] You equipped {f[0]} {f[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 10;
                    player.attackMinDamage = 5;
                    break;

                case Items.Rare_Sword:
                    var g = ItemName(item);
                    Console.Write($"[!] You equipped {g[0]} {g[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 25;
                    player.attackMinDamage = 15;
                    break;

                case Items.Uncommon_Sword:
                    var h = ItemName(item);
                    Console.Write($"[!] You equipped {h[0]} {h[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 35;
                    player.attackMinDamage = 25;
                    break;

                case Items.Epic_Sword:
                    var i = ItemName(item);
                    Console.Write($"[!] You equipped {i[0]} {i[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 45;
                    player.attackMinDamage = 35;
                    break;

                case Items.Legendary_Sword:
                    var j = ItemName(item);
                    Console.Write($"[!] You equipped {j[0]} {j[1]}");
                    player.equipped = item;
                    player.attackMaxDamage = 55;
                    player.attackMinDamage = 45;
                    break;

                case Items.Basic_Shield:
                    Console.Write($"[!] You gained +{0.5} defense");
                    player.defense += 0.5f;
                    Inventory.Remove(item);
                    break;

                case Items.Rare_Shield:
                    Console.Write($"[!] You gained +{0.7} defense");
                    player.defense += 0.7f;
                    Inventory.Remove(item);
                    break;

                case Items.Uncommon_Shield:
                    Console.Write($"[!] You gained +{0.8} defense");
                    player.defense += 0.8f;
                    Inventory.Remove(item);
                    break;

                case Items.Epic_Shield:
                    Console.Write($"[!] You gained +{0.9} defense");
                    player.defense += 0.9f;
                    Inventory.Remove(item);
                    break;

                case Items.Legendary_Shield:
                    Console.Write($"[!] You gained +{1} defense");
                    player.defense += 1.0f;
                    Inventory.Remove(item);
                    break;

                case Items.Health_Potion:
                    int gained = 0;
                    if(player.health < 100)
                    {
                        for(int z = 0; z < 25; z++)
                        {
                            if(player.health < 100)
                            {
                                player.health++;
                                gained++;
                            }
                        }
                    }
                    Console.Write($"[!] You gained {gained} HP");
                    Inventory.Remove(item);
                    break;

                case Items.Mana_Potion:
                    int restored = 0;
                    if (player.mana < 10)
                    {
                        for (int zz = 0; zz < 25; zz++)
                        {
                            if (player.mana < 10)
                            {
                                player.mana++;
                                restored++;
                            }
                        }
                    }
                    Console.Write($"[!] You restored {restored} mana");
                    Inventory.Remove(item);
                    break;


            }

            return (player, Inventory);
        }
        public static string[] ItemName(Items item)
        {
            var names = item.ToString().Split('_');
            return names;
        }//Name formatter

    }
}