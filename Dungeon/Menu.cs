using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public static class Menu
    {
        public static int Choose()
        {
            
            Console.WriteLine("[?] What do you wanna do now?");
            Console.WriteLine(String.Empty);
            Console.Write("[Explore] ");
            Console.Write("[Inventory] ");
            Console.Write("[Profile]");
            Console.WriteLine(String.Empty);
            Console.Write("\n" + "[+] Press one key [E, I , P]: ");
            ConsoleKey key =  Console.ReadKey().Key;

            switch(key)
            {

                case ConsoleKey.E:
                    Console.WriteLine(String.Empty);
                    Console.WriteLine("\n" + "[E] You've choose Explore");
                    return 1;

                case ConsoleKey.I:
                    Console.WriteLine(String.Empty);
                    Console.WriteLine("\n" + "[I] You've choose Inventory");
                    return 2;

                case ConsoleKey.P:
                    Console.WriteLine(String.Empty);
                    Console.WriteLine("\n" + "[I] You've choose Profile");
                    return 3;

                case ConsoleKey.Escape:
                    return 0;

                case ConsoleKey.Enter:
                    return 0;

                case ConsoleKey.Spacebar:
                    return 0;

                default: return -1;
            }
       
        }//Menu Nav
        public static void Continue()
        {
            Console.Write("\n" + "[+] Press any key to continue: ");
            Console.ReadKey();
            Console.WriteLine(String.Empty);
        }//Continue Button
        public static void Clear()
        {
            Console.Clear();
        }//Clear

        public static (Player, List<Items>) GetInventory(Player player, List<Items> Inventory)
        {

            int index = 0;

            //Controllo se l'inventario non sia vuoto
            if (!Inventory.Any())
            {
                Console.WriteLine("\n" + "[!] You're inventory is empty");
            }
            else
            {

                var inv = true;
                var enter = false;
                Items item = Items.Basic_Axe;

                //Controllo se ha solo un oggetto per la scrittura al singolare
                var s = Inventory.Count == 1 ? "\n" + $"You've got {Inventory.Count} item" : "\n" + $"You've got { Inventory.Count } items";

                Console.WriteLine(s);
                Console.WriteLine("\n" + "[-] Navigate with <- or ->");
                Console.WriteLine("\n" + "[-] Press ENTER for use item");
                Console.WriteLine("\n" + "[-] Press ESC for quit inventory");
                Console.WriteLine(String.Empty);

                //Loop
                while (inv)
                {
                    Console.Write("\n" + "[>]");
                    ConsoleKey key = Console.ReadKey().Key;

                    //Controllo l'input
                    switch (key)
                    {

                        case ConsoleKey.RightArrow:
                            if (enter) enter = false;
                            //Controllo che non vada oltre ai limiti
                            if (index <= Inventory.Count)
                            {
                                //Se arrivo alla fine rompo il controllo
                                if (index == Inventory.Count)
                                {
                                    break;
                                }
                                else
                                {
                                    Console.Write($"[{index}] {Inventory[index].ToString().Replace('_', ' ')}");
                                    item = Inventory[index];
                                    index = index == Inventory.Count ? index : index + 1;
                                }
                            }
                            break;

                        case ConsoleKey.LeftArrow:
                            if (enter) enter = false;
                            //Controllo che non vada oltre ai limiti
                            if (index >= 0)
                            {
                                //Se arrivo all'inizio rompo il controllo
                                if (index == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    Console.Write($"[{index-1}] {Inventory[index-1].ToString().Replace('_', ' ')}");
                                    item = Inventory[index-1];
                                    index = index == 0 ? index : index - 1;
                                }
                            }
                            break;

                        case ConsoleKey.Enter:
                            if (enter) break;
                            if (!enter) enter = true;
                            var a = Gameplay.UseItem(player, Inventory, item);
                            player = a.Item1;
                            Inventory = a.Item2;
                            //Se è un consumabile diminuisco di 1 l'index perchè lo ho consumato
                            if (item == Items.Health_Potion || item == Items.Mana_Potion)
                            {
                                index--;
                            }
                            else
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    string[] usable = item.ToString().Split('_');
                                    if (usable[1] == "Shield")
                                    {
                                        index--;
                                        break;
                                    }
                                }
                            }
                            break;

                        //Se premo esc rompo il loop
                        case ConsoleKey.Escape:
                            if (enter) enter = false;
                            inv = false;
                            Menu.Clear();
                            break;
                    }

                }

            }

            return (player, Inventory);

        }//Get Inventory Function
        public static (Player, List<Items>) GetProfile(Player player, List<Items> Inventory)
        {
            Console.WriteLine("\n" + $"[{player.name}'S PROFILE]");
            Console.WriteLine("\n" + $"[-] LEVEL: {player.level}");
            Console.WriteLine("\n" + $"[-] HEALTH: {player.health}");
            Console.WriteLine("\n" + $"[-] MANA: {player.mana}");
            Console.WriteLine("\n" + $"[-] DEFENSE: {player.defense}");
            Console.WriteLine("\n" + $"[-] ITEM EQUIPPED: {player.equipped.ToString().Replace('_', ' ')}");
            Console.WriteLine("\n" + $"[-] MAX DAMAGE: {player.attackMaxDamage}");
            Console.WriteLine("\n" + $"[-] MIN DAMAGE: {player.attackMinDamage}");
            return (player, Inventory);
        }//Get Profile Function
    }
}