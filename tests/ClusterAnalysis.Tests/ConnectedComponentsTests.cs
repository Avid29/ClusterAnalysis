// Adam Dernis 2023

using ClusterAnalysis.Methods;
using ClusterAnalysis.Shapes.Graph;
using ClusterAnalysis.Shapes.Matrix;
using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Tests;

[TestClass]
public class ConnectedComponentsTests
{
    [TestMethod]
    public void GraphTest()
    {
        // Get points and space
        var shape = GeneratePointsAndSpace<Node, GraphShape>(out var points);

        // Cluster over the set of points
        var clusters = ConnectedComponents.Cluster(points, shape);

        // Assert results are correct
        AssertOutput(clusters, points);
    }

    [TestMethod]
    public void SparseMatrixTest()
    {
        // Get points and space
        var shape = GeneratePointsAndSpace<MatrixCell, SparseMatrixShape>(out var points);

        // Cluster over the set of points
        var clusters = ConnectedComponents.Cluster(points, shape);

        // Assert results are correct
        AssertOutput(clusters, points);
    }

    [TestMethod]
    public void MatrixTest()
    {
        // Matrix test requires the nodes have values and the space has a size,
        // therefore it cannot use GeneratePointsAndSpace

        // Creates nodes
        var a = new MatrixCell(0);
        var b = new MatrixCell(1);
        var c = new MatrixCell(2);
        var d = new MatrixCell(3);
        var points = new[] { a, b, c, d};

        // Create sparse matrix
        var shape = new MatrixShape(4);

        // Connect a to d and d to c
        shape.AddConnection(a, d, 1);
        shape.AddConnection(d, c, 1);

        // Cluster over the set of points
        var clusters = ConnectedComponents.Cluster(points, shape);
        
        // Assert results are correct
        AssertOutput(clusters, points);
    }
    
    private static TShape GeneratePointsAndSpace<T, TShape>(out T[] points)
        where T : new()
        where TShape : IMetricSpace<T>, new()
    {
        // Create points
        var a = new T();
        var b = new T();
        var c = new T();
        var d = new T();
        points = new [] { a, b, c, d };

        // Create space
        var shape = new TShape();

        // Connect a to d and d to c
        shape.AddConnection(a, d, 1);
        shape.AddConnection(d, c, 1);

        return shape;
    }

    private static void AssertOutput<T>(List<List<T>> clusters, T[] points)
    {
        // Get point references
        var a = points[0];
        var b = points[1];
        var c = points[2];
        var d = points[3];

        // 1st cluster is only a, d, and c 
        var c1 = clusters[0];
        Assert.AreEqual(3, c1.Count);
        Assert.IsTrue(c1.Contains(a));
        Assert.IsTrue(c1.Contains(d));
        Assert.IsTrue(c1.Contains(c));

        // 2nd cluster is only b
        var c2 = clusters[1];
        Assert.AreEqual(1, c2.Count);
        Assert.IsTrue(c2.Contains(b));
    }
}
