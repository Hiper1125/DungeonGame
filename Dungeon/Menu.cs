using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dungeon
{
    public static class Menu
    {
        //Menu Navigation
        public static int Choose()
        {
            string[] continues = new[]
            {
                "[?] What do you wanna do now?",
                "[?] And now what do you wanna do?",
                "[?] What's your next move?",
                "[?] How will you continue now?",
                "[?] What will you do now?"
            };

            Random rnd = new Random();

            Console.WriteLine(continues[rnd.Next(continues.Length)]);
            Console.WriteLine(String.Empty);
            Console.WriteLine("[E] Continue the journey exploring");
            Console.WriteLine("[R] Rest and gain stamina");
            Console.WriteLine("[I] Open your inventory");
            Console.WriteLine("[P] Open your profile");
            Console.WriteLine(String.Empty);
            Console.Write("[+] Press one of the indicates keys: ");

            ConsoleKey key = Console.ReadKey().Key;
            Clear();

            switch (key)
            {
                case ConsoleKey.E:
                    PrintChoise("E", "Explore");
                    return 1;

                case ConsoleKey.I:
                    PrintChoise("I", "Inventory");
                    return 2;

                case ConsoleKey.P:
                    PrintChoise("P", "Profile");
                    return 3;

                case ConsoleKey.R:
                    PrintChoise("R", "Restore");
                    return 4;

                case ConsoleKey.Escape:
                    return 0;

                case ConsoleKey.Enter:
                    return 0;

                case ConsoleKey.Spacebar:
                    return 0;

                default: return -1;
            }
        }

        //Continue Button
        public static void Continue()
        {
            Console.Write("\n" + "[+] Press any key to continue: ");
            Console.ReadKey();
            Console.WriteLine(String.Empty);
        }

        public static void Rest(Player player)
        {
            Console.WriteLine("[!] Your character is sleeping...");

            Console.Write("\n" + "[#] STAMINA: ");
            DrawBar(player.stamina, 100, ConsoleColor.Gray);

            Console.Write("[#] HEALTH: ");
            DrawBar(player.health, 100, ConsoleColor.Red);

            Console.Write("[#] MANA: ");
            DrawBar(player.mana, 100, ConsoleColor.Blue);
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static (Player, List<Item>) GetInventory(Player player, List<Item> inventory)
        {
            int index = -1;

            //Controllo se l'inventario non sia vuoto
            if (!inventory.Any())
            {
                Console.WriteLine("\n" + "[!] You're inventory is empty");
            }
            else
            {
                var inv = true;
                var enter = false;
                Item item = Item.BasicAxe;

                //Controllo se ha solo un oggetto per la scrittura al singolare
                var s = inventory.Count == 1
                    ? "\n" + $"[!] You've got {inventory.Count} item"
                    : "\n" + $"[!] You've got {inventory.Count} items";
                
                Console.WriteLine(s);

                Console.WriteLine("\n" + "[-] Press ENTER for use item");
                Console.WriteLine("[-] Press S for sell item");
                Console.WriteLine("[-] Press ESC for quit inventory");

                Console.WriteLine("\n" + "[-] Navigate with <- or ->" + "\n");

                //Loop
                while (inv)
                {
                    Console.Write("[>] ");
                    ConsoleKey key = Console.ReadKey().Key;

                    int digits = inventory.Count.ToString().Length - 1;
                    var fmt = new String('0', digits) + new String('#', digits) + ".##";

                    //Controllo l'input
                    switch (key)
                    {
                        case ConsoleKey.RightArrow:
                            if (enter) enter = false;

                            Console.Write("\b");

                            //Controllo che non vada oltre ai limiti
                            if (index < inventory.Count - 1)
                            {
                                index = index + 1;
                            }

                            Console.Write($"[{(index + 1).ToString(fmt)}] ");
                            PrintItem(inventory[index]);
                            item = inventory[index];

                            break;

                        case ConsoleKey.LeftArrow:
                            if (enter) enter = false;
                            if (index == -1) index = 0;

                            Console.Write("\b");

                            //Controllo che non vada oltre ai limiti
                            if (index > 0)
                            {
                                index = index - 1;
                            }

                            Console.Write($"[{((index + 1).ToString(fmt))}] ");
                            PrintItem(inventory[index]);
                            item = inventory[index];

                            break;

                        case ConsoleKey.Enter:
                            if (enter) break;

                            Console.Write("\b");

                            enter = true;
                            
                            var newData = Gameplay.EquipItem(player, inventory, item);
                            player = newData.Item1;
                            inventory = newData.Item2;
                            
                            // Consumable items
                            if (item == Item.HealthPotion || item == Item.ManaPotion)
                            {
                                inventory.RemoveAt(index);
                                index--;
                            }
                            else if(ItemName(inventory[index])[1] == "Shield")
                            {
                                inventory.RemoveAt(index);
                                index--;
                            }
                            else // Non consumable items
                            {
                                inventory.Add(player.equipped);
                                player.equipped = item;
                                inventory.RemoveAt(index);
                                index--;
                            }

                            break;

                        //Se premo esc rompo il loop
                        case ConsoleKey.Escape:
                            if (enter) enter = false;
                            inv = false;
                            Menu.Clear();
                            break;

                        case ConsoleKey.S:
                            Console.Write("\b");

                            // basic axe not allowed
                            if (inventory[index] != Item.BasicAxe)
                            {
                                Console.Write($"[!] You sold ");
                                PrintItem(inventory[index]);
                                Console.Write(" for ");
                                
                                Console.BackgroundColor = ConsoleColor.Yellow;
                                Console.Write($" {(int) inventory[index]} gold! ");
                                
                                player.gold += (int) inventory[index];
                                Console.Write($" Total gold: {player.gold} ");
                                Console.BackgroundColor = ConsoleColor.DarkCyan;
                                inventory.RemoveAt(index);
                                index--;
                            }
                            else
                            {
                                Console.Write("[X] You can't sell the Basic Axe!");
                            }

                            break;
                    }

                    Console.WriteLine();
                }
            }

            return (player, inventory);
        }

        public static (Player, List<Item>) GetProfile(Player player, List<Item> inventory)
        {
            Console.WriteLine("\n" + $"[!] {player.name.ToUpper()}'S PROFILE");

            Console.WriteLine("\n" +
                              $"[!] Your character has walked {player.steps} steps.{(player.steps > 10000 ? " Impressive!" : "")}");
            Console.WriteLine(
                $"[!] Your character has reached the level {player.level}.{(player.level > 30 ? " Impressive!" : "")}");
            
            Console.Write("[!] You have ");
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write($" {player.gold} gold. ");
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.Write($"{(player.gold > 2000 ? " Impressive! \n" : "\n")}");

            Console.Write("\n" + "[#] STAMINA: ");
            DrawBar(player.stamina, 100, ConsoleColor.Gray);

            Console.Write("[#] HEALTH: ");
            DrawBar(player.health, 100, ConsoleColor.Red);

            Console.Write("[#] MANA: ");
            DrawBar(player.mana, 100, ConsoleColor.Blue);

            var itemNames = Regex.Split(player.equipped.ToString(), @"(?<!^)(?=[A-Z])");
            Console.Write("\n" + $"[*] ITEM EQUIPPED: ");
            PrintItem(player.equipped);
            Console.WriteLine();
            Console.WriteLine($"[*] DAMAGE: ({player.attackMinDamage} min)/({player.attackMaxDamage} max)");
            Console.WriteLine($"[*] DEFENSE: {player.defense}");

            return (player, inventory);
        }

        private static void PrintChoise(string symbol, string sectionName)
        {
            Console.Write($"[{symbol}] ");

            string[] outputStrings = new[]
            {
                "You've choose $!",
                "You entered the $ section!",
                "Here you are in the $ section!",
            };

            Random rnd = new Random();

            string output = outputStrings[rnd.Next(outputStrings.Length)];

            Console.WriteLine(output.Replace("$", sectionName));
        }

        public static void PrintItem(Item item)
        {
            var names = Regex.Split(item.ToString(), @"(?<!^)(?=[A-Z])");

            string rarity = names[0];

            if (rarity == "Legendary")
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
            }
            else if (rarity == "Epic")
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
            }
            else if (rarity == "Uncommon")
            {
                Console.BackgroundColor = ConsoleColor.Blue;
            }
            else if (rarity == "Rare")
            {
                Console.BackgroundColor = ConsoleColor.Green;
            }
            else if (rarity == "Basic")
            {
                Console.BackgroundColor = ConsoleColor.Gray;
            }
            else if (rarity == "Health")
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else if (rarity == "Mana")
            {
                Console.BackgroundColor = ConsoleColor.Blue;
            }

            Console.Write($" {String.Join(" ", names)} ");

            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;
        }

        private static void DrawBar(double current, double max, ConsoleColor color, int barWidth = 20)
        {
            Console.BackgroundColor = color;

            int currentWidth = (int) Math.Round((current * barWidth) / max);
            Console.Write("[");
            for (int i = 0;
                i < barWidth;
                i++)
            {
                Console.Write(i <= currentWidth ? "#" : "-");
            }

            Console.Write($"]");

            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.WriteLine($" {current}/{max}");
        }
        
        //Name formatter
        public static string[] ItemName(Item item)
        {
            return Regex.Split(item.ToString(), @"(?<!^)(?=[A-Z])");
        }
    }
}