// Adam Dernis 2023

using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Methods;

/// <summary>
/// A static class containing DBSCAN methods.
/// </summary>
public static class DBSCAN
{
    private const int UNCLASSIFIED_ID = 0;
    private const int NOISE_ID = -1;

    /// <summary>
    /// Clusters a set of points using DBSCAN.
    /// </summary>
    /// <remarks>
    /// Assumes graph is non-directed.
    /// </remarks>
    /// <typeparam name="T">The type of points to cluster.</typeparam>
    /// <param name="points">The list of points to cluster.</param>
    /// <param name="space">The <see cref="IMetricSpace{T}"/> defining the space.</param>
    /// <param name="range">The max range a point is consider a seed.</param>
    /// <param name="minPoints">The number of seeds needed to make a core point.</param>
    /// <returns>A list of clusters</returns>
    public static List<List<T>> Cluster<T>(IList<T> points, IMetricSpace<T> space, float range, int minPoints)
    {
        // Create cluster list and context for child methods
        var clusters = new List<List<T>>();
        var context = new Context<T>(points, space, range, minPoints);

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
        var seeds = GetSeeds(index, context);

        // Check if point is not a core point
        if (seeds.Count < context.MinPoints)
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
            var childSeeds = GetSeeds(s, context);

            // Check if point is a core point
            if (childSeeds.Count >= context.MinPoints)
            {
                // Add child seeds to stack
                seeds.AddRange(childSeeds);
            }
        }
    }

    private static List<int> GetSeeds<T>(int index, Context<T> context)
    {
        // Create a list of index of seed points
        var seeds = new List<int>();

        // Check every point to see if its a seed point for this point
        T point = context.Points[index];
        for (int i = 0; i < context.Points.Count; i++)
        {
            // A point cannot be its own seed
            if (i == index)
                continue;

            // Point is a seed if within the seed range
            if (context.Space.Distance(point, context.Points[i]) <= context.Range)
                seeds.Add(i);
        }

        return seeds;
    }

    private ref struct Context<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context{T}"/> struct.
        /// </summary>
        public Context(IList<T> points, IMetricSpace<T> space, float range, int minPoints)
        {
            Points = points;
            Space = space;
            Range = range;
            MinPoints = minPoints;
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
        /// Gets the max range that a point is consider a seed.
        /// </summary>
        public float Range { get; }

        /// <summary>
        /// Gets the number of seeds needed to make a core point.
        /// </summary>
        public int MinPoints { get; }

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
