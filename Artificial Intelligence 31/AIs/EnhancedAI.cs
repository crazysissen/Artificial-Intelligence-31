using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrettioEtt;

class EnhancedAI : Player
{
    // The modifiable variables
    bool 
        keepOpponentsSuit = false,
        firstTurnKnock = true;

    int
        firstTurnMinumumKnock = 14,
        minimumKnock = 15,
        lateMinimumKnock = 22;

    int gameCount;
    List<Card> scrappedCards;
    List<Card> knownOpponentCards;

    public EnhancedAI() 
    {
        Name = "Enhanced AI"; 
    }

    // Returnerar true om spelaren skall knacka, annars false
    public override bool Knacka(int round) 
    {
        totalIncrease[round] += Hand.HandValue();
        timesAdded[round]++;

        if (firstTurnKnock && round == 0 && Hand.HandValue() >= firstTurnMinumumKnock)
        {
            return true;
        }

        int currentMinimumKnock = minimumKnock;

        if (round < 12 && round > 1)
        {
            currentMinimumKnock = (int)(11 * Math.Pow(round, 0.25)) + 4;
        }

        if (round >= 12)
        {
            currentMinimumKnock = lateMinimumKnock;
        }

        if (Hand.HandValue() >= currentMinimumKnock)
        {
            return true;
        }

        return false;
    }

    // Returnerar true om spelaren skall ta upp korten på skräphögen (card), annars false för att dra kort från leken.
    public override bool TaUppKort(Card card) 
    {
        Card discardCard = EMethods.DiscardCardAdditional(Hand, card);

        if (discardCard == card)
        {
            return false;
        }

        return true;
    }

    // Returnerar det kort som skall kastas av de fyra som finns på handen
    public override Card KastaKort()  
    {
        Card discardCard = EMethods.DiscardCard(Hand);

        return discardCard;
    }

    // Anropas när ett spel tar slut. Wongames++ får ej ändras!
    public override void SpelSlut(bool wonTheGame) 
    {
        gameCount++;

        if (gameCount == 10000)
        {
            WriteAverageTurnIncrease();
        }

        if (wonTheGame)
        {
            Wongames++;
        }
    }

    static int[] timesAdded = new int[200];
    static int[] totalIncrease = new int[200];
    public static void WriteAverageTurnIncrease()
    {
        string completeString = "";
        float total = 0.0f;

        for (int i = 0; i < 200; i++)
        {
            if (timesAdded[i] == 0 && i > 10)
                break;

            float x = i;

            if (timesAdded[i] == 0)
            {
                completeString += (i == 0 ? "" : ", ") + "(" + x + ", " + 0 + ")";
                continue;
            }

            float y = totalIncrease[i] / timesAdded[i];

            completeString += (i == 0 ? "" : ", ") + "(" + x + ", " + y + ")";
        }

        System.IO.File.WriteAllText(Environment.CurrentDirectory + "/Out.txt", completeString);
    }
}

