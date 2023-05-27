// Adam Dernis 2023
// Written at https://youtu.be/aMvMKCRSbgg?t=400

using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Shapes.Matrix;

/// <summary>
/// An implementation of <see cref="IMetricSpace{T}"/> that defines relations between <see cref="MatrixCell"/>s with a sparse matrix.
/// </summary>
public class SparseMatrixShape : IMetricSpace<MatrixCell>
{
    private readonly HashSet<(MatrixCell, MatrixCell)> _connections;

    /// <summary>
    /// Initializes a new instance of the <see cref="SparseMatrixShape"/> class.
    /// </summary>
    public SparseMatrixShape()
    {
        _connections = new HashSet<(MatrixCell, MatrixCell)>();
    }

    /// <inheritdoc/>
    public void AddConnection(MatrixCell node1, MatrixCell node2)
    {
        // Add connection bidirectionally so lookup works either way
        _connections.Add((node1, node2));
        _connections.Add((node2, node1));
    }
    
    /// <inheritdoc/>
    public bool Distance(MatrixCell a, MatrixCell b)
        => _connections.Contains((a, b));
}
