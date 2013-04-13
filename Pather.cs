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

        /// <summary>
        /// Returns a connected set of points including start and end
        /// </summary>
        public static IEnumerable<Point> aStar(Point start, Point goal, BitArray passable)
        {
 //    closedset := the empty set    // The set of nodes already evaluated.
            var closedSet = new HashSet<Point>();
 //    openset := {start}    // The set of tentative nodes to be evaluated, initially containing the start node
            var openSet = new LinkedList<Point>();
            openSet.AddFirst(start);
 //    came_from := the empty map    // The map of navigated nodes.
            var cameFrom = new Dictionary<Point, Point>();
 //    g_score[start] := 0    // Cost from start along best known path.
            var g_cost = new Dictionary<Point, int>();
            g_cost[start] = 0;
 //    f_score[start] := g_score[start] + heuristic_cost_estimate(start, goal)
            var f_cost = new Dictionary<Point, int>();
            f_cost[start] = manhattanDistance(start, goal);
            
 //    while openset is not empty
            while (openSet.Count > 0)
            {
 //        current := the node in openset having the lowest f_score[] value
                Point current = openSet.First.Value;
 //        if current = goal
                if (current.Equals(goal))
                {
//            return reconstruct_path(came_from, goal)
                    return buildPath(cameFrom, current);
                }

//        remove current from openset
                openSet.RemoveFirst();

 //        add current to closedset
                closedSet.Add(current);
 //        for each neighbor in neighbor_nodes(current)
                foreach (Point neighbor in expand(current, passable))
                {
//            tentative_g_score := g_score[current] + dist_between(current,neighbor)
//            if neighbor in closedset
//                if tentative_g_score >= g_score[neighbor]
//                    continue

//            if neighbor not in openset or tentative_g_score < g_score[neighbor] 
//                came_from[neighbor] := current
//                g_score[neighbor] := tentative_g_score
//                f_score[neighbor] := g_score[neighbor] + heuristic_cost_estimate(neighbor, goal)
//                if neighbor not in openset
//                    add neighbor to openset
                }

            }

            //    return failure
            return new Point[] { start };
        }

        private static IEnumerable<Point> buildPath(Dictionary<Point, Point> cameFrom, Point point)
        {
            do
            {
                yield return point;
                point = cameFrom[point];
            } while (cameFrom.ContainsKey(point));
        }

        private static IEnumerable<Point> expand(Point point, BitArray passable)
        {
            var points = new List<Point>(4);
            Point left = new Point(point.X - 1, point.Y);
            Point up = new Point(point.X, point.Y - 1);
            Point right = new Point(point.X + 1, point.Y);
            Point down = new Point(point.X, point.Y + 1);
            if (isPassable(left, passable))
            {
                yield return left;
            }
            if (isPassable(up, passable))
            {
                yield return up;
            }
            if (isPassable(right, passable))
            {
                yield return right;
            }
            if (isPassable(down, passable))
            {
                yield return down;
            }
        }

        private static bool isPassable(Point point, BitArray passable)
        {
            return passable[point.Y * Bb.MaxX + point.X];
        }

        private static int manhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point2.X - point1.X) + Math.Abs(point2.Y - point1.Y);
        }
    }
}
