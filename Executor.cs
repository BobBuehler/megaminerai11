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
                case Objective.getNearestTrash:
                    GetNearestTrash(ai, mission);
                    break;
            }
            return;
        }

        public static bool GoTo(AI ai, Mission mission)
        {
            Fish fish = mission.m_agent;
            Point fishPoint = new Point(fish.X, fish.Y);
            BitArray targets = mission.m_targets();
            if (fish.MovementLeft == 0)
            {
                return Bb.Get(targets, fishPoint.X, fishPoint.Y);
            }


            Bb.Update(ai);

            BitArray passable = Bb.GetPassable(fishPoint);

            // Only the targets that can be achieved
            targets = new BitArray(targets).And(passable);

            var path = Pather.aStar(fishPoint, targets, passable);
            return MoveAlong(fish, path);
        }

        public static void GetNearestTrash(AI ai, Mission mission)
        {
            Fish fish = mission.m_agent;
            int availableCapacity = fish.CarryCap - fish.CarryingWeight;
            if (availableCapacity == 0)
            {
                return;
            }

            Point fishPoint = new Point(fish.X, fish.Y);

            Bb.Update(ai);

            BitArray targets = new BitArray(mission.m_targets());
            BitArray passableOrTrash = Bb.GetPassable().Or(targets);
            passableOrTrash.Set(Bb.GetOffset(fishPoint.X, fishPoint.Y), true);

            while (availableCapacity > 0)
            {
                var path = Pather.aStar(fishPoint, targets, passableOrTrash).ToArray();
                if (path.Length > 1 && MoveAlong(fish, path.Range(0, path.Length - 1)))
                {
                    var trash = path[path.Length - 1];
                    var tile = ai.getTile(trash.X, trash.Y);
                    var amount = Math.Min(tile.TrashAmount, availableCapacity);
                    Console.WriteLine("{0} picking up {1} at {2}", fish.Text(), amount, trash);
                    fish.pickUp(tile, amount);

                    availableCapacity -= amount;
                    Bb.Set(targets, trash, false);
                }
                else
                {
                    return; // no trash found, or didn't move far enough.
                }
            }
        }

        private static bool MoveAlong(Fish fish, IEnumerable<Point> path)
        {
            Point[] fullPath = path.ToArray();
            for (int i = 1; i < fullPath.Length && fish.MovementLeft > 0; ++i)
            {
                Point moveTo = fullPath[i];
                Console.WriteLine("({0},{1}) moving to {2}", fish.X, fish.Y, moveTo);
                fish.move(moveTo.X, moveTo.Y);
            }

            var goal = fullPath.Last();
            return fish.X == goal.X && fish.Y == goal.Y;
        }

        public static IEnumerable<T> Range<T>(this IEnumerable<T> source, int start, int end)
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
    }
}
