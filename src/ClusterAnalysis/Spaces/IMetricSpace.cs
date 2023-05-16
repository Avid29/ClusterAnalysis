﻿// Adam Dernis 2023

namespace ClusterAnalysis.Spaces;

/// <summary>
/// An interface for describing the metric space of a type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of a point in the space.</typeparam>
public interface IMetricSpace<in T>
{
    /// <summary>
    /// Adds a connection between two nodes.
    /// </summary>
    /// <param name="node1">The first node to connect.</param>
    /// <param name="node2">The second node to connect.</param>
    void AddConnection(T node1, T node2);

    /// <summary>
    /// Gets the distance between points <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Temporarily a bool in place of a float.
    /// </remarks>
    /// <param name="a">Point a.</param>
    /// <param name="b">Point b.</param>
    /// <returns>The distance between point <paramref name="a"/> and <paramref name="b"/>.</returns>
    bool Distance(T a, T b);
}