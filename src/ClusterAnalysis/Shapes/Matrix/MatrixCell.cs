// Adam Dernis 2023
// Written at https://youtu.be/aMvMKCRSbgg?t=370

namespace ClusterAnalysis.Shapes.Matrix;

/// <summary>
/// A metric space point for the <see cref="MatrixShape"/>.
/// </summary>
public class MatrixCell
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixCell"/> class.
    /// </summary>
    public MatrixCell()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixCell"/> class.
    /// </summary>
    public MatrixCell(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value of the node for a matrix index.
    /// </summary>
    public int Value { get; }
}
