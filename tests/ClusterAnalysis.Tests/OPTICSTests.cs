// Adam Dernis 2023

using ClusterAnalysis.Methods;
using ClusterAnalysis.Shapes.Graph;
using ClusterAnalysis.Spaces;

namespace ClusterAnalysis.Tests;

[TestClass]
public class OPTICSTests
{
    [TestMethod]
    public void MaxRange7()
    {
        // Get points and shape
        var space = GeneratePointsAndSpace<Node, GraphShape>(out var points);

        // Cluster over the set of points
        var clusters = OPTICS.Cluster(points, space, minPoints: 3, range: 7);

        // Only one cluster
        Assert.AreEqual(1, clusters.Count);

        // Get point references
        var a = points[0];
        var b = points[1];
        var c = points[2];
        var d = points[3];
        var e = points[4];
        
        // Cluster contains A, B, C, and D
        var c1 = clusters[0];
        Assert.AreEqual(4, c1.Count);
        Assert.IsTrue(c1.Contains(a));
        Assert.IsTrue(c1.Contains(b));
        Assert.IsTrue(c1.Contains(c));
        Assert.IsTrue(c1.Contains(d));
        Assert.IsFalse(c1.Contains(e));
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
        var e = new T();
        points = new[] { a, b, c, d, e};

        // Create space
        var space = new TShape();

        // Connect a to b, c, and d
        space.AddConnection(a, b, 6);
        space.AddConnection(a, c, 4);
        space.AddConnection(a, d, 4);
        space.AddConnection(a, e, 20);

        return space;
    }
}
