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
                                                        SpeciesIndex.ANGELFISH,                                                        
                                                        SpeciesIndex.CUTTLEFISH,
                                                        SpeciesIndex.JELLYFISH,
                                                        SpeciesIndex.REEF_SHARK,
                                                        SpeciesIndex.CONESHELL_SNAIL,
                                                        SpeciesIndex.OCTOPUS,
                                                        SpeciesIndex.SEA_URCHIN,                                                        
                                                        SpeciesIndex.SPONGE,
                                                        SpeciesIndex.SEA_STAR};

    List<List<Mission>> missions = new List<List<Mission>>();
    Func<BitArray> ourTrash = () => Bb.OurReef;
    Func<BitArray> notUrchin = () => new BitArray(Bb.TheirUrchinsMap).Not();
    List<int> emergencyCarriers = new List<int>();
    int maxExtraNonCarries = 3;

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
        Console.WriteLine("Turn:{0}", iteration);
        if (turnNumber() >= 30)
        {
            ourTrash = () => new BitArray(Bb.OurReef).Or(Bb.NeutralReef);
        }
        missions.Clear();
        Bb.Update(this);
        manageSpecials();
        spawn();
        assignmissions();
        Executor.Execute(this, missions);
        /*
        Executor.Execute(this, fishes.Where(f => f.Owner == playerID()).Select(f => new Mission[]
            {
                new Mission(f, Objective.getTrash, () => new BitArray(Bb.OurReef).Or(Bb.NeutralReef)),
                new Mission(f, Objective.dumpTrash, () => Bb.TheirReef)
            }.ToList()
        ).ToList());*/
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
    public Fish getFish(int x, int y)
    {
        if (fishes.Count(f => f.X == x && f.Y == y) != 0)
            return fishes.First(f => f.X == x && f.Y == y);

        return null;
    }
    #endregion

    //############################ our code ######################################
    public void manageSpecials()
    {
        try
        {
            removeDeadFish();
            if (specialsRequired())
            {
                maxExtraNonCarries = 5;
                foreach (Tile tile in Bb.OurCoveSet)
                {
                    if (emergencyCarriers.Count < 3 && getFish(tile.X, tile.Y) != null)
                    {
                        emergencyCarriers.Add(getFish(tile.X, tile.Y).Id);
                    }
                }
                if (emergencyCarriers.Count < 3)
                {
                    foreach (Fish f in fishes)
                    {
                        if (f.Owner == playerID() && emergencyCarriers.Count < 3)
                        {
                            emergencyCarriers.Add(f.Id);
                        }
                    }
                }
            }
            else
            {
                maxExtraNonCarries = 3;
            }
            foreach (Fish f in fishes)
            {
                if (emergencyCarriers.Contains(f.Id))
                {
                    List<Mission> mission = new List<Mission>();
                    mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurCoveMap));
                    mission.Add(new Mission(f, Objective.getTrash, ourTrash));
                    mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
                    mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.NeutralReef));
                    mission.Add(new Mission(f, Objective.getTrash, ourTrash));
                    missions.Add(mission);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public void removeDeadFish()//todo: make this efficient
    {
        try
        {
            List<int> newList = new List<int>();
            foreach (Fish f in fishes)
            {
                if (emergencyCarriers.Contains(f.Id))
                {
                    newList.Add(f.Id);
                }
            }
            emergencyCarriers.Clear();
            emergencyCarriers = newList;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public bool specialsRequired()
    {
        if (carryCount() == 0 && turnNumber() % seasonLength() < 43)
        {
            return true;
        }
        return false;
    }

    public void spawn()
    {
        int spawnCount = 0;
        if (!saveMoney())
        {
            // Iterate across all tiles.
            foreach (Tile tile in Bb.OurCoveSet) //todo: order coves in order of closeness
            {
                // If the tile is yours, is not spawning a fish, and has no fish on it...
                if (tile.HasEgg == 0 && getFish(tile.X, tile.Y) == null)
                {
                    List<Species> ps = calcPreferedSpecies();
                    foreach (Species s in ps)
                    {
                        if (s.Season == currentSeason())
                        {
                            if (players[playerID()].SpawnFood >= s.Cost)
                            {
                                // ...spawn it and break (can't spawn multiple fish on the same cove).
                                if (shouldSpawn(s, spawnCount))
                                {
                                    s.spawn(tile);
                                    spawnCount++;
                                    break;
                                }
                            }
                            else
                            {
                                // if tomcod or angelfish are in season and we have less than 4 tomcod or angelfish save the money!
                                if (s.SpeciesNum == (int)SpeciesIndex.TOMCOD || s.SpeciesNum == (int)SpeciesIndex.ANGELFISH)
                                {
                                    if (Bb.OurTomcodsSet.Count + Bb.OurAngelfishesSet.Count < 4)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public bool shouldSpawn(Species s, int spawnCount)
    {
        if (s.SpeciesNum == (int)SpeciesIndex.TOMCOD)
        {
            if (Bb.OurTomcodsSet.Count >= 3)
            {
                return false;
            }
            return true;
        }
        else if (s.SpeciesNum == (int)SpeciesIndex.ANGELFISH)
        {
            return true;
        }
        else if (spawnCount > 3)
        {
            return false;
        }
        else if (s.SpeciesNum == (int)SpeciesIndex.SEA_STAR && Bb.OurStarfishSet.Count > 2)
        {
            return false;
        }
        else if (s.SpeciesNum == (int)SpeciesIndex.SPONGE && Bb.OurSpongesSet.Count > 2)
        {
            return false;
        }
        else if (s.SpeciesNum == (int)SpeciesIndex.SEA_URCHIN && Bb.OurUrchinsSet.Count >= 2)
        {
            return false;
        }
        else if (s.SpeciesNum == (int)SpeciesIndex.CONESHELL_SNAIL && (Bb.OurSnailsSet.Count + 1 > Bb.OurSharksSet.Count && speciesList[(int)SpeciesIndex.REEF_SHARK].Season == currentSeason()))
        {
            return false;
        }
        else if (nonCarryCount() >= carryCount() + maxExtraNonCarries || nonCarryCount() > 5)
        {
            return false;
        }


        return true;
    }

    public bool isNonCarry(Species s)
    {
        if (s.SpeciesNum == (int)SpeciesIndex.CONESHELL_SNAIL ||
            s.SpeciesNum == (int)SpeciesIndex.SEA_URCHIN ||
            s.SpeciesNum == (int)SpeciesIndex.OCTOPUS ||
            s.SpeciesNum == (int)SpeciesIndex.REEF_SHARK ||
            s.SpeciesNum == (int)SpeciesIndex.CUTTLEFISH ||
            s.SpeciesNum == (int)SpeciesIndex.CLEANER_SHRIMP ||
            s.SpeciesNum == (int)SpeciesIndex.ELECTRIC_EEL ||
            s.SpeciesNum == (int)SpeciesIndex.JELLYFISH)
        {
            return true;
        }
        return false;
    }

    public int carryCount()
    {
        return Bb.OurAngelfishesSet.Count + Bb.OurTomcodsSet.Count + Bb.OurStarfishSet.Count;
    }

    public int nonCarryCount()
    {
        return Bb.OurSnailsSet.Count + Bb.OurUrchinsSet.Count + Bb.OurOctopiSet.Count +
            Bb.OurSharksSet.Count + Bb.OurCuttlefishesSet.Count + Bb.OurShrimpsSet.Count +
            Bb.OurEelsSet.Count + Bb.OurJellyfishSet.Count;
    }

    public List<Species> calcPreferedSpecies()
    {
        List<Species> preflist = new List<Species>();

        for (int i = 0; i < preferedSpeciesList.Length; i++)
        {
            Species s = speciesList[(int)preferedSpeciesList[i]];
            if (s.Season == currentSeason())
            {
                preflist.Add(s);
            }
        }
        return preflist;
    }

    public bool saveMoney()
    {
        if (turnNumber() < 455 && turnNumber() % seasonLength() > 39 && getNextSelectionPriority() > getThisSeasonPriority())
        {
            return true;
        }
        return false;
    }
    public int getThisSeasonPriority()
    {
        try
        {
            List<Species> ps = calcPreferedSpecies();
            for (int i = 0; i < ps.Count; i++)
            {
                if (ps[i].Season == currentSeason())
                {
                    return i;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return 12;
    }

    public int getNextSelectionPriority()
    {
        try
        {
            int nextSeason = currentSeason() + 1;
            if (nextSeason == 4)
            {
                nextSeason = 0;
            }
            List<Species> ps = calcPreferedSpecies();
            for (int i = 0; i < ps.Count; i++)
            {
                if (ps[i].Season == nextSeason)
                {
                    return i;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return 13;
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
        assignJellyFishes();
        assignOctopi();
        assignCuttleFishes();
        assignSharks();        
        assignSnails();
        assignUrchins();
        assignTomcods();
        assignAngelfishes();
        assignStarfish();
        assignSponges();
        assignEels();
        assignShrimp();
    }



    public void assignStarfish()
    {
        foreach (Fish f in Bb.OurStarfishSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, ourTrash));//todo: if starfish is not on their reef
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.NeutralReef));
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirDeepestReef));//todo: if not implemented go to edge
            mission.Add(new Mission(f, Objective.surviveWithInTarget, () => Bb.TheirReef, false));

            missions.Add(mission);
        }
    }

    public void assignSponges()
    {
        foreach (Fish f in Bb.OurSpongesSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurCoveMap));
            mission.Add(new Mission(f, Objective.getTrash, ourTrash));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.NeutralReef));
            mission.Add(new Mission(f, Objective.getTrash, ourTrash));
            mission.Add(new Mission(f, Objective.dontsuicide, () => Bb.TheirReef));
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin));
            missions.Add(mission);
        }
    }

    public void assignAngelfishes()
    {
        foreach (Fish f in Bb.OurAngelfishesSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurCoveMap));
            mission.Add(new Mission(f, Objective.getTrash, ourTrash));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));//todo: change their reef to their coves??
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.NeutralReef));
            mission.Add(new Mission(f, Objective.getTrash, ourTrash));
            mission.Add(new Mission(f, Objective.dontsuicide, () => Bb.TheirReef));
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin));

            missions.Add(mission);
        }
    }

    public void assignSnails()
    {
        foreach (Fish f in Bb.OurSnailsSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin, true));//todo:snails protect our borders?
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirCoveMap));//todo:broken
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin));
            missions.Add(mission);
        }
    }
    public void assignUrchins()
    {
        foreach (Fish f in Bb.OurUrchinsSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin, true));
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirCoveMap));//todo:broken
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin));
            missions.Add(mission);
        }
    }
    public void assignOctopi()
    {
        foreach (Fish f in Bb.OurOctopiSet)
        {
            List<Mission> mission = new List<Mission>();//todo: camp center?
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin, true));//todo: implement multiple attacks
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirCoveMap));//todo:broken
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin));
            missions.Add(mission);
        }
    }
    public void assignTomcods()
    {
        foreach (Fish f in Bb.OurTomcodsSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurCoveMap));
            mission.Add(new Mission(f, Objective.getTrash, ourTrash));//todo:dump at coves/far
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirCoveMap));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirDeepestReef));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.NeutralReef));
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurCoveMap));
            mission.Add(new Mission(f, Objective.getTrash, ourTrash));
            missions.Add(mission);
        }
    }
    public void assignSharks()
    {
        foreach (Fish f in Bb.OurSharksSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.attackTarget, () => Bb.TheirFishMap));
            missions.Add(mission);
        }
    }
    public void assignCuttleFishes()
    {
        foreach (Fish f in Bb.OurCuttlefishesSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.attackTarget, () => Bb.TheirStarfishMap));//todo:attack stars first??
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin));
            missions.Add(mission);
        }
    }

    public void assignShrimp()
    {
        foreach (Fish f in Bb.OurShrimpsSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.getTrash, () => Bb.OurCoveMap));
            mission.Add(new Mission(f, Objective.getTrash, ourTrash));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.TheirReef));
            mission.Add(new Mission(f, Objective.dumpTrash, () => Bb.NeutralReef));
            mission.Add(new Mission(f, Objective.attackTarget, () => Bb.OurFishMap));
            missions.Add(mission);
        }
    }
    public void assignEels()
    {
        foreach (Fish f in Bb.OurEelsSet)
        {
            List<Mission> mission = new List<Mission>();
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin));
            missions.Add(mission);
        }
    }
    public void assignJellyFishes()
    {
        foreach (Fish f in Bb.OurJellyfishSet)
        {
            List<Mission> mission = new List<Mission>();//todo: camp center?
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin, true));
            mission.Add(new Mission(f, Objective.goTo, () => Bb.TheirCoveMap));//todo:broken
            mission.Add(new Mission(f, Objective.attackTarget, notUrchin));
            missions.Add(mission);
        }
    }
    //##################################################################
}