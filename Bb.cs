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


        public static HashSet<Fish> OurStarfishSet = new HashSet<Fish>();
        public static HashSet<Fish> OurSpongesSet = new HashSet<Fish>();
        public static HashSet<Fish> OurAngelfishesSet = new HashSet<Fish>();
        public static HashSet<Fish> OurSnailsSet = new HashSet<Fish>();
        public static HashSet<Fish> OurUrchinsSet = new HashSet<Fish>();
        public static HashSet<Fish> OurOctopiSet = new HashSet<Fish>();
        public static HashSet<Fish> OurTomcodsSet = new HashSet<Fish>();
        public static HashSet<Fish> OurSharksSet = new HashSet<Fish>();
        public static HashSet<Fish> OurCuttlefishesSet = new HashSet<Fish>();
        public static HashSet<Fish> OurShrimpsSet = new HashSet<Fish>();
        public static HashSet<Fish> OurEelsSet = new HashSet<Fish>();
        public static HashSet<Fish> OurJellyfishSet = new HashSet<Fish>();

        public static HashSet<Fish> TheirStarfishSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirSpongesSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirAngelfishesSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirSnailsSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirUrchinsSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirOctopiSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirTomcodsSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirSharksSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirCuttlefishesSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirShrimpsSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirEelsSet = new HashSet<Fish>();
        public static HashSet<Fish> TheirJellyfishSet = new HashSet<Fish>();

        public static BitArray OurStarfishMap;
        public static BitArray OurSpongesMap;
        public static BitArray OurAngelfishesMap;
        public static BitArray OurSnailsMap;
        public static BitArray OurUrchinsMap;
        public static BitArray OurOctopiMap;
        public static BitArray OurTomcodsMap;
        public static BitArray OurSharksMap;
        public static BitArray OurCuttlefishesMap;
        public static BitArray OurShrimpsMap;
        public static BitArray OurEelsMap;
        public static BitArray OurJellyfishMap;

        public static BitArray TheirStarfishMap;
        public static BitArray TheirSpongesMap;
        public static BitArray TheirAngelfishesMap;
        public static BitArray TheirSnailsMap;
        public static BitArray TheirUrchinsMap;
        public static BitArray TheirOctopiMap;
        public static BitArray TheirTomcodsMap;
        public static BitArray TheirSharksMap;
        public static BitArray TheirCuttlefishesMap;
        public static BitArray TheirShrimpsMap;
        public static BitArray TheirEelsMap;
        public static BitArray TheirJellyfishMap;


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

        public static BitArray UnMappable;

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

            OurStarfishMap = new BitArray(AI.tiles.Length);
            OurSpongesMap = new BitArray(AI.tiles.Length);
            OurAngelfishesMap = new BitArray(AI.tiles.Length);
            OurSnailsMap = new BitArray(AI.tiles.Length);
            OurUrchinsMap = new BitArray(AI.tiles.Length);
            OurOctopiMap = new BitArray(AI.tiles.Length);
            OurTomcodsMap = new BitArray(AI.tiles.Length);
            OurSharksMap = new BitArray(AI.tiles.Length);
            OurCuttlefishesMap = new BitArray(AI.tiles.Length);
            OurShrimpsMap = new BitArray(AI.tiles.Length);
            OurEelsMap = new BitArray(AI.tiles.Length);
            OurJellyfishMap = new BitArray(AI.tiles.Length);

            TheirStarfishMap = new BitArray(AI.tiles.Length);
            TheirSpongesMap = new BitArray(AI.tiles.Length);
            TheirAngelfishesMap = new BitArray(AI.tiles.Length);
            TheirSnailsMap = new BitArray(AI.tiles.Length);
            TheirUrchinsMap = new BitArray(AI.tiles.Length);
            TheirOctopiMap = new BitArray(AI.tiles.Length);
            TheirTomcodsMap = new BitArray(AI.tiles.Length);
            TheirSharksMap = new BitArray(AI.tiles.Length);
            TheirCuttlefishesMap = new BitArray(AI.tiles.Length);
            TheirShrimpsMap = new BitArray(AI.tiles.Length);
            TheirEelsMap = new BitArray(AI.tiles.Length);
            TheirJellyfishMap = new BitArray(AI.tiles.Length);

            
            
            //Fill Reef Maps
            foreach (var tile in BaseAI.tiles)
            {
                if (tile.Damages == ai.playerID())
                {
                    OurReef[GetOffset(tile.X, tile.Y)] = true;
                }
                else if (tile.Damages == 1 - ai.playerID())
                {
                    TheirReef[GetOffset(tile.X, tile.Y)] = true;
                }
                else
                {
                    NeutralReef[GetOffset(tile.X, tile.Y)] = true;
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
                    switch (fish.Species)
                    {
                        case 0:
                            OurStarfishMap[GetOffset(fish.X, fish.Y)] = true;
                            OurStarfishSet.Add(fish);
                            break;
                        case 1:
                            OurSpongesMap[GetOffset(fish.X, fish.Y)] = true;
                            OurSpongesSet.Add(fish);
                            break;
                        case 2:
                            OurAngelfishesMap[GetOffset(fish.X, fish.Y)] = true;
                            OurAngelfishesSet.Add(fish);
                            break;
                        case 3:
                            OurSnailsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurSnailsSet.Add(fish);
                            break;
                        case 4:
                            OurUrchinsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurUrchinsSet.Add(fish);
                            break;
                        case 5:
                            OurOctopiMap[GetOffset(fish.X, fish.Y)] = true;
                            OurOctopiSet.Add(fish);
                            break;
                        case 6:
                            OurTomcodsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurTomcodsSet.Add(fish);
                            break;
                        case 7:
                            OurSharksMap[GetOffset(fish.X, fish.Y)] = true;
                            OurSharksSet.Add(fish);
                            break;
                        case 8:
                            OurCuttlefishesMap[GetOffset(fish.X, fish.Y)] = true;
                            OurCuttlefishesSet.Add(fish);
                            break;
                        case 9:
                            OurShrimpsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurShrimpsSet.Add(fish);
                            break;
                        case 10:
                            OurEelsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurEelsSet.Add(fish);
                            break;
                        case 11:
                            OurJellyfishMap[GetOffset(fish.X, fish.Y)] = true;
                            OurJellyfishSet.Add(fish);
                            break;
                    }
                }
                else
                {
                    TheirFishMap[GetOffset(fish.X, fish.Y)] = true;
                    switch (fish.Species)
                    {
                        case 0:
                            TheirStarfishMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirStarfishSet.Add(fish);
                            break;
                        case 1:
                            TheirSpongesMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirSpongesSet.Add(fish);
                            break;
                        case 2:
                            TheirAngelfishesMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirAngelfishesSet.Add(fish);
                            break;
                        case 3:
                            TheirSnailsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirSnailsSet.Add(fish);
                            break;
                        case 4:
                            TheirUrchinsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirUrchinsSet.Add(fish);
                            break;
                        case 5:
                            TheirOctopiMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirOctopiSet.Add(fish);
                            break;
                        case 6:
                            TheirTomcodsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirTomcodsSet.Add(fish);
                            break;
                        case 7:
                            TheirSharksMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirSharksSet.Add(fish);
                            break;
                        case 8:
                            TheirCuttlefishesMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirCuttlefishesSet.Add(fish);
                            break;
                        case 9:
                            TheirShrimpsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirShrimpsSet.Add(fish);
                            break;
                        case 10:
                            TheirEelsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirEelsSet.Add(fish);
                            break;
                        case 11:
                            TheirJellyfishMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirJellyfishSet.Add(fish);
                            break;
                    }
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

            OurTrashMap = new BitArray(TrashMap).And(OurReef);
            TheirTrashMap = new BitArray(TrashMap).And(TheirReef);
        }

        public static void Update(AI ai)
        {
            OurStarfishMap.SetAll(false);
            OurSpongesMap.SetAll(false);
            OurAngelfishesMap.SetAll(false);
            OurSnailsMap.SetAll(false);
            OurUrchinsMap.SetAll(false);
            OurOctopiMap.SetAll(false);
            OurTomcodsMap.SetAll(false);
            OurSharksMap.SetAll(false);
            OurCuttlefishesMap.SetAll(false);
            OurShrimpsMap.SetAll(false);
            OurEelsMap.SetAll(false);
            OurJellyfishMap.SetAll(false);

            TheirStarfishMap.SetAll(false);
            TheirSpongesMap.SetAll(false);
            TheirAngelfishesMap.SetAll(false);
            TheirSnailsMap.SetAll(false);
            TheirUrchinsMap.SetAll(false);
            TheirOctopiMap.SetAll(false);
            TheirTomcodsMap.SetAll(false);
            TheirSharksMap.SetAll(false);
            TheirCuttlefishesMap.SetAll(false);
            TheirShrimpsMap.SetAll(false);
            TheirEelsMap.SetAll(false);
            TheirJellyfishMap.SetAll(false);

            OurStarfishSet.Clear();
            OurSpongesSet.Clear();
            OurAngelfishesSet.Clear();
            OurSnailsSet.Clear();
            OurUrchinsSet.Clear();
            OurOctopiSet.Clear();
            OurTomcodsSet.Clear();
            OurSharksSet.Clear();
            OurCuttlefishesSet.Clear();
            OurShrimpsSet.Clear();
            OurEelsSet.Clear();
            OurJellyfishSet.Clear();

            TheirStarfishSet.Clear();
            TheirSpongesSet.Clear();
            TheirAngelfishesSet.Clear();
            TheirSnailsSet.Clear();
            TheirUrchinsSet.Clear();
            TheirOctopiSet.Clear();
            TheirTomcodsSet.Clear();
            TheirSharksSet.Clear();
            TheirCuttlefishesSet.Clear();
            TheirShrimpsSet.Clear();
            TheirEelsSet.Clear();
            TheirJellyfishSet.Clear();

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
                    switch (fish.Species)
                    {
                        case 0:
                            OurStarfishMap[GetOffset(fish.X, fish.Y)] = true;
                            OurStarfishSet.Add(fish);
                            break;
                        case 1:
                            OurSpongesMap[GetOffset(fish.X, fish.Y)] = true;
                            OurSpongesSet.Add(fish);
                            break;
                        case 2:
                            OurAngelfishesMap[GetOffset(fish.X, fish.Y)] = true;
                            OurAngelfishesSet.Add(fish);
                            break;
                        case 3:
                            OurSnailsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurSnailsSet.Add(fish);
                            break;
                        case 4:
                            OurUrchinsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurUrchinsSet.Add(fish);
                            break;
                        case 5:
                            OurOctopiMap[GetOffset(fish.X, fish.Y)] = true;
                            OurOctopiSet.Add(fish);
                            break;
                        case 6:
                            OurTomcodsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurTomcodsSet.Add(fish);
                            break;
                        case 7:
                            OurSharksMap[GetOffset(fish.X, fish.Y)] = true;
                            OurSharksSet.Add(fish);
                            break;
                        case 8:
                            OurCuttlefishesMap[GetOffset(fish.X, fish.Y)] = true;
                            OurCuttlefishesSet.Add(fish);
                            break;
                        case 9:
                            OurShrimpsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurShrimpsSet.Add(fish);
                            break;
                        case 10:
                            OurEelsMap[GetOffset(fish.X, fish.Y)] = true;
                            OurEelsSet.Add(fish);
                            break;
                        case 11:
                            OurJellyfishMap[GetOffset(fish.X, fish.Y)] = true;
                            OurJellyfishSet.Add(fish);
                            break;
                    }
                }
                else
                {
                    TheirFishMap[GetOffset(fish.X, fish.Y)] = true;
                    switch (fish.Species)
                    {
                        case 0:
                            TheirStarfishMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirStarfishSet.Add(fish);
                            break;
                        case 1:
                            TheirSpongesMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirSpongesSet.Add(fish);
                            break;
                        case 2:
                            TheirAngelfishesMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirAngelfishesSet.Add(fish);
                            break;
                        case 3:
                            TheirSnailsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirSnailsSet.Add(fish);
                            break;
                        case 4:
                            TheirUrchinsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirUrchinsSet.Add(fish);
                            break;
                        case 5:
                            TheirOctopiMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirOctopiSet.Add(fish);
                            break;
                        case 6:
                            TheirTomcodsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirTomcodsSet.Add(fish);
                            break;
                        case 7:
                            TheirSharksMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirSharksSet.Add(fish);
                            break;
                        case 8:
                            TheirCuttlefishesMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirCuttlefishesSet.Add(fish);
                            break;
                        case 9:
                            TheirShrimpsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirShrimpsSet.Add(fish);
                            break;
                        case 10:
                            TheirEelsMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirEelsSet.Add(fish);
                            break;
                        case 11:
                            TheirJellyfishMap[GetOffset(fish.X, fish.Y)] = true;
                            TheirJellyfishSet.Add(fish);
                            break;
                    }
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

            OurTrashMap = new BitArray(TrashMap).And(OurReef);
            TheirTrashMap = new BitArray(TrashMap).And(TheirReef);
        }


        public static BitArray GetNAwayFromPoint(int n, Point point)
        {
            BitArray nAwayFromPoint = new BitArray(AI.tiles.Length);
            int x = point.X;
            int y = point.Y;


            for (int i = 1; i <= n; i++)
            {
                if (y + i < MaxY)
                {
                    nAwayFromPoint[GetOffset(x, y + i)] = true;
                }
                if (y - i >= 0)
                {
                    nAwayFromPoint[GetOffset(x, y - i)] = true;
                }
                if (x + i < MaxX)
                {
                    nAwayFromPoint[GetOffset(x + i, y)] = true;
                }
                if (x - i >= 0)
                {
                    nAwayFromPoint[GetOffset(x - i, y)] = true;
                }

                for (int j = 1; j <= i - 1; j++)
                {
                    if (x + j < MaxX && y + i - j < MaxY && y + i - j >= 0)
                    {
                        nAwayFromPoint[GetOffset(x + j, y + i - j)] = true;
                    }
                    if (x - j >= 0 && y + i - j < MaxY && y + i - j >= 0)
                    {
                        nAwayFromPoint[GetOffset(x - j, y + i - j)] = true;
                    }
                    if (x + j < MaxX && y - i + j < MaxY && y - i + j >= 0)
                    {
                        nAwayFromPoint[GetOffset(x + j, y - i + j)] = true;
                    }
                    if (x - j >= 0 && y - i + j < MaxY && y - i + j >= 0)
                    {
                        nAwayFromPoint[GetOffset(x - j, y - i + j)] = true;
                    }
                }
            }

            return nAwayFromPoint;
        }

        public static BitArray GetNAwayFromPointMovable(int n, Point point)
        {
            int x = point.X;
            int y = point.Y;
            BitArray nAwayFromPointMovable = new BitArray(AI.tiles.Length);





            return nAwayFromPointMovable;
        }

        public static bool IsReachable(Point origin, Point destination, BitArray bB)
        {
            Queue<Point> Q = new Queue<Point>();
            HashSet<Point> Visited = new HashSet<Point>();

            Q.Enqueue(origin);
            while (Q.Count > 0)
            {
                Point current = Q.Dequeue();
            }

            return true;
        }

        public static BitArray GetPassable()
        {
            return new BitArray(Bb.WallMap).Or(Bb.CoveMap).Or(Bb.FishMap).Or(Bb.TrashMap).Not();
        }

        public static BitArray GetPassable(Point forcePassable)
        {
            BitArray passable = GetPassable();
            passable.Set(GetOffset(forcePassable.X, forcePassable.Y), true);
            return passable;
        }

        public static int GetOffset(int x, int y)
        {
            return (y * MaxX + x);
        }

        public static bool Get(BitArray board, int x, int y)
        {
            return board[GetOffset(x, y)];
        }

        public static void Set(BitArray board, Point point, bool value)
        {
            board.Set(GetOffset(point.X, point.Y), value);
        }

        public static string ToString(BitArray board)
        {
            StringBuilder builder = new StringBuilder();
            for (int y = 0; y < MaxY; ++y)
            {
                for (int x = 0; x < MaxX; ++x)
                {
                    builder.Append(Get(board, x, y) ? '#' : '-');
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }

        public static string ToString(BitArray board, Point mark)
        {
            StringBuilder builder = new StringBuilder();
            for (int y = 0; y < MaxY; ++y)
            {
                for (int x = 0; x < MaxX; ++x)
                {
                    builder.Append((mark.X == x && mark.Y == y) ? 'X' : Get(board, x, y) ? '#' : ' ');
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
