using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopaBot
{
    class Card
    {
        public int Value { get; }
        public string Suit { get; }
        public int Primeri { get; }

        public Card(int value, string suit)
        {
            Value = value;
            Suit = suit;
        }
    }
}
