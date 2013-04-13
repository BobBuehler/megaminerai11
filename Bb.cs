using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    static class Bb
    {
        public static int MaxX;
        public static int MaxY;



        public static HashSet<Tile> CoveSet = new HashSet<Tile>();
        public static HashSet<Tile> OurCoveSet = new HashSet<Tile>();
        public static HashSet<Tile> TheirCoveSet = new HashSet<Tile>();

        public static BitArray OurReef;
        public static BitArray TheirReef;
        public static BitArray NeutralReef;

        public static BitArray FishMap;
        public static BitArray OurFishMap;
        public static BitArray TheirFishMap;

        public static BitArray WallMap;

        public static BitArray TrashMap;
        public static BitArray OurTrashMap;
        public static BitArray TheirTrashMap;

        public static BitArray OurCoveMap;
        public static BitArray CoveMap;
        public static BitArray TheirCoveMap;

        public static void init(AI ai)
        {
            MaxX = ai.mapWidth();
            MaxY = ai.mapHeight();


            OurReef = new BitArray(AI.tiles.Length);
            TheirReef = new BitArray(AI.tiles.Length);
            NeutralReef = new BitArray(AI.tiles.Length);


            FishMap = new BitArray(AI.tiles.Length);
            OurFishMap = new BitArray(AI.tiles.Length);
            TheirFishMap = new BitArray(AI.tiles.Length);

            WallMap = new BitArray(AI.tiles.Length);

            TrashMap = new BitArray(AI.tiles.Length);
            OurTrashMap = new BitArray(AI.tiles.Length);
            TheirTrashMap = new BitArray(AI.tiles.Length);

            CoveMap = new BitArray(AI.tiles.Length);
            OurCoveMap = new BitArray(AI.tiles.Length);
            TheirCoveMap = new BitArray(AI.tiles.Length);

            //Fill Reef Maps
            foreach (var tile in BaseAI.tiles)
            {
                if (ai.playerID() == 0)
                {
                    if (tile.X <= 14)
                    {
                        OurReef[GetOffset(tile.X, tile.Y)] = true;
                    }
                    else if (tile.X > 14 && tile.X <= 24)
                    {
                        NeutralReef[GetOffset(tile.X, tile.Y)] = true;
                    }
                    else
                    {
                        TheirReef[GetOffset(tile.X, tile.Y)] = true;
                    }
                }
                else
                {
                    if (tile.X <= 14)
                    {
                        TheirReef[GetOffset(tile.X, tile.Y)] = true;
                    }
                    else if (tile.X > 14 && tile.X <= 24)
                    {
                        NeutralReef[GetOffset(tile.X, tile.Y)] = true;
                    }
                    else
                    {
                        OurReef[GetOffset(tile.X, tile.Y)] = true;
                    }
                }
            }

            //BaseAI.fishes.ToList().ForEach(fish => FishMap.Set(GetOffset(fish.X, fish.Y), true));

            //fill fish maps
            foreach (var fish in BaseAI.fishes)
            {
                FishMap[GetOffset(fish.X, fish.Y)] = true;
                if (fish.Owner == ai.playerID())
                {
                    OurFishMap[GetOffset(fish.X, fish.Y)] = true;
                }
                else
                {
                    TheirFishMap[GetOffset(fish.X, fish.Y)] = true;
                }
            }

            //fill wall & cove map & trash map
            foreach (var tile in BaseAI.tiles)
            {
                if (tile.Owner == 3)//is a wall
                {
                    WallMap[GetOffset(tile.X, tile.Y)] = true;
                }
                else if (tile.Owner == 0 || tile.Owner == 1)
                {
                    CoveSet.Add(tile);
                    CoveMap[GetOffset(tile.X, tile.Y)] = true;

                    if (tile.Owner == ai.playerID())
                    {
                        OurCoveSet.Add(tile);
                        OurCoveMap[GetOffset(tile.X, tile.Y)] = true;
                    }
                    else
                    {
                        TheirCoveSet.Add(tile);
                        TheirCoveMap[GetOffset(tile.X, tile.Y)] = true;
                    }
                }

                if (tile.TrashAmount > 0)
                {
                    TrashMap[GetOffset(tile.X, tile.Y)] = true;
                }
            }

            OurTrashMap = TrashMap.And(OurReef);
            TheirTrashMap = TrashMap.And(TheirReef);
        }

        public static void Update(AI ai)
        {
            FishMap.SetAll(false);
            TheirFishMap.SetAll(false);
            OurFishMap.SetAll(false);

            TrashMap.SetAll(false);
            OurTrashMap.SetAll(false);
            TheirTrashMap.SetAll(false);

            //update fish maps
            foreach (var fish in BaseAI.fishes)
            {
                FishMap[GetOffset(fish.X, fish.Y)] = true;
                if (fish.Owner == ai.playerID())
                {
                    OurFishMap[GetOffset(fish.X, fish.Y)] = true;
                }
                else
                {
                    TheirFishMap[GetOffset(fish.X, fish.Y)] = true;
                }
            }

            //update trash map
            foreach (var tile in BaseAI.tiles)
            {
                if (tile.TrashAmount > 0)
                {
                    TrashMap[GetOffset(tile.X, tile.Y)] = true;
                }
            }

            OurTrashMap = TrashMap.And(OurReef);
            TheirTrashMap = TrashMap.And(TheirReef);
        }

        public static int GetOffset(int x, int y)
        {
            return (y * MaxX + x);
        }
    }
}
