﻿using System;
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
        public int Prime { get; }

        public Card(int value, string suit,int prime)
        {
            Value = value;
            Suit = suit;
            Prime = prime;
        }
    }
}
