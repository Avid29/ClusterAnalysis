// Adam Dernis 2023

using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Methods;

/// <summary>
/// A static class containing OPTICS methods.
/// </summary>
public static class OPTICS
{
    private const int UNCLASSIFIED_ID = 0;
    private const int NOISE_ID = -1;

    /// <summary>
    /// Clusters a set of points using OPTICS.
    /// </summary>
    /// <remarks>
    /// Assumes graph is non-directed.
    /// </remarks>
    /// <typeparam name="T">The type of points to cluster.</typeparam>
    /// <param name="points">The list of points to cluster.</param>
    /// <param name="space">The <see cref="IMetricSpace{T}"/> defining the space.</param>
    /// <param name="pointCount">The number of seeds needed to make a core point.</param>
    /// <param name="maxRadius">The maximum radius allowed for a core point.</param>
    /// <returns>A list of clusters</returns>
    public static List<List<T>> Cluster<T>(IList<T> points, IMetricSpace<T> space, int pointCount, float maxRadius)
    {
        // Create cluster list and context for child methods
        var clusters = new List<List<T>>();
        var context = new Context<T>(points, space, pointCount, maxRadius);

        // Iterate every point
        for (int i = 0; i < points.Count; i++)
        {
            // Skip if the point is already a member of a cluster
            if (context.ClusterIds[i] != UNCLASSIFIED_ID)
                continue;

            // Attempt to create a cluster around the point
            if (TryCreateCluster(i, context, out var cluster))
            {
                clusters.Add(cluster!);
            }
        }

        return clusters;
    }

    private static bool TryCreateCluster<T>(int index, Context<T> context, out List<T>? cluster)
    {
        // Get the list of seeding points
        var radius = GetSeeds(index, context, out var seeds);

        // Check if point is not a core point
        if (radius > context.MaxRadius || radius is float.NaN)
        {
            // Assign as noise
            context.ClusterIds[index] = NOISE_ID;

            // Failed to create a cluster
            cluster = null;
            return false;
        }

        // Create cluster containing core point
        int id = context.CurrentClusterId += 1;
        cluster = new List<T> {context.Points[index]};
        context.ClusterIds[index] = id;

        // Expand the cluster recursively
        ExpandCluster(cluster, seeds, context);
        return true;
    }

    private static void ExpandCluster<T>(List<T> cluster, List<int> seeds, Context<T> context)
    {
        // Grab the cluster id
        int id = context.CurrentClusterId;

        // Seeds is used as a stack for depth first search
        while (seeds.Count > 0)
        {
            // Pop seed. Grab index and value
            int s = seeds[^1];
            T p = context.Points[s];
            seeds.RemoveAt(seeds.Count - 1);

            // Track the point's old cluster id
            var oldId = context.ClusterIds[s];

            // Skip already classified points
            if (oldId is not (UNCLASSIFIED_ID or NOISE_ID))
                continue;

            // Add point to cluster
            cluster.Add(p);
            context.ClusterIds[s] = id;
            
            // This is an optimization because noise cannot be a core point
            if (oldId is NOISE_ID)
                continue;

            // Get child's seeds in order to expand
            float radius = GetSeeds(s, context, out var childSeeds);

            // Check if point is a core point
            if (radius <= context.MaxRadius)
            {
                // Add child seeds to stack
                seeds.AddRange(childSeeds);
            }
        }
    }

    private static float GetSeeds<T>(int index, Context<T> context, out List<int> seeds)
    {   
        T point = context.Points[index];

        // Find this point's distance from every other
        var indexDistPairs = new List<(int, float)>();
        for (int i = 0; i < context.Points.Count; i++)
        {
            // A point cannot be its own seed
            if (i == index)
                continue;

            T p = context.Points[i];
            indexDistPairs.Add((i, context.Space.Distance(p, point)));
        }
        
        // Filter NaN distances
        // Sort by distance
        // Take the nearest [PointCount] items, or fewer
        indexDistPairs = indexDistPairs
            .Where(x => x.Item2 is not float.NaN)
            .OrderBy(x => x.Item2)
            .Take(int.Min(context.PointCount, indexDistPairs.Count))
            .ToList();
        
        // Get the seeds
        // Grab indices
        // Convert to list
        seeds = indexDistPairs
            .Select(x => x.Item1)
            .ToList();

        // Return NaN if there are not enough seeds
        if (seeds.Count < context.PointCount)
            return float.NaN;

        // Return the distance of the further seed
        return indexDistPairs[context.PointCount-1].Item2;
    }

    private ref struct Context<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context{T}"/> struct.
        /// </summary>
        public Context(IList<T> points, IMetricSpace<T> space, int pointCount, float maxRadius)
        {
            Points = points;
            Space = space;
            PointCount = int.Min(points.Count -1, pointCount);
            MaxRadius = maxRadius;
            ClusterIds = new int[Points.Count];
            CurrentClusterId = 0;
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
        public int PointCount { get; }

        /// <summary>
        /// Gets the maximum radius allowed for a core point.
        /// </summary>
        public float MaxRadius { get; }

        /// <summary>
        /// Gets an array for containing the id of points in <see cref="Points"/>.
        /// </summary>
        public int[] ClusterIds { get; }

        /// <summary>
        /// Gets or sets the current cluster id.
        /// </summary>
        public int CurrentClusterId { get; set; }
    }
}
