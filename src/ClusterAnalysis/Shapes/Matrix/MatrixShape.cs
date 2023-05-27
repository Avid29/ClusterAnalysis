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
    private readonly float[,] _connections;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixShape"/> class.
    /// </summary>
    public MatrixShape(int size)
    {
        _connections = new float[size, size];

        // Initialize all cells as NaN
        for (int i = 0; i < _connections.GetLength(0); i++)
          for (int j = 0; j < _connections.GetLength(1); j++)
            _connections[i, j] = float.NaN;
    }

    /// <inheritdoc/>
    public void AddConnection(MatrixCell node1, MatrixCell node2, float distance)
    {
        // Ensure the nodes are within the matrix size
        var max = _connections.GetLength(0);
        Guard.IsLessThan(node1.Value, max);
        Guard.IsLessThan(node2.Value, max);

        // Add connection bidirectionally so lookup works either way
        _connections[node1.Value, node2.Value] = distance;
        _connections[node2.Value, node1.Value] = distance;
    }
    
    /// <inheritdoc/>
    public float Distance(MatrixCell a, MatrixCell b)
        => _connections[a.Value, b.Value];
}
