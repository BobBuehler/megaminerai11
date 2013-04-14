using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    static class Executor
    {
        private static Dictionary<Fish, HashSet<Fish>> alreadyAttacked = new Dictionary<Fish, HashSet<Fish>>();
        private static HashSet<Fish> onADropMission = new HashSet<Fish>();
        public static void NewTurn()
        {
            alreadyAttacked.Clear();
            onADropMission.Clear();
        }

        public static void Execute(AI ai, List<List<Mission>> missions)
        {
            NewTurn();
            missions.ForEach(ms => ms.ForEach(m => Execute(ai, m)));
        }

        public static void Execute(AI ai, Mission mission)
        {
            if (mission.m_agent.CurrentHealth == 0)
            {
                return;
            }

            if (onADropMission.Contains(mission.m_agent) && !mission.m_overrideDropMission)
            {
                return;
            }

            switch (mission.m_obj)
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
                case Objective.coverWithTrash:
                    CoverWithTrash(ai, mission);
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
            MoveAlong(fish, path, mission.m_attackAlongTheWay);
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
                if (path.Length > 1 && MoveAlong(fish, path.Range(0, path.Length - 1), mission.m_attackAlongTheWay))
                {
                    fish.pickUp(tile, amount);

                    fishPoint = path[path.Length - 2];
                    availableCapacity -= amount;
                    if (tile.TrashAmount == 0)
                    {
                        Bb.Set(targetsInTrash, trash, false);
                    }
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
            if (path.Length > 1 && MoveAlong(fish, path.Range(0, path.Length - 1), mission.m_attackAlongTheWay))
            {
                var dump = path[path.Length - 1];
                var tile = ai.getTile(dump.X, dump.Y);
                fish.drop(tile, weight);
            }
            else if (path.Length > 1)
            {
                onADropMission.Add(fish);
            }
        }

        public static void CoverWithTrash(AI ai, Mission mission)
        {
            Fish fish = mission.m_agent;
            int weight = fish.CarryingWeight;
            if (weight == 0)
            {
                return;
            }

            Point fishPoint = fish.Point();

            Bb.Update(ai);

            BitArray targetsNotCovered = new BitArray(Bb.TrashMap).Not().And(mission.m_targets());
            Bb.Set(targetsNotCovered, fishPoint, false);
            BitArray passableOrNotCovered = Bb.GetPassable(fishPoint).Or(targetsNotCovered);

            var path = Pather.aStar(fishPoint, targetsNotCovered, passableOrNotCovered).ToArray();
            if (path.Length > 1 && MoveAlong(fish, path.Range(0, path.Length - 1), mission.m_attackAlongTheWay))
            {
                var dump = path[path.Length - 1];
                var tile = ai.getTile(dump.X, dump.Y);
                fish.drop(tile, weight);
            }
            else if (path.Length > 1)
            {
                onADropMission.Add(fish);
            }
        }

        public static void AttackTarget(AI ai, Mission mission)
        {
            Fish fish = mission.m_agent;
            if (fish.AttacksLeft == 0)
            {
                return;
            }

            int range = fish.Range;

            HashSet<Fish> previousTargets = GetAlreadyAttacked(fish);

            while (fish.AttacksLeft > 0)
            {
                Bb.Update(ai);

                Point fishPoint = fish.Point();
                BitArray dontAttack = Bb.ToBitArray(previousTargets);
                BitArray attackable = new BitArray(dontAttack).Not().And(Bb.TheirFishMap);
                BitArray targetsInAttackable = new BitArray(mission.m_targets()).And(attackable);
                BitArray passableOrTargetInAttackable = Bb.GetPassable(fishPoint).Or(targetsInAttackable);

                var path = Pather.aStar(fishPoint, targetsInAttackable, passableOrTargetInAttackable).ToArray();
                if (path.Length > 1)
                {
                    int attacks = fish.AttacksLeft;
                    MoveAlong(fish, path.Range(0, path.Length - range), true);
                    if (fish.AttacksLeft == attacks)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
                if (fish.MovementLeft == 0)
                {
                    return;
                }
            }
        }

        private static HashSet<Fish> GetAlreadyAttacked(Fish agent)
        {
            HashSet<Fish> previousTargets;
            if (!alreadyAttacked.TryGetValue(agent, out previousTargets))
            {
                previousTargets = new HashSet<Fish>();
                alreadyAttacked.Add(agent, previousTargets);
            }
            return previousTargets;
        }

        private static void Attack(Fish agent, Fish target)
        {
            if (GetAlreadyAttacked(agent).Add(target))
            {
                agent.attack(target);
            }
        }

        private static bool MoveAlong(Fish fish, IEnumerable<Point> path, bool attackAlongTheWay)
        {
            Point[] fullPath = path.ToArray();
            if (fullPath.Length == 0)
            {
                return true;
            }

            var attacks = new Dictionary<Point, IEnumerable<Fish>>();
            if (attackAlongTheWay)
            {
                attacks = targetsEnRoute(fish, path);
                if (attacks.ContainsKey(fish.Point()))
                {
                    attacks[fish.Point()].ForEach(target =>
                    {
                        fish.attack(target);
                    });
                }
            }

            for (int i = 1; i < fullPath.Length && fish.MovementLeft > 0; ++i)
            {
                Point moveTo = fullPath[i];
                fish.move(moveTo.X, moveTo.Y);
                if (attacks.ContainsKey(moveTo))
                {
                    attacks[moveTo].ForEach(target =>
                    {
                        Attack(fish, target);
                    });
                }
            }

            var goal = fullPath.Last();
            return fish.X == goal.X && fish.Y == goal.Y;
        }

        private static Dictionary<Point, IEnumerable<Fish>> targetsEnRoute(Fish fish, IEnumerable<Point> path)
        {
            int attacks = fish.AttacksLeft;
            int range = fish.Range;
            int enemy = 1 - fish.Owner;

            var allPoints = new List<Point>(path);
            allPoints.Add(fish.Point());

            var allTargets = allPoints.SelectMany(
                p => Picker.GetTargets(range, p, enemy)
                    .Select(t => new { p = p, t = t }));

            HashSet<Fish> previousTargets = GetAlreadyAttacked(fish);
            var distinct = new HashSet<Fish>();
            var distinctTargets = allTargets
                .Where(pt => !previousTargets.Contains(pt.t) && distinct.Add(pt.t)).ToArray();

            var bests = new HashSet<Fish>();
            while (attacks > bests.Count && distinct.Any())
            {
                var best = Picker.GetBestTarget(distinct, fish);
                bests.Add(best);
                distinct.Remove(best);
            }

            return distinctTargets.Where(pt => bests.Contains(pt.t))
                .GroupBy(pt => pt.p, pt => pt.t)
                .ToDictionary(g => g.Key, g => (IEnumerable<Fish>)g);
        }

        private static IEnumerable<T> Range<T>(this IEnumerable<T> source, int start, int end)
        {
            return source.Where((s, i) => i >= start && i < end);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var s in source)
            {
                action(s);
            }
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

        public static Point Point(this Mappable m)
        {
            return new Point(m.X, m.Y);
        }
    }
}
