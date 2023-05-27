// Adam Dernis 2023
// Written at https://youtu.be/aMvMKCRSbgg?t=377

using ClusterAnalysis.Spaces;
using CommunityToolkit.Diagnostics;

namespace ClusterAnalysis.Shapes.Matrix;

/// <summary>
/// An implementation of <see cref="IMetricSpace{T}"/> that defines relations between <see cref="MatrixCell"/>s with a matrix.
/// </summary>
public class MatrixShape : IMetricSpace<MatrixCell>
{
    private readonly bool[,] _connections;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixShape"/> class.
    /// </summary>
    public MatrixShape(int size)
    {
        _connections = new bool[size, size];
    }

    /// <inheritdoc/>
    public void AddConnection(MatrixCell node1, MatrixCell node2)
    {
        // Ensure the nodes are within the matrix size
        var max = _connections.GetLength(0);
        Guard.IsLessThan(node1.Value, max);
        Guard.IsLessThan(node2.Value, max);

        // Add connection bidirectionally so lookup works either way
        _connections[node1.Value, node2.Value] = true;
        _connections[node2.Value, node1.Value] = true;
    }
    
    /// <inheritdoc/>
    public bool Distance(MatrixCell a, MatrixCell b)
        => _connections[a.Value, b.Value];
}
