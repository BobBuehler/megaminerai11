using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    static class Picker
    {
        public static List<Fish> GetTargets(int range, Point point, int enemyId)
        {
            List<Fish> targets = new List<Fish>();


            foreach (Point p in Bb.BitArrayToList(new BitArray(Bb.FishMap).And(Bb.GetNAwayFromPoint(range, point))))
            {
                foreach (Fish fish in BaseAI.fishes)
                {
                    if (fish.Owner == enemyId && fish.X == p.X && fish.Y == p.Y)
                    {
                        targets.Add(fish);
                    }
                }
            }
            return targets;

        }

        public static Fish GetBestTarget(List<Fish> targets)
        {
            //Fish bestTarget;

            targets.Sort((x, y) => x.CurrentHealth.CompareTo(y.CurrentHealth));

            return targets[0];
        }
    }
}
