using ScopaBot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScopaBot
{
    public class Program
    {
        private static List<Card> deck;
        private static List<Card> playerHand = new List<Card>();
        private static List<Card> playerCollection = new List<Card>();
        private static int playerPoints = 0;
        private static List<Card> botHand = new List<Card>();
        private static List<Card> botCollection = new List<Card>();
        private static int botPoints = 0;
        private static List<Card> pot = new List<Card>();
        static void Main(string[] args)
        {
                deck = GetShuffledDeck();

                //Start Round
                Console.WriteLine("Press any key to start the game...");
                Console.ReadKey();
                SetupPot();

                while (deck.Any())
                {
                    DealHands();
                    Console.WriteLine("\nCards have been dealt! Press any key to start hand!");
                    Console.ReadKey();
                    while (playerHand.Count > 0)
                    {
                        Console.Clear();

                        //Display pot
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("Pot:");
                        Console.ResetColor();
                        PrintPot(pot);

                        Console.WriteLine("\n");

                        //Player Turn
                        PlayerTurn();

                        //Bot Turn
                        BotTurn();

                        if(playerHand.Count != 0)
                        {

                            Console.WriteLine("\nPress any key to start your turn!");
                            Console.ReadKey();
                        }
                    }
                }

                Endgame();
        }



        //Game Setup
        static List<Card> GetShuffledDeck()
        {
            List<string> suits = new List<string> { "Swords", "Cups", "Coins", "Clubs" };
            List<int> values = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<Card> deck = new List<Card>();

            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    deck.Add(new Card(value, suit));
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
        public static void Endgame()
        {
            //Figure out who won
            Console.WriteLine("\nEnd of game!");
            Console.ReadLine();
        }
        public static void CalculateScore()
        {
            //Count Points
        }



        // Turns
        static void PlayerTurn()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Your turn! Enter the index of the card you want to play:");
            Console.ResetColor();

            PrintPlayerHand(playerHand);
            int index = int.Parse(Console.ReadLine()) - 1;

            Card playedCard = playerHand[index];
            playerHand.RemoveAt(index);
            CalculateMove(playedCard);
        }
        static void BotTurn()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nBot turn!");
            Console.ResetColor();

            Random rand = new Random();
            var playedCard = botHand[rand.Next(0, botHand.Count)];
            botHand.Remove(playedCard);

            var combinations = FindCombinations(pot, playedCard);
            var collection = new List<Card>();

            if (combinations.Any())
            {
                if (combinations.Count > 1)
                {

                    var selectedCombination = combinations[rand.Next(0, combinations.Count())];
                    foreach (var card in selectedCombination)
                    {
                        collection.Add(card);
                        pot.Remove(card);
                    }
                }
                else
                {
                    foreach (var card in combinations[0])
                    {
                        collection.Add(card);
                        pot.Remove(card);
                    }
                }
                Console.WriteLine($"Bot played {playedCard.Value} of {playedCard.Suit}.");
                collection.Add(playedCard);

                playerCollection.AddRange(collection);
                Console.WriteLine("Bot collected: ");
                PrintPot(collection);
            }
            else
            {
                pot.Add(playedCard);
                Console.WriteLine($"Bot played {playedCard.Value} of {playedCard.Suit} and collected nothing.");
            }
        }
        static void CalculateMove(Card playedCard)
        {
            var combinations = FindCombinations(pot, playedCard);
            var collection = new List<Card>();

            //Check if theres any possible moves
            if (combinations.Any())
            {
                //If only 1 option skip the choice of combo
                if (combinations.Count > 1)
                {
                    Console.WriteLine($"\nYou played {playedCard.Value} of {playedCard.Suit}, pick an option: ");
                    PrintCombinations(combinations);
                    int index = int.Parse(Console.ReadLine()) - 1;

                    var selectedCombination = combinations[index];
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You collected: ");
                Console.ResetColor();
                PrintPot(collection);
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
                playerPoints++;
                Console.WriteLine("You got a Scopa!");
            }

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
                    FindCombinationsHelper(arr, new Card(target.Value - arr[i].Value, target.Suit), i + 1, currentCombination, result);
                    currentCombination.RemoveAt(currentCombination.Count - 1);
                }
            }
        }

        // Printer Functions
        static void PrintPot(List<Card> hand)
        {
            foreach (var card in hand)
            {
                Console.WriteLine($"{card.Value} of {card.Suit}");
            }
        }

        static void PrintPlayerHand(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {cards[i].Value} of {cards[i].Suit}");
            }
        }

        static void PrintCombinations(List<List<Card>> combinations)
        {
            for (int i = 0; i < combinations.Count; i++)
            {
                Console.WriteLine($"{i + 1}) [ {string.Join(", ", combinations[i].Select(card => $"{card.Value} of {card.Suit}"))} ]");
            }
        }
    }
}