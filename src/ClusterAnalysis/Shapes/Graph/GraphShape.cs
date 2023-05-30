// Adam Dernis 2023
// Written at https://youtu.be/aMvMKCRSbgg?t=298

using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Shapes.Graph;

/// <summary>
/// An <see cref="IMetricSpace{T}"/> implementation that uses a node graph to store connections.
/// </summary>
public class GraphShape : IMetricSpace<Node>
{
    /// <inheritdoc/>
    public void AddConnection(Node node1, Node node2, float distance)
    {
        // Add connection bidirectionally so lookup works either way
        node1.AddConnection(node2, distance);
        node2.AddConnection(node1, distance);
    }
    
    /// <inheritdoc/>
    public float Distance(Node a, Node b)
        => a.Distance(b);
}
