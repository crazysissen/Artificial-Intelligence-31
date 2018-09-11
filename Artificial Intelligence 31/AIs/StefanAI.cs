using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrettioEtt;

class StefanAI : Player
{
    //Lägg gärna till egna variabler här

    public StefanAI() //Skriv samma namn här
    {
        Name = "Stefan AI"; //Skriv in samma namn här
    }

    public override bool Knacka(int round) //Returnerar true om spelaren skall knacka, annars false
    {
        if (Game.Score(this) >= 23)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool TaUppKort(Card card) // Returnerar true om spelaren skall ta upp korten på skräphögen (card), annars false för att dra kort från leken.
    {
        if ((card.Suit == BestSuit && card.Value >= 7) || card.Value >= Status(BestSuit)[1] && card.Suit == BestSuit && Status(BestSuit)[0] == 4)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public int[] Status(Suit compareSuit)
    {
        int suitAmount = 0;
        int worstSuitValue = 0;
        for (int i = 0; i < Hand.Count; i++)
        {
            if (Hand[i].Suit == compareSuit)
            {
                if (worstSuitValue == 0 || Hand[i].Value <= worstSuitValue)
                {
                    worstSuitValue = Hand[i].Value;
                }

                suitAmount++;
            }
        }
        return new int[2] { suitAmount, worstSuitValue };
    }

    public override Card KastaKort()  // Returnerar det kort som skall kastas av de fyra som finns på handen
    {
        Game.Score(this);
        Card worstCard = null;
        for (int i = 0; i < Hand.Count; i++)
        {
            if (Hand[i].Suit != BestSuit || Status(Hand[0].Suit)[0] == 4)
            {
                if (worstCard == null || Hand[i].Value < worstCard.Value)
                {
                    worstCard = Hand[i];
                }
            }
        }
        return worstCard;

    }

    public override void SpelSlut(bool wonTheGame) // Anropas när ett spel tar slut. Wongames++ får ej ändras!
    {
        if (wonTheGame)
        {
            Wongames++;
        }

    }

    private int CardValue(Card card) // Hjälpmetod som kan användas för att värdera hur bra ett kort är
    {
        return card.Value;
    }

    // Lägg gärna till egna hjälpmetoder här
}
