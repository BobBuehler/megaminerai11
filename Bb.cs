using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    public static class Bb
    {
        public static int MaxX;
        public static int MaxY;

        public static HashSet<Tile> CoveSet = new HashSet<Tile>();
        public static HashSet<Tile> OurCoveSet = new HashSet<Tile>();
        public static HashSet<Tile> TheirCoveSet = new HashSet<Tile>();

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

            foreach (var tile in BaseAI.tiles)
            {
                if (tile.Owner == 3)//is a wall
                {
                    WallMap[GetOffset(tile.X, tile.Y)] = true;
                }

            }

            foreach (var tile in BaseAI.tiles)
            {
                if (tile.Owner == 0 || tile.Owner == 1)
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
            }
        }

        public static int GetOffset(int x, int y)
        {
            return (y * MaxX + x);
        }
    }
}
