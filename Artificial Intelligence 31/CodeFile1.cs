using System;
using System.Collections.Generic;
using System.Linq;
using TrettioEtt;

namespace TrettioEtt
{
    class Card
    {
        public int Value { get; private set; } //Kortets värde enligt reglerna i Trettioett, t.ex. dam = 10
        public Suit Suit { get; private set; }
        private int Id; //Typ av kort, t.ex dam = 12

        public Card(int id, Suit suit)
        {
            Id = id;
            Suit = suit;
            if (id == 1)
            {
                Value = 11;
            }
            else if (id > 9)
            {
                Value = 10;
            }
            else
            {
                Value = id;
            }
        }

        public void PrintCard()
        {
            string cardname = "";
            if (Id == 1)
            {
                cardname = "ess ";
            }
            else if (Id == 10)
            {
                cardname = "tio ";
            }
            else if (Id == 11)
            {
                cardname = "knekt ";
            }
            else if (Id == 12)
            {
                cardname = "dam ";
            }
            else if (Id == 13)
            {
                cardname = "kung ";
            }
            if (Suit == Suit.Hjärter)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (Suit == Suit.Ruter)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (Suit == Suit.Spader)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (Suit == Suit.Klöver)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.Write(" " + Suit + " " + cardname);
            if (cardname == "")
            {
                Console.Write(Id + " ");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}