﻿// Adam Dernis 2023
// Written at https://youtu.be/aMvMKCRSbgg?t=304
 
namespace ClusterAnalysis.Shapes.Graph;

/// <summary>
/// A point in the <see cref="GraphShape"/> space.
/// </summary>
public class Node
{
    private readonly Dictionary<Node, float> _connections;

    /// <summary>
    /// Initializes a new instance of the <see cref="Node"/> class.
    /// </summary>
    public Node()
    {
        _connections = new Dictionary<Node, float>();
    }

    /// <summary>
    /// Adds a connection to a node.
    /// </summary>
    /// <param name="node">The node to connect with,</param>
    /// <param name="distance">The distance from the node.</param>
    public void AddConnection(Node node, float distance)
        => _connections.Add(node, distance);

    /// <summary>
    /// Gets if a node is connected to this node.
    /// </summary>
    /// <param name="node">The node to check for a connection with.</param>
    /// <returns>True if the nodes are connected. False otherwise.</returns>
    public float Distance(Node node)
        => _connections.ContainsKey(node) ? _connections[node] : float.NaN;
}
