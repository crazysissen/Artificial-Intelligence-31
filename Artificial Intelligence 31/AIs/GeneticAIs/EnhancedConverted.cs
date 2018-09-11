using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrettioEtt;

class EnhancedConverted : GeneticAI
{
    public override Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>
    {
        { "functionC", 0.0 },
        { "functionA", 0.0 },
        { "functionM", 0 },
        { "turnOverRound", 0 },
        { "turnOverMinimum", 0 },
        { "firstKnockMinimum", 0 },
        { "firstKnock", false }
    };

    List<Card> knownOpponentCards;
    protected List<Card> scrappedCards;

    public EnhancedConverted(GeneticAI clone, bool clearValue, int generation, int index, bool randomize = false, double exponent = 1) : base(clone, clearValue, generation, index, randomize, exponent)
    {

    }

    public EnhancedConverted(bool randomize, int generation, int index, double exponent = 1) : base(randomize, generation, index, exponent)
    {

    }

    public override void NewValues()
    {
        Settings = new Dictionary<string, object>
        {
            { "functionC", 13.0 },
            { "functionA", 2.0 / 9.0 },
            { "functionM", 4 },
            { "turnOverRound", 12 },
            { "turnOverMinimum", 22 },
            { "firstKnockMinimum", 15 },
            { "firstKnock", true }
        };
    }

    // Returnerar true om spelaren skall knacka, annars false
    public override bool Knacka(int round)
    {
        if (round == 0)
        {
            if ((bool)Settings["firstTurnKnock"] && Hand.HandValue() >= (int)Settings["firstKnockMinimum"])
            {
                return true;
            }

            return false;
        }

        if (round == 1)
            return false;

        int currentMinimumKnock = 0;

        if (round < 12)
        {
            currentMinimumKnock = (int)((double)Settings["functionC"] * Math.Pow(round, (double)Settings["functionA"])) + (int)Settings["functionM"];
        }

        if (round >= 12)
        {
            currentMinimumKnock = (int)Settings["turnOverMinimum"];
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

}
