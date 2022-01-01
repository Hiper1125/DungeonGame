using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dungeon
{
    public static class Gameplay
    {
        public static (Player, List<Item>) Explore(Player player, List<Item> inventory)
        {
            //Il player percorre x passi
            Random random = new Random();
            int steps = random.Next(10, 120);

            if (player.stamina > Math.Round(steps / (double)random.Next(1, 15)) + 10)
            {
                player.steps += steps;
                player.stamina -= (int) Math.Round((steps / 15f) * random.Next(1, 4));

                Console.WriteLine(String.Empty);
                Console.Write($"[-] {player.name} has walked {steps} steps ");

                //Se il player non è implicato in una condizione gliene do una
                if (player.currentCondition == Conditions.None)
                {
                    var cc = random.Next(1, 4);

                    switch (cc)
                    {
                        case 1:
                            player.currentCondition = Conditions.Fight; //Si è scontrato con un nemico
                            var a = Fight(player, inventory);
                            player = a.Item1;
                            inventory = a.Item2;
                            break;
                        case 2:
                            player.currentCondition = Conditions.Loot; //Si è scontrato con un oggetto
                            var b = Loot(player, inventory);
                            player = b.Item1;
                            inventory = b.Item2;
                            break;
                        case 3:
                            Console.Write("and nothing happen"); //Non si è scontrato
                            Console.WriteLine(String.Empty);
                            break;
                    }
                }

                player.currentCondition = Conditions.None;

                return (player, inventory);
            }
            else
            {
                Console.WriteLine(String.Empty);

                List<string> tiredMessages = new List<string>();
                
                tiredMessages.Add("[-] Your character is too tired to walk any more!");
                tiredMessages.Add($"[-] {player.name} is tired, you should rest for a little!");
                tiredMessages.Add("[-] Please rest before exploring again!");

                Random rnd = new Random();
                Console.WriteLine(tiredMessages[rnd.Next(tiredMessages.Count)]);
                
                player.currentCondition = Conditions.None;

                return (player, inventory);
            }
        }

        private static (Player, List<Item>) Fight(Player player, List<Item> inventory)
        {
            //Genero un nuovo nemico
            Enemy enemy = new Enemy(player);

            Console.Write($"and has ran into {enemy.name}" + "\n");

            Random random = new Random();

            //Se hanno sufficente vita combattono
            while (player.health != 0 || enemy.health != 0)
            {
                Menu.Continue();

                //Proabilità di mancare il nemico
                if (random.Next(0, 80) < enemy.dodgeChance)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("\n" + $"[X]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" You missed {enemy.name}");
                } //Probabilità di ridurre i danni inflitti
                else if (random.Next(0, 40) < enemy.defense)
                {
                    var a = Math.Round(random.Next(player.attackMinDamage, player.attackMaxDamage) / enemy.defense, 2);
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
                else //Danni normali
                {
                    var b = Math.Round((double)random.Next(player.attackMinDamage, player.attackMaxDamage), 2);
                    enemy.health -= b;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write("\n" + $"[V]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($" {player.name.ToLower()} has done {b} damage to {enemy.name}");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                }

                //Controllo se l'ho uscciso
                if (enemy.health < 0)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.Write("\n" + $"[#]");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.Write($" {player.name.ToLower()} killed {enemy.name} ");
                    //Ricompensa casuale
                    if (random.Next(0, 2) == 1)
                    {
                        var reward = Loot(player, inventory);
                        player = reward.Item1;
                        inventory = reward.Item2;
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
                    Console.Write($" {player.health} HP left ");
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
                else if (random.Next(0, 40) < player.defense) //Probabilità di ridurre i danni subiti
                {
                    var a = Math.Round(random.Next(enemy.attackMinDamage, enemy.attackMaxDamage) / player.defense, 2);
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
                else //Danno normale
                {
                    var b = Math.Round((double)random.Next(enemy.attackMinDamage, enemy.attackMaxDamage), 2);
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

            return (player, inventory);
        }

        private static (Player, List<Item>) Loot(Player player, List<Item> inventory)
        {
            Console.Write($"and has found a ");

            var values = Enum.GetValues(typeof(Item));
            Random random = new Random();

            var rarity = random.Next(0, 11);
            var rarityString = string.Empty;
            string[] controls = null;

            Item item = Item.BasicAxe;

            if (rarity == 10)
            {
                rarityString = "Legendary";
            }
            else if (rarity == 7)
            {
                rarityString = "Epic";
            }
            else if (rarity == 5)
            {
                rarityString = "Uncommon";
            }
            else if (rarity == 3)
            {
                rarityString = "Rare";
            }
            else if (rarity == 1)
            {
                rarityString = "Basic";
            }
            else
            {
                int type = random.Next(1, 2);
                if (type == 1)
                {
                    item = Item.HealthPotion;
                }
                else
                {
                    item = Item.ManaPotion;
                }
            }

            if (item != Item.HealthPotion && item != Item.ManaPotion)
            {
                do
                {
                    item = (Item) values.GetValue(random.Next(values.Length));
                    controls = Menu.ItemName(item);
                } while (controls[0] != rarityString);
            }

            Menu.PrintItem(item);
            Console.WriteLine("");

            //Lo aggiungo al suo inventario
            inventory.Add(item);

            return (player, inventory);
        }

        public static (Player, List<Item>) EquipItem(Player player, List<Item> inventory, Item item, bool extraSpace = false)
        {
            if (extraSpace)
            {
                Console.WriteLine();
            }
            
            switch (item)
            {
                case Item.BasicAxe:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 15;
                    player.attackMinDamage = 5;
                    break;

                case Item.RareAxe:
                    Console.Write($"[!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 40;
                    player.attackMinDamage = 10;
                    break;

                case Item.UncommonAxe:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 50;
                    player.attackMinDamage = 10;
                    break;

                case Item.EpicAxe:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 60;
                    player.attackMinDamage = 20;
                    break;

                case Item.LegendaryAxe:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 70;
                    player.attackMinDamage = 20;
                    break;

                case Item.BasicSword:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 10;
                    player.attackMinDamage = 5;
                    break;

                case Item.RareSword:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 25;
                    player.attackMinDamage = 15;
                    break;

                case Item.UncommonSword:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 35;
                    player.attackMinDamage = 25;
                    break;

                case Item.EpicSword:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 45;
                    player.attackMinDamage = 35;
                    break;

                case Item.LegendarySword:
                    Console.Write($"[>] [!] You equipped ");
                    Menu.PrintItem(item);
                    player.attackMaxDamage = 55;
                    player.attackMinDamage = 45;
                    break;

                case Item.BasicShield:
                    Console.Write($"[>] [!] You gained +{0.5} defense");
                    player.defense += 0.5f;
                    break;

                case Item.RareShield:
                    Console.Write($"[>] [!] You gained +{0.7} defense");
                    player.defense += 0.7f;
                    break;

                case Item.UncommonShield:
                    Console.Write($"[>] [!] You gained +{0.8} defense");
                    player.defense += 0.8f;
                    break;

                case Item.EpicShield:
                    Console.Write($"[>] [!] You gained +{0.9} defense");
                    player.defense += 0.9f;
                    break;

                case Item.LegendaryShield:
                    Console.Write($"[>] [!] You gained +1 defense");
                    player.defense += 1.0f;
                    break;

                case Item.HealthPotion:
                    int gained = 0;
                    if (player.health < 100)
                    {
                        for (int z = 0; z < 25; z++)
                        {
                            if (player.health < 100)
                            {
                                player.health++;
                                gained++;
                            }
                        }
                    }

                    Console.Write($"[>] [!] You gained {gained} HP");
                    break;

                case Item.ManaPotion:
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

                    Console.Write($"[>] [!] You restored {restored} mana");
                    break;
            }

            if (extraSpace)
            {
                Console.WriteLine("\n" + "[>] ");
            }

            return (player, inventory);
        }

        public static (Player, List<Item>) Rest(Player player, List<Item> inventory)
        {
            Random rnd = new Random();

            double initialHealth = player.health;
            
            if (player.stamina < 100)
            {
                while (player.stamina < 100)
                {
                    Console.CursorVisible = false;
                    
                    player.stamina += rnd.Next(7, 19);
                    if (player.stamina > 100) player.stamina = 100;
                    
                    if (player.health < 100)
                    {
                        if (rnd.Next(60) > 50)
                        {
                            player.health += rnd.Next(1, 10);
                            if (player.health > 100) player.health = 100;
                        }
                    }

                    Menu.Clear();
                    Menu.Rest(player);
                
                    System.Threading.Thread.Sleep(rnd.Next(250, 450));
                }
                
                Console.Write("\n" + "[!] You're stamina is now full");
                Console.Write(player.health - initialHealth > 0 ? " and you also restored some life!" : "!");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(String.Empty);

                List<string> notTiredMessages = new List<string>();
                
                notTiredMessages.Add("[-] Your character is not tired!");
                notTiredMessages.Add($"[-] {player.name} is not tired, you are free to explore!");
                notTiredMessages.Add("[-] You can't rest while your full of energy!");
                
                Console.WriteLine(notTiredMessages[rnd.Next(notTiredMessages.Count)]);
                
                player.currentCondition = Conditions.None;
            }

            return (player, inventory);
        }
    }
}