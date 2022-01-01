using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dungeon
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();

            string username = Environment.UserName; //Username del pc dell'utente
            string fileData =
                System.AppDomain.CurrentDomain.BaseDirectory + @"data\config.txt"; //Folder di installazione dell'utente

            if (File.Exists(fileData)) //controllo se ci sia un salvataggio
            {
                var data = DataLoader(fileData); //Carico i dati nel salvataggio
                Player player = data.Item1;
                List<Item> inventory = data.Item2;

                Console.Write("[#] ");

                string[] welcomes = new[]
                {
                    "Welcome back ",
                    "Glad to see you here ",
                    "The journey is ready for you ",
                    "Enjoy your stay ",
                    "The adventure is waiting for you ",
                    "Hello again ",
                    "Oh, hello ",
                };

                string nickname = player.name;
                char[] nicknameChar = nickname.ToCharArray();

                Random rnd = new Random();

                var welcome = welcomes[rnd.Next(welcomes.Length)].ToUpper().ToCharArray();

                foreach (var element in welcome)
                {
                    Console.Write(element);
                    System.Threading.Thread.Sleep(50);
                }

                foreach (char element in nicknameChar)
                {
                    Console.Write(element);
                    System.Threading.Thread.Sleep(50);
                }

                Console.WriteLine(String.Empty);

                GameCore(player, inventory, fileData); //Avvio il gioco con i dati caricati
            }
            else
            {
                NewUser(fileData); //Creo un nuovo salvataggio
            }
        }

        private static void NewUser(string fileData)
        {
            char[] welcome1 = "Welcome in dungeon ".ToUpper().ToCharArray();
            char[] welcome2 = "Enjoy the journey ".ToUpper().ToCharArray();

            string nickname;

            Console.Write("[#] ");

            foreach (var element in welcome1)
            {
                Console.Write(element);
                System.Threading.Thread.Sleep(150);
            }

            do
            {
                System.Threading.Thread.Sleep(50);
                Console.WriteLine(String.Empty);
                Console.Write("\n" + "[?] Write a name for your character: ");

                nickname = Console.ReadLine()?.ToUpper();

                if (nickname != null)
                {
                    char[] nicknameChar = nickname.ToCharArray();

                    Console.WriteLine(String.Empty);
                    Console.Write("[#] ");

                    foreach (var element in welcome2)
                    {
                        Console.Write(element);
                        System.Threading.Thread.Sleep(50);
                    }

                    foreach (char element in nicknameChar)
                    {
                        Console.Write(element);
                        System.Threading.Thread.Sleep(50);
                    }
                }
                else
                {
                    Console.Clear();
                }
            } while (string.IsNullOrEmpty(nickname));

            Console.WriteLine(String.Empty);

            //Default values for player

            #region PlayerConfig

            Player player = new Player(nickname);
            List<Item> inventory = new List<Item>();

            #endregion

            GameCore(player, inventory, fileData); //Avvio la partita con i valori base
        }

        private static void GameCore(Player player, List<Item> inventory, string fileData)
        {
            bool ignore = false;
            bool game = true;

            while (game)
            {
                while (player.health > 0)
                {
                    DataSaver(player, inventory, fileData);
                    if (ignore == true)
                    {
                        ignore = false;
                    }
                    else
                    {
                        Console.Write("\n" + "[+] Press any key to continue: ");
                        Console.ReadKey();
                        Menu.Clear();
                    }

                    //Controllo l'input dell'utente sulla sezione del menu
                    Debug error = Debugger(Menu.Choose());

                    //Se c'è un errore lo stampo su schermo
                    if (!String.IsNullOrEmpty(error.name))
                    {
                        Console.Clear();
                        ignore = true;
                    }
                    else //Eseguo azione di gameplay in base all'input
                    {
                       (Player, List<Item>)? newData = null;
                        
                        switch (error.section)
                        {
                            case 0:
                                ignore = true;
                                Menu.Clear();
                                break;

                            case 1:
                                newData = Gameplay.Explore(player, inventory);
                                goto case -999;
                            case 2:
                                newData = Menu.GetInventory(player, inventory);
                                ignore = true;
                                goto case -999;

                            case 3:
                                newData = Menu.GetProfile(player, inventory);
                                goto case -999;
                            
                            case 4:
                                newData = Gameplay.Rest(player, inventory);
                                goto case -999;
                            
                            case -999:
                                if (newData != null)
                                {
                                    player = newData.Value.Item1;
                                    inventory = newData.Value.Item2;
                                }

                                break;
                                
                            default: goto case 0;
                        }
                    }
                }

                Console.Write("\n" + "[+] Press any key to continue: ");
                Console.ReadKey();
                Menu.Clear();

                //Setto ai dati base il player

                #region PlayerConfig

                player.level = 0;
                player.steps = 0;
                player.health = 100;
                player.stamina = 100;
                player.mana = 10;
                player.gold = 0;
                player.attackMaxDamage = 15;
                player.attackMinDamage = 5;
                player.dodgeChance = 13f;
                player.defense = 10;
                player.equipped = Item.BasicAxe;
                player.currentCondition = Conditions.None;
                inventory.Clear();

                #endregion
            }
        }

        private static void DataSaver(Player player, List<Item> inventory, string fileData)
        {
            //Creo il file config
            using (FileStream fs = File.Create(fileData))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("");
                fs.Write(info, 0, info.Length);
            }

            StreamWriter sr = new StreamWriter(fileData);

            //Scrivo le variabili sul file
            sr.WriteLine(player.name);
            sr.WriteLine(player.level);
            sr.WriteLine(player.steps);
            sr.WriteLine(player.health);
            sr.WriteLine(player.stamina);
            sr.WriteLine(player.mana);
            sr.WriteLine(player.gold);
            sr.WriteLine(player.attackMaxDamage);
            sr.WriteLine(player.attackMinDamage);
            sr.WriteLine(player.dodgeChance);
            sr.WriteLine(player.defense);
            sr.WriteLine(player.equipped);
            sr.WriteLine(player.currentCondition);

            for (int i = 0; i < inventory.Count; i++)
            {
                sr.WriteLine(inventory[i]);
            }

            //Chiudo l'editing del file     
            sr.Close();
        }

        private static (Player, List<Item>) DataLoader(string fileData)
        {
            //Apro il file
            StreamReader sr = new StreamReader(fileData);
            
            Player player = new Player(sr.ReadLine());
            List<Item> inventory = new List<Item>();

            //Leggo e assegno alle variabili
            player.level = int.Parse(sr.ReadLine() ?? string.Empty);
            player.steps = int.Parse(sr.ReadLine() ?? string.Empty);
            player.health = double.Parse(sr.ReadLine() ?? string.Empty);
            player.stamina = double.Parse(sr.ReadLine() ?? string.Empty);
            player.mana = int.Parse(sr.ReadLine() ?? string.Empty);
            player.gold = int.Parse(sr.ReadLine() ?? string.Empty);
            player.attackMaxDamage = int.Parse(sr.ReadLine() ?? string.Empty);
            player.attackMinDamage = int.Parse(sr.ReadLine() ?? string.Empty);
            player.dodgeChance = double.Parse(sr.ReadLine() ?? string.Empty);
            player.defense = double.Parse(sr.ReadLine() ?? string.Empty);
            player.equipped = (Item) System.Enum.Parse(typeof(Item), sr.ReadLine() ?? string.Empty);
            player.currentCondition = (Conditions) System.Enum.Parse(typeof(Conditions), sr.ReadLine() ?? string.Empty);

            while (!sr.EndOfStream)
            {
                inventory.Add((Item) System.Enum.Parse(typeof(Item), sr.ReadLine() ?? string.Empty));
            }

            //Chiudo l'editing del file
            sr.Close();

            return (player, inventory);
        }

        private static Debug Debugger(int e)
        {
            Debug error;

            error.name = String.Empty;
            error.section = int.MinValue;

            //Se è -1 allora c'è errore
            if (e < 0)
            {
                error.name = "[x] The typed section does not exist, retry";
                return error;
            }
            else if (e == 0)
            {
                error.section = 0; //Nope
            }
            else if (e == 1) //Explore
            {
                error.section = 1;
            }
            else if (e == 2) //Inventory
            {
                error.section = 2;
            }
            else if (e == 3) //Profile
            {
                error.section = 3;
            }
            else if(e == 4)
            {
                error.section = 4;
            }

            return error;
        }
    }
}