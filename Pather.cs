using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    public static class Pather
    {

        public class AStarNode
        {
            public Point Point;
            public AStarNode Prev;
            public int Cost;

            public AStarNode(Point point, AStarNode prev, int cost)
            {
                this.Point = point;
                this.Prev = prev;
                this.Cost = cost;
            }
        }

        public static Point aStar(Point start, BitArray want, BitArray passable)
        {
            return aStar(start, p => isSet(p, want), p => 0, passable).Last();
        }

        public static IEnumerable<Point> aStar(Point start, Point goal, BitArray passable)
        {
            return aStar(start, p => p.Equals(goal), p => manhattanDistance(p, goal), passable);
        }

        /// <summary>
        /// Returns a connected set of points including start and end
        /// </summary>
        public static IEnumerable<Point> aStar(Point start, Func<Point, bool> is_goal, Func<Point, int> h, BitArray passable)
        {
 //    closedset := the empty set    // The set of nodes already evaluated.
            var closedSet = new HashSet<Point>();
 //    openset := {start}    // The set of tentative nodes to be evaluated, initially containing the start node
            var openSet = new HashSet<Point>();
            openSet.Add(start);
 //    came_from := the empty map    // The map of navigated nodes.
            var cameFrom = new Dictionary<Point, Point>();
 //    g_score[start] := 0    // Cost from start along best known path.
            var g_cost = new Dictionary<Point, int>();
            g_cost[start] = 0;
 //    f_score[start] := g_score[start] + heuristic_cost_estimate(start, goal)
            var f_cost = new Dictionary<Point, int>();
            f_cost[start] = h(start);
            
 //    while openset is not empty
            while (openSet.Count > 0)
            {
 //        current := the node in openset having the lowest f_score[] value
                Point current = minCostPoint(openSet, f_cost);
 //        if current = goal
                if (is_goal(current))
                {
//            return reconstruct_path(came_from, goal)
                    return buildPath(cameFrom, current).Reverse();
                }

//        remove current from openset
                openSet.Remove(current);

 //        add current to closedset
                closedSet.Add(current);
//        for each neighbor in neighbor_nodes(current)
                int neighborCost = g_cost[current] + 1;
                foreach (Point neighbor in expand(current, passable))
                {
//            if neighbor in closedset
//                if tentative_g_score >= g_score[neighbor]
//                    continue
                    if (closedSet.Contains(neighbor))
                        if (neighborCost >= g_cost[neighbor])
                            continue;

//            if neighbor not in openset or tentative_g_score < g_score[neighbor] 
                    if (!openSet.Contains(neighbor) || neighborCost < g_cost[neighbor])
                    {
//                came_from[neighbor] := current
                        cameFrom[neighbor] = current;
//                g_score[neighbor] := tentative_g_score
                        g_cost[neighbor] = neighborCost;
//                f_score[neighbor] := g_score[neighbor] + heuristic_cost_estimate(neighbor, goal)
                        f_cost[neighbor] = neighborCost + h(neighbor);
//                if neighbor not in openset
//                    add neighbor to openset
                        openSet.Add(neighbor);
                    }
                }

            }

            //    return failure
            return new Point[] { start };
        }

        private static IEnumerable<Point> buildPath(Dictionary<Point, Point> cameFrom, Point point)
        {
            yield return point;
            do
            {
                point = cameFrom[point];
                yield return point;
            } while (cameFrom.ContainsKey(point));
        }

        private static Point minCostPoint(HashSet<Point> openSet, Dictionary<Point, int> costs)
        {
            int minCost = Int32.MaxValue;
            Point minPoint = new Point(-1, -1);
            foreach (var kvp in costs.Where(kvp => openSet.Contains(kvp.Key)))
            {
                if (kvp.Value < minCost)
                {
                    minCost = kvp.Value;
                    minPoint = kvp.Key;
                }
            }
            return minPoint;
        }

        private static IEnumerable<Point> expand(Point point, BitArray passable)
        {
            var points = new List<Point>(4);
            Point left = new Point(point.X - 1, point.Y);
            Point up = new Point(point.X, point.Y - 1);
            Point right = new Point(point.X + 1, point.Y);
            Point down = new Point(point.X, point.Y + 1);
            if (isSet(left, passable))
            {
                yield return left;
            }
            if (isSet(up, passable))
            {
                yield return up;
            }
            if (isSet(right, passable))
            {
                yield return right;
            }
            if (isSet(down, passable))
            {
                yield return down;
            }
        }

        private static bool isSet(Point point, BitArray bb)
        {
            return
                point.X >= 0 &&
                point.Y >= 0 &&
                point.X < Bb.MaxX &&
                point.Y < Bb.MaxY &&
                bb[point.Y * Bb.MaxX + point.X];
        }

        private static int manhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point2.X - point1.X) + Math.Abs(point2.Y - point1.Y);
        }
    }
}
