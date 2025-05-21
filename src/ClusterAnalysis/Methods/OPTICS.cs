// Adam Dernis 2025

using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Methods;

/// <summary>
/// A static class containing OPTICS methods.
/// </summary>
public static class OPTICS
{
    /// <summary>
    /// Clusters a set of points using OPTICS.
    /// </summary>
    /// <remarks>
    /// Assumes graph is non-directed.
    /// </remarks>
    /// <typeparam name="T">The type of points to cluster.</typeparam>
    /// <param name="points">The list of points to cluster.</param>
    /// <param name="space">The <see cref="IMetricSpace{T}"/> defining the space.</param>
    /// <param name="minPoints">The number of seeds needed to make a core point.</param>
    /// <param name="range">The maximum range for a point to be a neighbor.</param>
    /// <returns>A list of clusters</returns>
    public static List<List<T>> Cluster<T>(IList<T> points, IMetricSpace<T> space, int minPoints, float range)
    {
        // Create context for child methods
        var context = new Context<T>(points, space, minPoints, range);

        // Iterate every point
        for (int i = 0; i < points.Count; i++)
        {
            // Skip if the point is already processed
            if (context.ProcessedSet.Contains(i))
                continue;

            if(TryUpdateCorePoint(i, context))
            {
                while (context.Seeds.Count > 0)
                {
                    var s = context.Seeds.Dequeue();
                    TryUpdateCorePoint(s, context);
                }
            }
        }

        return MakeClusters(context);
    }

    private static bool TryUpdateCorePoint<T>(int index, Context<T> context)
    {
        // Process and order the point
        int pos = context.ProcessedSet.Count;
        context.Order[pos] = index;
        context.ProcessedSet.Add(index);

        // Get neighbors and core distance
        var coreDist = GetCoreDistance(index, context, out var neighbors);

        // Return if not a core point
        if (float.IsNaN(coreDist))
            return false;

        // Get the point
        T p = context.Points[index];

        foreach(var n in neighbors)
        {
            // Skip processed points
            if (context.ProcessedSet.Contains(n))
                continue;

            float newReachDist = float.Max(coreDist, context.Space.Distance(p, context.Points[n]));
            float oldReachDist = context.Reachability[n];
            if (float.IsPositiveInfinity(oldReachDist))
            {
                context.Reachability[n] = newReachDist;
                context.Seeds.Enqueue(n, newReachDist);
            }
            else if (newReachDist < oldReachDist)
            {
                context.Reachability[n] = newReachDist;

                // Just add the seed earlier in the list. It will already be processed when it pops up a second time
                // NOTE: This is not a great solution, but the current BCL doesn't have a better priority queue
                context.Seeds.Enqueue(n, newReachDist);
            }
        }

        return true;
    }

    private static float GetCoreDistance<T>(int index, Context<T> context, out int[] neighbors)
    {
        // Create a list of index of neighboring points
        var neighborDist = new List<(int, float)>();

        // Check every point to see if its a neighbor point for this point
        T point = context.Points[index];
        for (int i = 0; i < context.Points.Count; i++)
        {
            // A point cannot be its own neighbor
            if (i == index)
                continue;

            // Point is a neighbor if within the range
            var distance = context.Space.Distance(point, context.Points[i]);
            if (distance <= context.Range)
                neighborDist.Add((i, distance));
        }

        // Return NaN if point is not a core point
        if (neighborDist.Count < context.MinPoints)
        {
            neighbors = Array.Empty<int>();
            return float.NaN;
        }

        // Sort by distance
        neighborDist.Sort((x, y) => y.CompareTo(x));

        // Get a list with just the neighbor indices.
        neighbors = neighborDist.Select(x => x.Item1).ToArray();

        // Return the [min points]th distance
        return neighborDist[context.MinPoints-1].Item2;
    }

    private static List<List<T>> MakeClusters<T>(Context<T> context)
    {
        var clusters = new List<List<T>>();

        var cluster = new List<T>();
        for (int o = 0; o < context.Order.Length; o++)
        {
            // Iterate points in order
            int i = context.Order[o];
            float r = context.Reachability[i];

            float next = 0;
            if (i != context.Order.Length - 1)
                next = context.Reachability[i + 1];

            float diff = r - next;

            // Start a new cluster when the reachability drops by more than range
            if (diff > context.Range)
            {
                // Begin next cluster
                if (cluster.Count > 1)
                {
                    clusters.Add(cluster);
                }

                cluster = new List<T>();
            }

            // If the reachability is greater than range
            if (r < context.Range || r is float.PositiveInfinity)
            {
                var p = context.Points[i];
                cluster.Add(p);
            }
        }

        // Add the in progress cluster if not empty
        if (cluster.Count > 1)
        {
            clusters.Add(cluster);
        }

        return clusters;
    }

    private readonly ref struct Context<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context{T}"/> struct.
        /// </summary>
        public Context(IList<T> points, IMetricSpace<T> space, int minPoints, float range)
        {
            Points = points;
            Space = space;
            MinPoints = int.Min(points.Count-1, minPoints);
            Range = range;
            Reachability = new float[points.Count];
            Order = new int[points.Count];
            ProcessedSet = new HashSet<int>();
            Seeds = new PriorityQueue<int, float>();

            // Initialize reachability as not a number
            for (int i = 0; i < points.Count; i++)
                Reachability[i] = float.PositiveInfinity;
        }

        /// <summary>
        /// Gets the points being clustered.
        /// </summary>
        public IList<T> Points { get; }

        /// <summary>
        /// Gets the <see cref="IMetricSpace{T}"/> defining the space.
        /// </summary>
        public IMetricSpace<T> Space { get; }

        /// <summary>
        /// Gets the number of seeds needed to make a core point.
        /// </summary>
        public int MinPoints { get; }
        
        /// <summary>
        /// Gets the max range that a point is consider a neighbor.
        /// </summary>
        public float Range { get; }

        /// <summary>
        /// Gets an array mapping the order of the points.
        /// </summary>
        public int[] Order { get; }

        /// <summary>
        /// Gets an array containing the reachability distance of each point in <see cref="Points"/>.
        /// </summary>
        public float[] Reachability { get; }

        /// <summary>
        /// Gets a set to track which points have been processed.
        /// </summary>
        public HashSet<int> ProcessedSet { get; }

        /// <summary>
        /// Gets a priority queue for iterating seeds.
        /// </summary>
        public PriorityQueue<int, float> Seeds { get; }
    }
}
