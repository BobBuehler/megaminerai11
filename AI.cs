using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Pizza;

/// <summary>
/// Extends BaseAI with AI gameplay logic.
/// </summary>
class AI : BaseAI
{
    /// <summary>
    /// Specifies the species available.
    /// </summary>
    public enum SpeciesIndex
    {
        SEA_STAR,
        SPONGE,
        ANGELFISH,
        CONESHELL_SNAIL,
        SEA_URCHIN,
        OCTOPUS,
        TOMCOD,
        REEF_SHARK,
        CUTTLEFISH,
        CLEANER_SHRIMP,
        ELECTRIC_EEL,
        JELLYFISH
    };

    public static SpeciesIndex[] preferedSpeciesList = { SpeciesIndex.TOMCOD,
                                                        SpeciesIndex.SEA_STAR,
                                                        SpeciesIndex.ANGELFISH,
                                                        SpeciesIndex.CUTTLEFISH,
                                                        SpeciesIndex.JELLYFISH,
                                                        SpeciesIndex.SPONGE,
                                                        SpeciesIndex.REEF_SHARK,
                                                        SpeciesIndex.CONESHELL_SNAIL,
                                                        SpeciesIndex.SEA_URCHIN,
                                                        SpeciesIndex.OCTOPUS,
                                                        SpeciesIndex.ELECTRIC_EEL,
                                                        SpeciesIndex.CLEANER_SHRIMP };

    //HashSet<Fish> starfish = new HashSet<Fish>();
    //HashSet<Fish> sponges = new HashSet<Fish>();
    //HashSet<Fish> angelfishes = new HashSet<Fish>();
    //HashSet<Fish> snails = new HashSet<Fish>();
    //HashSet<Fish> urchins = new HashSet<Fish>();
    //HashSet<Fish> octopi = new HashSet<Fish>();
    //HashSet<Fish> tomcods = new HashSet<Fish>();
    //HashSet<Fish> sharks = new HashSet<Fish>();
    //HashSet<Fish> cuttlefishes = new HashSet<Fish>();
    //HashSet<Fish> shrimps = new HashSet<Fish>();
    //HashSet<Fish> eels = new HashSet<Fish>();
    //HashSet<Fish> jellyfish = new HashSet<Fish>();

    List<Mission> missions = new List<Mission>();

    /// <summary>
    /// Returns your username.
    /// </summary>
    /// <remarks></remarks>
    /// <returns>Your username.</returns>
    public override string username()
    {
        return "Pizza Seeker";
    }

    /// <summary>
    /// Returns your password.
    /// </summary>
    /// <returns>Your password.</returns>
    public override string password()
    {
        return "password";
    }

    /// <summary>
    /// This function is called each time it's your turn.
    /// </summary>
    /// <returns>Return true to end your turn, return false to ask the server for updated information.</returns>
    public override bool run()
    {
        Bb.Update(this);

        spawn();

        Console.WriteLine("Trash:");
        Console.WriteLine(Bb.ToString(Bb.TrashMap));
        // Iterate through all the fishes.
        foreach (Fish fish in fishes)
        {
            // Only attempt to move fish we own.
            if (fish.Owner == playerID())
            {
                Console.WriteLine("Fish:" + fish.Id);
                if (fish.CarryingWeight == 0)
                {
                    goNearAndDoSomething(fish, Bb.TrashMap, g =>
                    {
                        var tile = getTile(g.X, g.Y);
                        fish.pickUp(tile, Math.Min(fish.CarryCap - fish.CarryingWeight, tile.TrashAmount));
                    });
                }
                if (fish.CarryingWeight > 0 && fish.MovementLeft > 0)
                {
                    goNearAndDoSomething(fish, Bb.TrashMap, g =>
                    {
                        var tile = getTile(g.X, g.Y);
                        fish.drop(tile, fish.CarryingWeight);
                    });
                }
            }
        }

        return true;
    }

    /// <summary>
    /// This function is called once, before your first turn.
    /// </summary>
    public override void init()
    {
        Bb.init(this);
    }

    /// <summary>
    /// This function is called once, after your last turn.
    /// </summary>
    public override void end() { }


    /// <summary>
    /// Initializes a new instance of the AI class connected to the server.
    /// </summary>
    /// <param name="c">The managed pointer to the open connection.</param>
    public AI(IntPtr c)
        : base(c)
    {
    }

    #region Helper Methods
    /// <summary>
    /// Returns the Tile at the specified coordinates.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The Tile with the specififed coordinates.</returns>
    /// <exception cref="System.ArgumentException">The specified x and y coordinates must be on the map.</exception>
    Tile getTile(int x, int y)
    {
        if (x < 0 || y < 0 || x > mapWidth() || y > mapHeight())
            throw new ArgumentException(String.Format("The specified x and y coordinates ({0}, {1}) must be on the map.", x, y));

        return tiles[(mapHeight() * x) + y];
    }

    /// <summary>
    /// Returns the Fish at the specified coordinates, or null if there is none.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The Fish at the specified coordinates if there is one; otherwise, null.</returns>
    Fish getFish(int x, int y)
    {
        if (fishes.Count(f => f.X == x && f.Y == y) != 0)
            return fishes.First(f => f.X == x && f.Y == y);

        return null;
    }
    #endregion

    //############################ our code ######################################
    public void spawn()
    {
        // Iterate across all tiles.
        foreach (Tile tile in Bb.OurCoveSet)
        {
            // If the tile is yours, is not spawning a fish, and has no fish on it...
            if (tile.HasEgg == 0 && getFish(tile.X, tile.Y) == null)
            {
                // ...iterate across all species.
                for (int i = 0; i < preferedSpeciesList.Length; i++)
                {
                    // If the species is in season and we can afford it...
                    Species s = speciesList[(int)preferedSpeciesList[i]];
                    if (s.Season == currentSeason() && players[playerID()].SpawnFood >= s.Cost)
                    {
                        // ...spawn it and break (can't spawn multiple fish on the same cove).
                        s.spawn(tile);
                        break;
                    }
                }
            }
        }
    }

    public void goNearAndDoSomething(Fish fish, BitArray want, Action<Point> something)
    {
        Point p = new Point(fish.X, fish.Y);

        Bb.Update(this);
        BitArray passable = new BitArray(Bb.WallMap).Or(Bb.CoveMap).Or(Bb.FishMap).Not().Or(want);
        passable.Set(Bb.GetOffset(p.X, p.Y), true);

        var path = Pather.aStar(p, Bb.OurTrashMap, passable).ToArray();
        if (path.Length > 1)
        {
            var goal = path[path.Length - 1];
            bool madeIt = true;
            for (int i = 1; i < path.Length - 1; ++i)
            {
                if (fish.MovementLeft > 0)
                {
                    Console.WriteLine("({0},{1}) moving to {2}", fish.X, fish.Y, path[i]);
                    fish.move(path[i].X, path[i].Y);
                }
                else
                {
                    madeIt = false;
                    break;
                }
            }
            if (madeIt)
            {
                Console.WriteLine("({0},{1}) did reach {2}", fish.X, fish.Y, goal);
                var goalTile = getTile(goal.X, goal.Y);
                something(goal);
            }
        }
    }

    public void assignMissions()
    {
        assignStarfish();
    }

    public void assignStarfish()
    {
        foreach (Fish f in Bb.OurStarfishSet)
        {
            missions.Add(new Mission(f, Objective.goTo, Bb.TheirReef));
            if (f.MovementLeft > 0)
            {
                missions.Add(new Mission(f, Objective.dodgeInReef, Bb.TheirReef,false));
            }
        }
    }

    //##################################################################
}