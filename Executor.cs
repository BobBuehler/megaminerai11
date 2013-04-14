using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    class Executor
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
                    case Objective.
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

            Point fishPoint = new Point(fish.X, fish.Y);

            BitArray passable = Bb.GetPassable();
            passable.Set(Bb.GetOffset(fishPoint.X, fishPoint.Y), true);

            BitArray targets = new BitArray(mission.m_targets).And(passable);

            var path = Pather.aStar(fishPoint, targets, passable).ToArray();
            for (int i = 1; i < path.Length && fish.MovementLeft > 0; ++i)
            {
                Point moveTo = path[i];
                Console.WriteLine("({0},{1}) moving to {2}", fish.X, fish.Y, moveTo);
                fish.move(moveTo.X, moveTo.Y);
            }
        }
    }
}
