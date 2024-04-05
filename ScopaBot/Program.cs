using ScopaBot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScopaBot
{
    public class Program
    {
        private static List<string> SUITS = new List<string> { "Swords", "Cups", "Coins", "Clubs" };
        private static List<int> VALUES = new List<int> { 7, 6, 1, 5, 4, 3, 2, 8, 9, 10 };
        private static List<int> PRIMES = new List<int> { 21, 18, 16, 15, 14, 13, 12, 10, 10, 10 };

        private static List<Card> deck;
        private static List<Card> pot = new List<Card>();

        private static List<Card> playerHand = new List<Card>();
        private static List<Card> playerCollection = new List<Card>();
        private static int playerPoints = 0;
        private static int playerScopas = 0;

        private static List<Card> botHand = new List<Card>();
        private static List<Card> botCollection = new List<Card>();
        private static int botPoints = 0;
        private static int botScopas = 0;

        private static bool playerLastCollection = false;

        private static int handCount = 0;

        static void Main(string[] args)
        {
            deck = GetShuffledDeck();

            //Start Round
            Console.WriteLine("Press any key to start the game...");
            Console.ReadKey();
            SetupPot();


            //for testing!
                //while (true)
                //{

                //    DealHands();
                //    while (botHand.Count > 0)
                //    {
                //        Console.Clear();

                //        Console.WriteLine("Pot:");
                //        PrintHelper.PrintPot(pot);

                //        Console.WriteLine("\nBot Hand:");
                //        PrintHelper.PrintPlayerHand(botHand);
                //        BotTurn();
                //    }
                //}
            //

            while (deck.Any())
            {
                DealHands();

                handCount++;

                Console.WriteLine("\nCards have been dealt! Press any key to start hand!");
                Console.ReadKey();
                while (playerHand.Count > 0)
                {
                    Console.Clear();

                    Console.WriteLine($"Hand {handCount} of 6");
                    //Display pot
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\nPot:");
                    Console.ResetColor();
                    PrintHelper.PrintPot(pot);

                    Console.WriteLine("\n");

                    //Player Turn
                    PlayerTurn();

                    //Bot Turn
                    BotTurn();

                    if (playerHand.Count != 0)
                    {

                        Console.WriteLine("\nPress any key to start your turn!");
                        Console.ReadKey();
                    }
                }
            }

            // End game, give pot to player who made last collection
            if (playerLastCollection && pot.Any())
            {
                playerCollection.AddRange(pot);
            }
            else
            {
                botCollection.AddRange(pot);
            }

            // Figure out who won
            Console.Clear();
            Console.WriteLine("\nEnd of game!");
            CalculatePoints();

            Console.ReadKey();
        }



        //Game Setup
        static List<Card> GetShuffledDeck()
        {

            List<Card> deck = new List<Card>();

            foreach (var suit in SUITS)
            {
                for (int i = 0; i < VALUES.Count; i++)
                {

                    deck.Add(new Card(VALUES[i], suit, PRIMES[i]));
                }
            }

            var random = new Random();
            return deck.OrderBy(card => random.Next()).ToList();
        }
        static void DealHands()
        {
            for (int i = 0; i < 3; i++)
            {
                playerHand.Add(deck[0]);
                deck.RemoveAt(0);

                botHand.Add(deck[0]);
                deck.RemoveAt(0);
            }
        }
        public static void SetupPot()
        {
            for (int i = 0; i < 4; i++)
            {
                pot.Add(deck[0]);
                deck.RemoveAt(0);
            }
        }


        // End Game
        public static void CalculatePoints()
        {
            bool coinTie = false;
            bool primeTie = false;

            //Amount of cards
            if (playerCollection.Count() > botCollection.Count())
            {
                playerPoints++;
            }
            else if (playerCollection.Count < botCollection.Count())
            {
                botPoints++;
            }

            //Sette Bello
            if (playerCollection.Any(card => card.Value == 7 && card.Suit == "Coins"))
            {
                playerPoints++;
            }
            else
            {
                botPoints++;
            }

            //Coins
            if (playerCollection.Count(card => card.Suit == "Coins") > botCollection.Count(card => card.Suit == "Coins"))
            {
                playerPoints++;
            }
            else if (playerCollection.Count(card => card.Suit == "Coins") < botCollection.Count(card => card.Suit == "Coins"))
            {
                botPoints++;
            }

            // Prime
            var playerPrime = getPrime(playerCollection);
            var botPrime = getPrime(botCollection);

            if (playerPrime > botPrime)
            {
                playerPoints++;
            }
            else if (botPrime > playerPrime)
            {
                botPoints++;
            }

            //Add Scopas
            playerPoints += playerScopas;
            botPoints += botScopas;

            object[,] tableData = {
                {"Point", "You", "Bot"},
                {"Cards", playerCollection.Count(), botCollection.Count() },
                {"Coins", playerCollection.Count(card => card.Suit == "Coins"), botCollection.Count(card => card.Suit == "Coins")},
                {"Sette Bello", playerCollection.Any(card => card.Value == 7 && card.Suit == "Coins").ToString(), botCollection.Any(card => card.Value == 7 && card.Suit == "Coins").ToString()},
                {"Prime",getPrime(playerCollection),getPrime(botCollection) },
                { "Scopas",playerScopas,botScopas},
                {"Total",playerPoints,botPoints }
            };

            PrintHelper.PrintTable(tableData,playerPoints,botPoints);

            if (playerPoints > botPoints)
            {
                Console.WriteLine("\nPlayer wins!");
            }
            else if(botPoints > playerPoints)
            {
                Console.WriteLine("\nBot wins.");
            }
            else
            {
                Console.WriteLine("\nNo one wins!");
            }
        }
        static int getPrime(List<Card> collection)
        {
            var prime = 0;
            List<Card> primeCards = [];
            foreach (string suit in SUITS)
            {
                foreach (var val in VALUES)
                {
                    if (collection.Any(card => card.Suit == suit && card.Value == val))
                    {
                        var card = collection.First(card => card.Suit == suit && card.Value == val);
                        prime += card.Prime;
                        primeCards.Add(card);
                        break;
                    }
                }
            }
            return prime;
        }


        // Turns
        static void PlayerTurn()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Your turn! Enter the index of the card you want to play:");
            Console.ResetColor();

            PrintHelper.PrintPlayerHand(playerHand);
            int handIndex = int.Parse(Console.ReadLine()) - 1;

            Card playedCard = playerHand[handIndex];
            playerHand.RemoveAt(handIndex);


            var combinations = FindCombinations(pot, playedCard);
            var collection = new List<Card>();

            //Check if theres any possible moves
            if (combinations.Any())
            {
                //If only 1 option skip the choice of combo
                if (combinations.Count > 1)
                {
                    Console.WriteLine($"\nYou played {playedCard.Value} of {playedCard.Suit}, pick an option: ");
                    PrintHelper.PrintCombinations(combinations);
                    int combinationIndex = int.Parse(Console.ReadLine()) - 1;

                    var selectedCombination = combinations[combinationIndex];
                    foreach (var card in selectedCombination)
                    {
                        collection.Add(card);
                        pot.Remove(card);
                    }
                }
                else
                {
                    Console.WriteLine($"\nYou played {playedCard.Value} of {playedCard.Suit}.");
                    foreach (var card in combinations[0])
                    {
                        collection.Add(card);
                        pot.Remove(card);
                    }
                }

                collection.Add(playedCard);
                playerCollection.AddRange(collection);
                playerLastCollection = true;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You collected: ");
                Console.ResetColor();

                PrintHelper.PrintPot(collection);
            }
            else
            {
                // No collection, just drop the card in the pot
                pot.Add(playedCard);
                Console.WriteLine($"\nYou played {playedCard.Value} of {playedCard.Suit}, and collected nothing.");
            }

            //Scopa Check
            if (!pot.Any())
            {
                playerScopas++;
                Console.WriteLine("You got a Scopa!");
            }
        }
        static void BotTurn()
        {
            List<List<List<Card>>> possibilities = new List<List<List<Card>>>();

            // Check combinations for every possible card that can be played
            foreach (var playableCard in botHand)
            {
                var combos = FindCombinations(pot, playableCard);
                possibilities.Add(combos);
            }

            List<Card> bestCombo = null;
            Card bestCardToPlay = null;

            // Evaluate possible plays
            int bestScore = -1;
            for (int i = 0; i < botHand.Count; i++)
            {
                var playableCard = botHand[i];
                var combos = possibilities[i];

                foreach (var combo in combos)
                {
                    var score = EvaluateCombo(combo, playableCard);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestCombo = combo;
                        bestCardToPlay = playableCard;
                    }
                }
            }

            if (bestCardToPlay != null && bestCombo != null)
            {
                botHand.Remove(bestCardToPlay);
                foreach (var card in bestCombo)
                {
                    pot.Remove(card);
                }
                botCollection.Add(bestCardToPlay);
                botCollection.AddRange(bestCombo);
                playerLastCollection = false;

                Console.Write($"\nBot played ");
                PrintHelper.PrintCard(bestCardToPlay);
                Console.Write(" and collected: \n");
                PrintHelper.PrintPot(bestCombo);
            }
            else
            {
                Card bestCardToDrop = null;
                if (botHand.Count > 1)
                {
                    int bestDropScore = 100;

                    foreach (var droppableCard in botHand)
                    {
                        int score = EvaluateDrop(droppableCard);

                        if(score < bestDropScore)
                        {
                            bestDropScore = score;
                            bestCardToDrop = droppableCard;
                        }
                    }
                }
                else
                {
                    bestCardToDrop = botHand[0];
                }

                botHand.Remove(bestCardToDrop);
                pot.Add(bestCardToDrop);

                Console.Write($"\nBot played ");
                PrintHelper.PrintCard(bestCardToDrop);
                Console.Write(" and collected nothing.\n");
            }

            //Scopa Check
            if (!pot.Any())
            {
                botScopas++;
                Console.WriteLine("\nBot got a Scopa.");
            }
        }
        static int EvaluateDrop(Card droppableCard)
        {
            int score = 0;

            var potWithDrop = new List<Card>(pot);

            potWithDrop.Add(droppableCard);

            var potential7Combos = FindCombinations(potWithDrop, new Card(7, "Coins", 21));
            foreach(var combo in potential7Combos)
            {
                score--;
            }

            if(droppableCard.Suit == "Coin")
            {
                score--;
            }

            score += droppableCard.Value;
            return score;
        }
        static int EvaluateCombo(List<Card> possibleCollection, Card playedCard)
        {
            int score = 0;

            //Evaluate what could be picked up
            foreach (var card in possibleCollection)
            {
                if (card.Suit == "Coins" && card.Value == 7)
                {
                    score++;
                }
                else if (card.Suit == "Coins")
                {
                    score++;
                }
                score++;
                score += card.Prime;
            }

            //Evaluate what could be put down
            if (playedCard.Suit == "Coins" && playedCard.Value == 7)
            {
                score++;
            }
            else if (playedCard.Suit == "Coins")
            {
                score++;
            }
            score++;
            score += playedCard.Prime;


            return score;
        }




        // Combos
        static List<List<Card>> FindCombinations(List<Card> pot, Card x)
        {
            List<List<Card>> result = new List<List<Card>>();
            FindCombinationsHelper(pot, x, 0, new List<Card>(), result);
            return result;
        }

        static void FindCombinationsHelper(List<Card> arr, Card target, int startIndex, List<Card> currentCombination, List<List<Card>> result)
        {
            if (target.Value == 0)
            {
                result.Add(new List<Card>(currentCombination));
                return;
            }

            for (int i = startIndex; i < arr.Count; i++)
            {
                if (target.Value >= arr[i].Value)
                {
                    currentCombination.Add(arr[i]);
                    FindCombinationsHelper(arr, new Card(target.Value - arr[i].Value, target.Suit, target.Prime), i + 1, currentCombination, result);
                    currentCombination.RemoveAt(currentCombination.Count - 1);
                }
            }
        }
    }
}