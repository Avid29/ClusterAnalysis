// Adam Dernis 2023

namespace ClusterAnalysis.Shapes.Graph;

/// <summary>
/// A point in the <see cref="GraphShape"/> space.
/// </summary>
public class Node
{
    private readonly HashSet<Node> _connections;

    /// <summary>
    /// Initializes a new instance of the <see cref="Node"/> class.
    /// </summary>
    public Node()
    {
        _connections = new HashSet<Node>();
    }

    /// <summary>
    /// Adds a connection to a node.
    /// </summary>
    /// <param name="node">The node to connect with,</param>
    public void AddConnection(Node node)
        => _connections.Add(node);

    /// <summary>
    /// Gets if a node is connected to this node.
    /// </summary>
    /// <param name="node">The node to check for a connection with.</param>
    /// <returns>True if the nodes are connected. False otherwise.</returns>
    public bool IsConnected(Node node)
        => _connections.Contains(node);
}
