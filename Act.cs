using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pizza
{
    public enum ActType
    {
        move,
        pickup,
        drop,
        attack
    }

    public class Act
    {
        public ActType type;
        public int x;
        public int y;
        public Tile tile;
        public int weight;
        public Fish target;

        public bool act(Fish fish)
        {
            switch (type)
            {
                case ActType.move:
                    return fish.move(x, y);
                case ActType.pickup:
                    return fish.pickUp(tile, weight);
                case ActType.drop:
                    return fish.drop(tile, weight);
                case ActType.attack:
                    return fish.attack(target);
            }
            return false;
        }
    }
}
