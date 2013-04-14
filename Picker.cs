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

            foreach (Fish fish in BaseAI.fishes)
            {
                int manhDist = (Math.Abs(fish.X - point.X) + Math.Abs(fish.Y - point.Y));
                if(manhDist <= range && fish.Owner == enemyId)
                {
                    targets.Add(fish);
                }
            }
            return targets;
        }

        public static Fish GetBestTarget(IEnumerable<Fish> targets, Fish attacker)
        {
            IComparer<Fish> myComparer = (IComparer<Fish>)new sortTargetsHelper(attacker);
            List<Fish> targetList = targets.ToList();
            targetList.Sort(myComparer);

            return targetList[0];
            
            
        }


        public class sortTargetsHelper : IComparer<Fish>
        {
            Fish attacker;
            public sortTargetsHelper(Fish a_attacker)
            {
                attacker = a_attacker;
            }

            int IComparer<Fish>.Compare(Fish a, Fish b)
            {
                if (a.CurrentHealth <= attacker.AttackPower && b.CurrentHealth > attacker.AttackPower)
                {
                    return 1;
                }
                else if (b.CurrentHealth <= attacker.AttackPower && a.CurrentHealth > attacker.AttackPower)
                {
                    return -1;
                }
                else //if(a.CurrentHealth <= attacker.AttackPower && b.CurrentHealth <= attacker.AttackPower)
                {
                    if (a.Species == (int)AI.SpeciesIndex.SEA_STAR && b.Species != (int)AI.SpeciesIndex.SEA_STAR)
                    {
                        return 1;
                    }
                    else if (b.Species == (int)AI.SpeciesIndex.SEA_STAR && a.Species != (int)AI.SpeciesIndex.SEA_STAR)
                    {
                        return -1;
                    }
                    else if (a.Species == (int)AI.SpeciesIndex.SEA_URCHIN && b.Species != (int)AI.SpeciesIndex.SEA_URCHIN)
                    {
                        return -1;
                    }
                    else if (b.Species == (int)AI.SpeciesIndex.SEA_URCHIN && a.Species != (int)AI.SpeciesIndex.SEA_URCHIN)
                    {
                        return 1;
                    }
                    else if (a.Species == (int)AI.SpeciesIndex.SEA_URCHIN && b.Species == (int)AI.SpeciesIndex.SEA_URCHIN)
                    {
                        return 0;
                    }
                    else
                    {
                        if (a.CarryingWeight > b.CarryingWeight)
                        {
                            return 1;
                        }
                        else if (b.CarryingWeight < a.CarryingWeight)
                        {
                            return -1;
                        }
                        else
                        {
                            if (a.CurrentHealth > b.CurrentHealth)
                            {
                                return 1;
                            }
                            else if (b.CurrentHealth > a.CurrentHealth)
                            {
                                return -1;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
        }

    }
}
