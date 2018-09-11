using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrettioEtt;

class GeneManager
{
    const int
        PLAYERCOUNT = 50, // Even, please
        GAMECOUNT = 5000;

    const bool
        CLICKTHROUGHGENERATIONS = false;

    GeneticAI bestAI;
    Game game;
    int currentGeneration, currentIndex;
    bool run;

    public List<GeneticAI> players = new List<GeneticAI>();

    public GeneManager(Game game)
    {
        this.game = game;
    }

    public void Initialize()
    {
        Console.CursorVisible = false;

        currentGeneration = 1;
        run = true;

        for (int i = 0; i < PLAYERCOUNT; ++i)
        {
            players.Add(new GeneticAI(true, currentGeneration, currentIndex++));
        }

        while (run)
        {
            PlayGeneration(currentGeneration == 1);

            if (CLICKTHROUGHGENERATIONS)
                Console.ReadKey(true);

            ++currentGeneration;
        }
    }

    public void PlayGeneration(bool firstGeneration)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("\nPlaying");

        for (int i = 0; i < players.Count; ++i)
        {
            if (i % (PLAYERCOUNT / 5) == 0)
            {
                Console.Write(".");
            }

            if (/*players[i].Wins == 0*/ true)
            {
                players[i].Wongames = 0;

                Player opponent = GetOpponent();

                players[i].Game = game;
                opponent.Game = game;

                game.Player1 = players[i];
                game.Player2 = opponent;

                for (int j = 0; j < GAMECOUNT; ++j)
                {
                    game.Initialize();
                    game.PlayAGame(j % 2 == 0);
                }

                players[i].Wins = players[i].Wongames;
            }
        }

        players = players.OrderBy(ai => ai.Wins).ToList();
        players.Reverse();
        
        if (firstGeneration)
        {
            bestAI = new GeneticAI(players[0], false, players[0].Generation, players[0].Index);
        }
        else if (players[0].Wins > bestAI.Wins)
        {
            bestAI = new GeneticAI(players[0], false, players[0].Generation, players[0].Index);
        }

        Console.Clear();
        WriteStats();

        HalfAndMutate(currentGeneration);
    }

    /// <summary>
    /// Make sure the list is sorted
    /// </summary>
    public void WriteStats()
    {
        float combinedChance = 0;
        foreach (GeneticAI aI in players)
        {
            combinedChance += (float)aI.Wins / GAMECOUNT;
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("GENERATION {0}\nAverage: {1}%\nTop players:\n", currentGeneration, ((int)(combinedChance / PLAYERCOUNT * 1000)) * 0.1f);

        for (int i = 0; i < 5; i++)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0}. AI [{1}], Generation [{2}]:\t", i + 1, players[i].Index, players[i].Generation);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} wins - {1} losses ({2}%)", players[i].Wins, GAMECOUNT - players[i].Wins, (int)(((double)players[i].Wins / GAMECOUNT) * 1000) * 0.1);
        }
    }

    public void HalfAndMutate(int generation)
    {
        int count = players.Count;

        players.RemoveRange(count / 2, count / 2);

        for (int i = 0; i < count / 2; i++)
        {
            GeneticAI newAi = players[i].Combine(players[i != count / 2 - 1 ? i + 1 : 0], generation, currentIndex++, 0.4);

            //if (/*i < count / 3*/ true)
            //{
            //    GeneticAI newAI = new GeneticAI(players[i], true, generation, currentIndex++);
            //    newAI.RandomizeValues(0.7 + 0.6 * (i / ((double)count / 2)));
            //    players.Add(newAI);
            //    continue;
            //}

            players.Add(new GeneticAI(true, generation, currentIndex++));
        }
    }

    public Player GetOpponent()
    {
        return new EnhancedAI();
    }
}

class GeneticAI : Player
{
    public int Wins { get; set; }
    public int Generation { get; private set; }
    public int Index { get; private set; }

    // Genetic modifiers
    public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>
    {
        { "functionC", 0.0 },
        { "functionA", 0.0 },
        { "functionM", 0 },
        { "turnOverRound", 0 },
        { "turnOverMinimum", 0 },
        { "firstKnockMinimum", 0 },
        { "firstKnock", false }
    };

    List<string> settingTags;

    List<Card> scrappedCards;
    List<Card> knownOpponentCards;

    public GeneticAI(GeneticAI clone, bool clearValue, int generation, int index, bool randomize = false, double exponent = 1)
    {
        Name = clone.Name;
        Generation = generation;
        Index = index;

        settingTags = new List<string>();
        foreach (KeyValuePair<string, object> item in Settings)
        {
            settingTags.Add(item.Key);
        }

        if (!clearValue)
        {
            Wins = clone.Wins;
        }

        Settings = clone.Settings;

        if (randomize)
        {
            RandomizeValues(exponent);
        }
    }

    public GeneticAI(bool randomize, int generation, int index, double exponent = 1)
    {
        Name = "Genetic AI";
        Generation = generation;
        Index = index;

        settingTags = new List<string>();
        foreach (KeyValuePair<string, object> item in Settings)
        {
            settingTags.Add(item.Key);
        }

        NewValues();

        if (randomize)
            RandomizeValues(exponent);
    }

    public void NewValues()
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

    public void RandomizeValues(double exponent)
    {
        for (int i = 0; i < settingTags.Count; i++)
        {
            Settings[settingTags[i]] = Settings[settingTags[i]].ChangeRandomly(exponent);
        }
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

    // Anropas när ett spel tar slut. Wongames++ får ej ändras!
    public override void SpelSlut(bool wonTheGame)
    {
        if (wonTheGame)
        {
            Wongames++;
        }
    }
}