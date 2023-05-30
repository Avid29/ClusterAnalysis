// Adam Dernis 2023
// Written at https://youtu.be/aMvMKCRSbgg?t=400

using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Shapes.Matrix;

/// <summary>
/// An implementation of <see cref="IMetricSpace{T}"/> that defines relations between <see cref="MatrixCell"/>s with a sparse matrix.
/// </summary>
public class SparseMatrixShape : IMetricSpace<MatrixCell>
{
    private readonly Dictionary<(MatrixCell, MatrixCell), float> _connections;

    /// <summary>
    /// Initializes a new instance of the <see cref="SparseMatrixShape"/> class.
    /// </summary>
    public SparseMatrixShape()
    {
        _connections = new Dictionary<(MatrixCell, MatrixCell), float>();
    }

    /// <inheritdoc/>
    public void AddConnection(MatrixCell node1, MatrixCell node2, float distance)
    {
        // Add connection bidirectionally so lookup works either way
        _connections.Add((node1, node2), distance);
        _connections.Add((node2, node1), distance);
    }
    
    /// <inheritdoc/>
    public float Distance(MatrixCell a, MatrixCell b)
        => _connections.ContainsKey((a, b)) ? _connections[(a, b)] : float.NaN;
}
