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
        GAMECOUNT = 500;

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
            players.Add(GetPlayer());
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
            bestAI = GetClone(players[0]);
        }
        else if (players[0].Wins > bestAI.Wins)
        {
            bestAI = GetClone(players[0]);
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

            players.Add(GetPlayer());
        }
    }

    public Player GetOpponent()
    {
        return new EnhancedAI();
    }

    public GeneticAI GetPlayer()
    {
        return new EnhancedConverted(true, currentGeneration, currentIndex++);
    }

    public GeneticAI GetClone(GeneticAI template)
    {
        return new EnhancedConverted(template, false, currentGeneration, currentIndex++);
    }
}

abstract class GeneticAI : Player
{
    public int Wins { get; set; }
    public int Generation { get; protected set; }
    public int Index { get; protected set; }

    // Genetic modifiers
    public abstract Dictionary<string, object> Settings { get; set; }

    protected List<string> settingTags;

    public void RandomizeValues(double exponent)
    {
        for (int i = 0; i < settingTags.Count; i++)
        {
            Settings[settingTags[i]] = Settings[settingTags[i]].ChangeRandomly(exponent);
        }
    }

    public abstract void NewValues();

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

    // Anropas när ett spel tar slut. Wongames++ får ej ändras!
    public override void SpelSlut(bool wonTheGame)
    {
        if (wonTheGame)
        {
            Wongames++;
        }
    }
}