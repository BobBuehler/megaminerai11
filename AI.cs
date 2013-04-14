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
                                                        SpeciesIndex.SEA_URCHIN,
                                                        SpeciesIndex.OCTOPUS,
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

    List<List<Mission>> missions = new List<List<Mission>>();

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
        Executor.Execute(this, fishes.Where(f => f.Owner == playerID()).Select(f => new Mission[]
            {
                new Mission(f, Objective.getTrash, () => Bb.OurTrashMap),
                new Mission(f, Objective.dumpTrash, () => Bb.TheirReef)
            }.ToList()
        ).ToList());
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
    public Tile getTile(int x, int y)
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

    public void goNearAndDoSomething(Fish fish, BitArray want, Action<Tile> something)
    {
        Point fishPoint = new Point(fish.X, fish.Y);

        BitArray passable = Bb.GetPassable().Or(want);
        passable.Set(Bb.GetOffset(fishPoint.X, fishPoint.Y), true);

        var path = Pather.aStar(fishPoint, want, passable).ToArray();
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
                something(getTile(goal.X, goal.Y));
            }
        }
    }

    public void assignmissions()
    {
        assignStarfish();
        assignSponges();
        assignAngelfishes();
        assignSnails();
        assignUrchins();
        assignOctopi();
        assignTomcods();
        assignSharks();
        assignCuttleFishes();
        assignShrimp();
        assignEels();
        assignJellyFishes();
    }



    public void assignStarfish()
    {
        foreach (Fish f in Bb.OurStarfishSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirReef));
            mission.Add(new Mission(f, Objective.surviveWithInTarget, () => Bb.TheirReef, false));
            missions.Add(mission);
        }
    }

    public void assignSponges()
    {
        foreach (Fish f in Bb.OurSpongesMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurTrashMap));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            missions.Add(mission);
        }
    }

    public void assignAngelfishes()
    {
        foreach (Fish f in Bb.OurAngelfishesMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurTrashMap));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));//todo: change their reef to their coves/dump far
            missions.Add(mission);
        }
    }

    public void assignSnails()
    {
        foreach (Fish f in Bb.OurSnailsMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.attackTarget, () => (Bb.TheirFishMap.Xor(Bb.TheirUrchinsMap)),true));//todo:snails protect our borders?
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirCoveMap));//todo:broken
            mission.Add(new Mission(f, Objective.attackTarget, () => (Bb.TheirFishMap.Xor(Bb.TheirUrchinsMap))));
            missions.Add(mission);
        }
    }
    public void assignUrchins()
    {
        foreach (Fish f in Bb.OurUrchinsMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.attackTarget, () => (Bb.TheirFishMap.Xor(Bb.TheirUrchinsMap)),true));
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirCoveMap));//todo:broken
            mission.Add(new Mission(f, Objective.attackTarget, () => (Bb.TheirFishMap.Xor(Bb.TheirUrchinsMap))));
            missions.Add(mission);
        }
    }
    public void assignOctopi()
    {
        foreach (Fish f in Bb.OurOctopiMap)
        {
            List<Mission> mission = new List<Mission>();//todo: camp center?
            mission.Add(new Mission(f, Objective.attackTarget, () => (Bb.TheirFishMap.Xor(Bb.TheirUrchinsMap)),true));//todo: implement multiple attacks
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirCoveMap));//todo:broken
            mission.Add(new Mission(f, Objective.attackTarget, () => (Bb.TheirFishMap.Xor(Bb.TheirUrchinsMap))));
            missions.Add(mission);
        }
    }
    public void assignTomcods()
    {
        foreach (Fish f in Bb.OurTomcodsMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurTrashMap));//todo:dump at coves/far
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            missions.Add(mission);
        }
    }
    public void assignSharks()
    {
        foreach (Fish f in Bb.OurSharksMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.attackTarget, () => (Bb.TheirFishMap.Xor(Bb.TheirUrchinsMap))));
            missions.Add(mission);
        }
    }
    public void assignCuttleFishes()
    {
        foreach (Fish f in Bb.OurFishMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurTrashMap));

            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            missions.Add(mission);
        }
    }
    public void assignShrimp()
    {
        foreach (Fish f in Bb.OurShrimpsMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurTrashMap));

            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            missions.Add(mission);
        }
    }
    public void assignEels()
    {
        foreach (Fish f in Bb.OurEelsMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurTrashMap));

            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            missions.Add(mission);
        }
    }
    public void assignJellyFishes()
    {
        foreach (Fish f in Bb.OurJellyfishMap)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurTrashMap));

            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            missions.Add(mission);
        }
    }

    //##################################################################
}