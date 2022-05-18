using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FinalProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = (@"C:\Users\Nick\Desktop\FinalProject\player_log.csv");
            
            List<Player> playerList = PlayerLoader.Load(filePath);
            
            while(true){
                Console.WriteLine("Welcome to Rock, Paper, Scissors!\n\n    1. Start New Game\n    2. Load Game\n    3. Quit\n\nEnter Choice: ");
                string response = Console.ReadLine();
                
                if(response == "1"){
                    newGame(playerList);
                    break;
                }else if(response == "2"){
                    loadGame(playerList);
                }else if(response == "3"){
                    Environment.Exit(0);
                }
            }
        }

        static string viewLeaderboard(List<Player> playerList){
            string report = "\n----------------------\nGlobal Game Statistics\n----------------------\n";

            if(playerList.Count() < 1){
                report += "No data is available.\n";
                return report;
            }

            report += "\n----------------------\nTop 10 Winning Players\n----------------------\n";
            var topTenWinningPlayers = (from player in playerList orderby player.Win descending select player).Take(10);
            foreach(var player in topTenWinningPlayers){
                report += $"{player.Name}: {player.Win}\n";
            }

            report += "\n----------------------\nMost Games Played\n----------------------\n";
            var mostGamesPlayed = (from player in playerList group player by player.getGamesPlayed(player.Win, player.Loss, player.Tie) into mostGames
            orderby mostGames.Key descending select mostGames).Take(5);
            foreach(var player in mostGamesPlayed){
                foreach(var i in player){
                    report += $"{i.Name}: ";
                }
                report += $"{player.Key} games played\n";
            }

            var winLossRatio = from player in playerList select player;
            float totalWin = 0;
            float totalLoss = 0;
            float totalTie = 0;
            foreach(var player in winLossRatio){
                totalWin += player.Win;
                totalLoss += player.Loss;
                totalTie += player.Tie;
            }
            float totalRatio = totalWin / totalLoss;
            report += $"\n----------------------\nWin/Loss Ratio: {totalRatio.ToString("0.00")}\n----------------------\n";

            float totalGames = totalWin + totalLoss + totalTie;
            report += $"\n----------------------\nTotal Games Played: {totalGames}\n----------------------\n";

            return report;

        }

        static void saveGame(Player player, List<Player> playerList){
            var fileName = "player_log.csv";
            using (var sw = new StreamWriter(fileName)){
                try{
                    foreach(var players in playerList){
                        sw.WriteLine($"{players.Name},{players.Win},{players.Loss},{players.Tie}");
                    }
                }catch(Exception){
                    Console.WriteLine($"Sorry {player.Name}, the game could not be saved.");
                }finally{
                    if(sw != null){
                        sw.Close();
                    }
                }
            }
            Console.WriteLine($"{player.Name}, your game has been saved.");
        }

        static void gameStatistics(Player player){
            Console.WriteLine($"\n{player.Name} here are your game play statistics: ");
            Console.WriteLine($"Wins: {player.Win.ToString()}\nLosses: {player.Loss.ToString()}\nTies: {player.Tie.ToString()}");
            if(player.Loss != 0){
                float ratio = player.Win / player.Loss;
                Console.WriteLine(String.Format($"Win/Loss Ratio: {ratio}"));
            }else{
                Console.WriteLine($"Win/Loss Ratio: {player.Win.ToString()}");
            }
        }

        static void loadGame(List<Player> playerList){
            Console.WriteLine("What is your name?");
            Player currentPlayer = null;
            string loadPlayer = Console.ReadLine();
            bool playerFound = false;
            foreach (var player in playerList){
                if(player.Name == loadPlayer){ 
                    currentPlayer = player; 
                    playerFound = true;
                    break;
                }
            }
            if(playerFound == true){
                Console.WriteLine($"\nWelcome back {loadPlayer}. Let's play!");
                gamePlay(currentPlayer, playerList);
            }else{
                Console.WriteLine($"\n{loadPlayer} not found");
            }
        }

        static void gamePlay(Player player, List<Player> playerList){
            Dictionary<int, string> gameChoices = new Dictionary<int, string>(){
                {1,"Rock"}, {2, "Paper"}, {3, "Scissors"}
            };
            Dictionary<string, string> rpsDictionary = new Dictionary<string, string>(){
                {"Rock", "Scissors"}, {"Paper", "Rock"}, {"Scissors", "Paper"}
            };
            var choices = new List<object>{
                "1", "2", "3"
            }; 
            Random rnd = new Random();
            var roundNumber = player.Win + player.Loss + player.Tie + 1;
            
            
            Console.WriteLine($"Round {roundNumber}\n");
            Console.WriteLine("    1. Rock\n    2. Paper\n    3.Scissors\n\nWhat will it be?");
            string response = Console.ReadLine();

            while(!choices.Contains(response)){
                Console.WriteLine($"{response} is not a valid choice.\nPlease enter a value between 1 and 3. ");
                response = Console.ReadLine();
            }

            var userChoice = gameChoices[Convert.ToInt32(response)];
            var computerChoice = gameChoices[rnd.Next(1, 3)];
            Console.WriteLine($"\nYou chose {userChoice}. The computer chose {computerChoice}.");

            if(computerChoice == rpsDictionary[userChoice]){
                player.Win += 1;
                Console.WriteLine("You win!");
            }else if(computerChoice == userChoice){
                player.Tie += 1;
                Console.WriteLine("You tied!");
            }else{
                player.Loss += 1;
                Console.WriteLine("You lost!");
            }

            gamePlayMenu(player, playerList);
        }
        static void gamePlayMenu(Player player, List<Player> playerList){
            var choices = new List<object>{
                "1", "2", "3", "4"
            };

            while(true){
                Console.WriteLine("\nWhat would you like to do?\n    1. Play again\n    2. View Player Statistics\n    3. View Leaderboard\n    4. Quit\nEnter choice: ");
                string response = Console.ReadLine();

                while(!choices.Contains(response)){
                    Console.WriteLine($"'{response}' is not a valid choice.\nPlease enter a value between 1 and 4. ");
                    response = Console.ReadLine();
                }
                if(response == "1"){
                    gamePlay(player, playerList);
                }
                else if(response == "2"){
                    gameStatistics(player);
                    gamePlayMenu(player, playerList);
                }
                else if(response == "3"){
                    var report = viewLeaderboard(playerList);
                    Console.WriteLine(report);
                    gamePlayMenu(player, playerList);
                }
                else{
                    saveGame(player, playerList);
                    Environment.Exit(0);
                }
                break;
            }
        }

        static void newGame(List<Player> playerList){
            Console.WriteLine("What is your name? ");
            string playerName = Console.ReadLine();
            while(true){
                foreach(var player in playerList){
                    if(player.Name == playerName){
                        Console.WriteLine("\nA user with that name already exists.\nPlease choose a new name. ");
                        playerName = Console.ReadLine();
                        break;
                    }else{
                        continue;
                    }
                }
                Console.WriteLine($"\nHello {playerName}. Let's play!\n");

                Player playerNew = new Player(playerName, 0, 0, 0);
                playerList.Add(playerNew);

                gamePlay(playerNew, playerList);
            }
        }
    }
}
