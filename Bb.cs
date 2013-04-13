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
        public static BitArray Fish;
        public static BitArray Wall;
        public static BitArray Trash;

        public static void init()
        {
            Fish = new BitArray(AI.tiles.Length);
            Wall = new BitArray(AI.tiles.Length);
            Trash = new BitArray(AI.tiles.Length);

            //BaseAI.fishes.ToList().ForEach(fish =>Fish[fish.X*)
        }
    }
}
