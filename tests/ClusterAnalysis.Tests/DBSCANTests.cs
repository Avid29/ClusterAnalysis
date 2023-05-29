// Adam Dernis 2023

using ClusterAnalysis.Methods;
using ClusterAnalysis.Shapes.Graph;
using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Tests;

[TestClass]
public class DBSCANTests
{
    [TestMethod]
    public void MinPoints2()
    {
        // Get points and shape
        var space = GeneratePointsAndSpace<Node, GraphShape>(out var points);

        // Cluster over the set of points
        var clusters = DBSCAN.Cluster(points, space, range: 5, minPoints: 2);

        // Only one cluster
        Assert.AreEqual(1, clusters.Count);

        // Get point references
        var a = points[0];
        var c = points[2];
        var d = points[3];
        
        // Cluster contains A, C, and D
        var c1 = clusters[0];
        Assert.AreEqual(3, c1.Count);
        Assert.IsTrue(c1.Contains(a));
        Assert.IsTrue(c1.Contains(c));
        Assert.IsTrue(c1.Contains(d));
    }

    [TestMethod]
    public void MinPoints3()
    {
        // Get points and shape
        var space = GeneratePointsAndSpace<Node, GraphShape>(out var points);

        // Cluster over the set of points
        var clusters = DBSCAN.Cluster(points, space, range: 5, minPoints: 3);

        // There is not cluster
        Assert.AreEqual(0, clusters.Count);
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
        points = new[] { a, b, c, d };

        // Create space
        var space = new TShape();

        // Connect a to b, c, and d
        space.AddConnection(a, b, 6);
        space.AddConnection(a, c, 4);
        space.AddConnection(a, d, 4);

        return space;
    }
}
