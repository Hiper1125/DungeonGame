using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dungeon
{
    public struct Player
    {
        public string name;
        public int level;
        public double health;
        public int mana;
        public int attackMaxDamage;
        public int attackMinDamage;
        public double dodgeChance;
        public double defense;
        public Items equipped;
        public Conditions currentCondition;
    }
    public struct Debug
    {
        public string name;
        public int section;
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();

            string username = Environment.UserName; //Username del pc dell'utente
            string fileData = System.AppDomain.CurrentDomain.BaseDirectory + @"data\config.txt"; //Folder di installazione dell'utente

            if (File.Exists(fileData))//controllo se ci sia un salvataggio
            {
                var data = DataLoader(fileData);//Carico i dati nel salvataggio
                Player player = data.Item1;
                List<Items> Inventory = data.Item2;

                Console.Write("[@] ");

                string[] welcome = { "W", "E", "L", "C", "O", "M", "E", " ", "B", "A", "C", "K", " ",};
                string nickname = player.name;
                char[] nicknameChar = nickname.ToCharArray();

                foreach (string element in welcome)
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

                GameCore(player, Inventory, fileData);//Avvio il gioco con i dati caricati
            }
            else
            {
                newUser(fileData);//Creo un nuovo salvataggio
            }
        }

        public static void newUser (string fileData)
        {
            string[] welcome1 = { "W", "E", "L", "C", "O", "M", "E", " ", "I", "N", " ", "D", "U", "N", "G", "E", "O", "N" };
            string[] welcome2 = { "W", "E", "L", "C", "O", "M", "E", " ", "A", "B", "O", "A", "R", "D", " " };

            Console.Write("[@] ");

            foreach (string element in welcome1)
            {
                Console.Write(element);
                System.Threading.Thread.Sleep(150);
            }

            System.Threading.Thread.Sleep(50);
            Console.WriteLine(String.Empty);
            Console.Write("\n" + "[?] Put here your username: ");

            string nickname = Console.ReadLine().ToUpper();
            char[] nicknameChar = nickname.ToCharArray();

            Console.WriteLine(String.Empty);
            Console.Write("[@] ");

            foreach (string element in welcome2)
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
 
            //Default values for player
            #region PlayerConfig
            Player player;
            player.name = nickname;
            player.level = 0;
            player.health = 100;
            player.mana = 10;
            player.attackMaxDamage = 15;
            player.attackMinDamage = 5;
            player.dodgeChance = 13f;
            player.defense = 10;
            player.equipped = Items.Basic_Axe;
            player.currentCondition = Conditions.None;
            List<Items> Inventory = new List<Items>();
            Inventory.Add(Items.Basic_Axe);
            #endregion

            GameCore(player, Inventory, fileData);//Avvio la partita con i valori base

        }
        
        public static void GameCore(Player player, List<Items> Inventory, string fileData)
        {
            bool ignore = false;
            bool game = true;

            while(game)
            {
                while (player.health > 0)
                {

                    DataSaver(player, Inventory, fileData);
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
                        Console.WriteLine(String.Empty);
                        Console.WriteLine(error.name);
                    }
                    else //Eseguo azione di gameplay in base all'input
                    {
                        switch (error.section)
                        {
                            case 0:
                                ignore = true;
                                Menu.Clear();
                                break;

                            case 1:
                                var a = Gameplay.Explore(player, Inventory);
                                player = a.Item1;
                                Inventory = a.Item2;
                                break;
                            case 2:
                                var b = Menu.GetInventory(player, Inventory);
                                player = b.Item1;
                                Inventory = b.Item2;
                                ignore = true;
                                break;

                            case 3:
                                var c = Menu.GetProfile(player, Inventory);
                                player = c.Item1;
                                Inventory = c.Item2;
                                break;
                        }
                    }
                }

                Console.Write("\n" + "[+] Press any key to continue: ");
                Console.ReadKey();
                Menu.Clear();

                //Setto ai dati base il player
                #region PlayerConfig
                player.level = 0;
                player.health = 100;
                player.mana = 10;
                player.attackMaxDamage = 15;
                player.attackMinDamage = 5;
                player.dodgeChance = 13f;
                player.defense = 10;
                player.equipped = Items.Basic_Axe;
                player.currentCondition = Conditions.None;
                Inventory.Clear();
                Inventory.Add(Items.Basic_Axe);
                #endregion
            }

        }

        public static void DataSaver(Player player, List<Items> Inventory, string fileData)
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
            sr.WriteLine(player.health);
            sr.WriteLine(player.mana);
            sr.WriteLine(player.attackMaxDamage);
            sr.WriteLine(player.attackMinDamage);
            sr.WriteLine(player.dodgeChance);
            sr.WriteLine(player.defense);
            sr.WriteLine(player.equipped);
            sr.WriteLine(player.currentCondition);

            for(int i = 0; i < Inventory.Count; i++)
            {
                sr.WriteLine(Inventory[i]);
            }

        //Chiudo l'editing del file     
        sr.Close();
        }

        public static (Player, List<Items>) DataLoader(string fileData)
        {
            Player player;
            List<Items> Inventory = new List<Items>();

            //Apro il file
            StreamReader sr = new StreamReader(fileData);

            //Leggo e assegno alle variabili
            player.name = sr.ReadLine();
            player.level = int.Parse(sr.ReadLine());
            player.health = double.Parse(sr.ReadLine());
            player.mana = int.Parse(sr.ReadLine());
            player.attackMaxDamage = int.Parse(sr.ReadLine());
            player.attackMinDamage = int.Parse(sr.ReadLine());
            player.dodgeChance = double.Parse(sr.ReadLine());
            player.defense = double.Parse(sr.ReadLine());
            player.equipped = (Items)System.Enum.Parse(typeof(Items), sr.ReadLine());
            player.currentCondition = (Conditions)System.Enum.Parse(typeof(Conditions), sr.ReadLine());

            while(!sr.EndOfStream)
            {
                Inventory.Add((Items)System.Enum.Parse(typeof(Items), sr.ReadLine()));
            }

            //Chiudo l'editing del file
            sr.Close();

            return (player, Inventory);
        }

        public static Debug Debugger(int e)
        {
            Debug error;

            error.name = String.Empty;
            error.section = int.MinValue;

            //Se è -1 allora c'è errore
            if (e < 0)
            {
                error.name = "\n" + "[x] The typed section does not exist, retry";
                return error;
            }
            else if(e == 0)
            {
                error.section = 0; //Ignoro l'input
            }
            else if (e == 1) //Setto sezione a esplorazione
            {
                error.section = 1;
            }
            else if (e == 2) //Setto sezione a inventario
            {
                error.section = 2;
            }
            else if (e == 3)//Setto sezione a profilo
            {
                error.section = 3;
            }

            return error;

        }


    }
}