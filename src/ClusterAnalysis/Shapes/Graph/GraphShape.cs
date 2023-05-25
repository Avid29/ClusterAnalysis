// Adam Dernis 2023

using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Shapes.Graph;

/// <summary>
/// An <see cref="IMetricSpace{T}"/> implementation that uses a node graph to store connections.
/// </summary>
public class GraphShape : IMetricSpace<Node>
{
    /// <inheritdoc/>
    public void AddConnection(Node node1, Node node2)
    {
        // Add connection bidirectionally so lookup works either way
        node1.AddConnection(node2);
        node2.AddConnection(node1);
    }
    
    /// <inheritdoc/>
    public bool Distance(Node a, Node b)
        => a.IsConnected(b);
}
