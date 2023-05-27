// Adam Dernis 2023
// Written at https://youtu.be/aMvMKCRSbgg?t=442

using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Methods;

/// <summary>
/// A static class containing connected components methods.
/// </summary>
public static class ConnectedComponents
{
    /// <summary>
    /// Clusters a set of points using connected components.
    /// </summary>
    /// <remarks>
    /// Assumes graph is non-directed.
    /// </remarks>
    /// <typeparam name="T">The type of points to cluster.</typeparam>
    /// <param name="points">The list of points to cluster.</param>
    /// <param name="space">The <see cref="IMetricSpace{T}"/> defining the space.</param>
    /// <param name="range">The maximum range that components are considered connected.</param>
    /// <returns>A list of clusters.</returns>
    public static List<List<T>> Cluster<T>(IList<T> points, IMetricSpace<T> space, float range = float.PositiveInfinity)
    {
        // Create cluster list and context for child methods
        var clusters = new List<List<T>>();
        var context = new Context<T>(points, space, range);

        // Iterate every point
        for (int i = 0; i < points.Count; i++)
        {
            // Skip if the point is already a member of a cluster
            if (context.ClusterIds[i] != 0)
                continue;

            // Create a cluster around point i
            var cluster = CreateCluster(i, context);
            clusters.Add(cluster);
        }

        return clusters;
    }

    private static List<T> CreateCluster<T>(int index, Context<T> context)
    {
        // Define a new cluster containing the point at index
        var cluster = new List<T> {context.Points[index]};
        int id = context.CurrentClusterId += 1;
        context.ClusterIds[index] = id;

        // Find all points connected to each point in the cluster
        // Cluster will be added to from within the loop
        for (int i = 0; i < cluster.Count; i++)
        {
            var p = cluster[i];

            // Check for connection with every other point
            for (int j = 0; j < context.Points.Count; j++)
            {
                // Skip if the point is already a member of another cluster
                if (context.ClusterIds[j] != 0)
                    continue;

                if (context.Space.Distance(p, context.Points[j]) <= context.Range)
                {
                    // Add point j to this cluster
                    cluster.Add(context.Points[j]);
                    context.ClusterIds[j] = id;
                }
            }
        }

        return cluster;
    }

    private ref struct Context<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context{T}"/> struct.
        /// </summary>
        public Context(IList<T> points, IMetricSpace<T> space, float range)
        {
            Points = points;
            Space = space;
            Range = range;
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
        /// Gets the maximum range that components are considered connected.
        /// </summary>
        public float Range { get; }

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
