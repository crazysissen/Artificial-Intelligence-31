using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrettioEtt;

static class EMethods
{
    const double
        MINIMUMVALUE = 0.8,
        MAXIMUMVALUE = 1.25;

    static Random random;

    public static object ChangeRandomly(this object target, double randomnessExponent)
    {
        object targetCopy = target;

        if (random == null)
            random = new Random();

        double preliminaryDouble = random.NextDouble();

        if (targetCopy is bool)
        {
            targetCopy = (bool)(preliminaryDouble > 0.65 ? !(bool)targetCopy : targetCopy);
            return targetCopy;
        }

        double randomDouble = Math.Pow(preliminaryDouble * (MAXIMUMVALUE - MINIMUMVALUE) + MINIMUMVALUE, randomnessExponent);

        if (targetCopy is int)
        {
            if (preliminaryDouble < 0.35)
            {
                targetCopy = (int)(randomDouble * ((int)targetCopy));
                return targetCopy;
            }

            if (preliminaryDouble > 0.65)
            {
                targetCopy = (int)(randomDouble * ((int)targetCopy)) + 1;
                return targetCopy;
            }

            return targetCopy;
        }

        if (targetCopy is double)
        {
            targetCopy = (double)targetCopy * randomDouble;
            return targetCopy;
        }

        return targetCopy;
    }

    public static int BestHandIndex(List<Card>[] hands)
    {
        int highestValue = hands[0].HandValue();
        int highestIndex = 0;

        for (int i = 1; i < 3; ++i)
        {
            int thisValue = hands[i].HandValue();

            if (thisValue > highestValue)
            {
                highestValue = thisValue;
                highestIndex = i;
            }
        }

        return highestIndex;
    }

    public static List<Card> BestCombination(List<Card> inputHand, out Card discardCard)
    {
        List<Card> allCards = new List<Card>(inputHand);

        List<Card> bestHand = new List<Card>(allCards);
        bestHand.RemoveAt(0);

        discardCard = allCards[0];

        int bestValue = bestHand.HandValue();
        int bestTotalValue = bestHand.TotalHandValue();

        for (int i = 1; i < allCards.Count; ++i)
        {
            List<Card> thisHand = new List<Card>(allCards);
            Card potentialDiscardCard = thisHand[i];
            thisHand.RemoveAt(i);

            int currentValue = thisHand.HandValue();
            int currentTotalValue = thisHand.TotalHandValue();

            if (currentValue < bestValue || (currentValue == bestValue && currentTotalValue <= bestTotalValue))
            {
                continue;
            }

            bestValue = currentValue;
            bestTotalValue = currentTotalValue;
            bestHand = thisHand;
            discardCard = potentialDiscardCard;
        }

        return bestHand;
    }

    public static int HandValue(this IEnumerable<Card> cards)
    {
        int[] suitValues = new int[4];

        foreach (Card card in cards)
        {
            suitValues[(int)card.Suit] += card.Value;
        }

        int highest = suitValues[0];

        for (int i = 1; i < 4; ++i)
        {
            if (suitValues[i] > highest)
            {
                highest = suitValues[i];
            }
        }

        return highest;
    }

    public static int TotalHandValue(this IEnumerable<Card> cards)
    {
        int totalValue = 0;

        foreach (Card card in cards)
        {
            totalValue += card.Value;
        }

        return totalValue;
    }

    public static List<Card>[] PossibleHands(List<Card> hand, Card additionalCard)
    {
        List<Card>[] returnValue = new List<Card>[3];

        for (int i = 0; i < 3; ++i)
        {
            returnValue[i].AddRange(hand);
            returnValue[i].Add(additionalCard);
        }

        return returnValue;
    }

    public static Card DiscardCard(List<Card> hand)
    {
        List<Card> tempHand = new List<Card>(hand);
        Card additionalCard = tempHand[tempHand.Count - 1];
        tempHand.RemoveAt(tempHand.Count - 1);

        return DiscardCardAdditional(tempHand, additionalCard);
    }

    public static Card DiscardCardAdditional(List<Card> hand, Card additionalCard)
    {
        List<Card> allCards = new List<Card>(hand) { additionalCard };

        List<Card> optimalHand = BestCombination(allCards, out Card discardCard);

        return discardCard;
    }

    public static GeneticAI Combine(this GeneticAI aI, GeneticAI counterpart, int generation, int index, double mutationChance)
    {
        GeneticAI newAi = new GeneticAI(aI, false, generation, index);

        Dictionary<string, object> settings = new Dictionary<string, object>(newAi.Settings);

        if (random == null)
            random = new Random();

        double randomDouble = random.NextDouble();

        foreach (KeyValuePair<string, object> setting in aI.Settings)
        {
            if (randomDouble < mutationChance)
            {
                settings[setting.Key] = settings[setting.Key].ChangeRandomly(1);
                continue;
            }

            if ((randomDouble - mutationChance) / mutationChance < 0.5)
            {
                settings[setting.Key] = counterpart.Settings[setting.Key];
            }
        }

        return newAi;
    }
}