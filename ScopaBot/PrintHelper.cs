using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopaBot
{
    internal static class PrintHelper
    {
        public static void PrintPot(List<Card> hand)
        {
            foreach (var card in hand)
            {
                Console.WriteLine($"{card.Value} of {card.Suit}");
            }
        }

        public static void PrintPlayerHand(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {cards[i].Value} of {cards[i].Suit}");
            }
        }

        public static void PrintCombinations(List<List<Card>> combinations)
        {
            for (int i = 0; i < combinations.Count; i++)
            {
                Console.WriteLine($"{i + 1}) [ {string.Join(", ", combinations[i].Select(card => $"{card.Value} of {card.Suit}"))} ]");
            }
        }
        public static void PrintTable(object[,] tableData, int playerPoints, int botPoints)
        {
            Console.WriteLine("\n");

            // Determine column widths based on the data
            int rows = tableData.GetLength(0);
            int columns = tableData.GetLength(1);
            int[] columnWidths = new int[columns];

            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    int length = GetDisplayLength(tableData[row, col]);
                    if (length > columnWidths[col])
                    {
                        columnWidths[col] = length;
                    }
                }
            }

            // Print the table headers
            for (int col = 0; col < columns; col++)
            {
                Console.Write(tableData[0, col].ToString().PadRight(columnWidths[col] + 2));
            }
            Console.WriteLine();

            // Print the line between header and content
            for (int col = 0; col < columns; col++)
            {
                Console.Write(new string('-', columnWidths[col] + 2));
            }
            Console.WriteLine();

            // Print the table rows
            for (int row = 1; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Console.Write(GetFormattedValue(tableData[row, col], columnWidths[col]).PadRight(columnWidths[col] + 2));
                }
                Console.WriteLine();
            }
        }

        private static int GetDisplayLength(object value)
        {
            return value.ToString().Length;
        }

        private static string GetFormattedValue(object value, int width)
        {
            if (value is int intValue)
            {
                return intValue.ToString().PadLeft(width);
            }
            return value.ToString().PadRight(width);
        }
    }
}
