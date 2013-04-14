using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    static class Executor
    {
        public static void Execute(AI ai, List<List<Mission>> missions)
        {
            missions.ForEach(ms => ms.ForEach(m => Execute(ai, m)));
        }

        public static void Execute(AI ai, Mission mission)
        {
            switch(mission.m_obj)
            {
                case Objective.goTo:
                    GoTo(ai, mission);
                    break;
                case Objective.getTrash:
                    GetTrash(ai, mission);
                    break;
                case Objective.dumpTrash:
                    DumpTrash(ai, mission);
                    break;
                case Objective.attackTarget:
                    AttackTarget(ai, mission);
                    break;
            }
            return;
        }

        public static void GoTo(AI ai, Mission mission)
        {
            Fish fish = mission.m_agent;
            if (fish.MovementLeft == 0)
            {
                return;
            }

            Point fishPoint = fish.Point();
            
            Bb.Update(ai);

            BitArray passable = Bb.GetPassable(fishPoint);
            BitArray targetsInPassable = new BitArray(mission.m_targets()).And(passable);

            var path = Pather.aStar(fishPoint, targetsInPassable, passable);
            MoveAlong(fish, path);
        }

        public static void GetTrash(AI ai, Mission mission)
        {
            Fish fish = mission.m_agent;
            int availableCapacity = fish.CarryCap - fish.CarryingWeight;
            if (availableCapacity == 0)
            {
                return;
            }

            Point fishPoint = fish.Point();

            Bb.Update(ai);

            BitArray targetsInTrash = new BitArray(mission.m_targets()).And(Bb.TrashMap);
            BitArray passableOrTargetsInTrash = Bb.GetPassable(fishPoint).Or(targetsInTrash);

            while (availableCapacity > 0)
            {
                var path = Pather.aStar(fishPoint, targetsInTrash, passableOrTargetsInTrash).ToArray();
                var trash = path[path.Length - 1];
                var tile = ai.getTile(trash.X, trash.Y);
                var amount = Math.Min(Math.Min(tile.TrashAmount, availableCapacity), fish.CurrentHealth - 1);
                if (amount == 0)
                {
                    return; // Can't pick it up
                }
                if (path.Length > 1 && MoveAlong(fish, path.Range(0, path.Length - 1)))
                {
                    Console.WriteLine("{0} picking up {1} at {2}", fish.Text(), amount, trash);
                    fish.pickUp(tile, amount);

                    fishPoint = path[path.Length - 2];
                    availableCapacity -= amount;
                    Bb.Set(targetsInTrash, trash, false);
                }
                else
                {
                    return; // no trash found, or didn't move far enough.
                }
            }
        }

        public static void DumpTrash(AI ai, Mission mission)
        {
            Fish fish = mission.m_agent;
            int weight = fish.CarryingWeight;
            if (weight == 0)
            {
                return;
            }

            Point fishPoint = fish.Point();

            Bb.Update(ai);

            BitArray passableOrTheirTrash = Bb.GetPassable(fishPoint).Or(Bb.TheirTrashMap);
            BitArray targetsInPassableOrTheirTrash = new BitArray(mission.m_targets()).And(passableOrTheirTrash);
            Bb.Set(targetsInPassableOrTheirTrash, fishPoint, false);

            var path = Pather.aStar(fishPoint, targetsInPassableOrTheirTrash, passableOrTheirTrash).ToArray();
            if (path.Length > 1 && MoveAlong(fish, path.Range(0, path.Length - 1)))
            {
                var dump = path[path.Length - 1];
                var tile = ai.getTile(dump.X, dump.Y);
                Console.WriteLine("{0} picking up {1} at {2}", fish.Text(), weight, dump);
                fish.drop(tile, weight);
            }
        }

        public static void AttackTarget(AI ai, Mission mission)
        {
            Fish fish = mission.m_agent;
            int attacks = fish.AttacksLeft;
            if (attacks == 0)
            {
                return;
            }

            Point fishPoint = fish.Point();
            int range = fish.Range;

            Bb.Update(ai);

            while (attacks > 0)
            {
                BitArray passableOrTheirFish = Bb.GetPassable(fishPoint).Or(Bb.TheirFishMap);
                BitArray targetsInTheirFish = new BitArray(mission.m_targets()).And(Bb.TheirFishMap);

                var path = Pather.aStar(fishPoint, targetsInTheirFish, passableOrTheirFish).ToArray();
                if (path.Length > 1 && MoveAlong(fish, path.Range(0, path.Length - range)))
                {
                    var aim = path[path.Length - 1];
                    var enemy = ai.getFish(aim.X, aim.Y);
                    Console.WriteLine("{0} attack {1}", fish.Text(), enemy.Text());
                    fish.attack(enemy);
                }
            }
        }


        private static bool MoveAlong(Fish fish, IEnumerable<Point> path)
        {
            Point[] fullPath = path.ToArray();
            if (fullPath.Length == 0)
            {
                return true;
            }
            for (int i = 1; i < fullPath.Length && fish.MovementLeft > 0; ++i)
            {
                Point moveTo = fullPath[i];
                fish.move(moveTo.X, moveTo.Y);
            }
            Console.WriteLine("{0} moved to {1}", fish.Text(), fish.Point());

            var goal = fullPath.Last();
            return fish.X == goal.X && fish.Y == goal.Y;
        }

        private static IEnumerable<T> Range<T>(this IEnumerable<T> source, int start, int end)
        {
            return source.Where((s, i) => i >= start && i < end);
        }

        /// <summary>
        /// Specifies the species available.
        /// </summary>
        private enum SpeciesIndex
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

        public static String Text(this Fish fish)
        {
            return String.Format("{0}:{1}", (SpeciesIndex)fish.Species, fish.Id);
        }

        public static Point Point(this Fish fish)
        {
            return new Point(fish.X, fish.Y);
        }
    }
}
