using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityBorder : MonoBehaviour
{
    public static List<Vector3> CalculateEdges(SpriteRenderer spriteRenderer)
    {
        Bounds spriteBounders = spriteRenderer.bounds;
        Vector3 center = spriteBounders.center;
        Vector3 extents = spriteBounders.extents;
        float horizontalSize = 0.5f;
        float verticalSize = 0.4329883f;
        List<Vector3> edges = new();

        var rightEdge = new Vector3(center.x + horizontalSize, center.y);
        var bottomRightEdge = new Vector3(center.x + (horizontalSize / 2), center.y - verticalSize);
        var bottomLeftEdge = new Vector3(center.x - (horizontalSize / 2), center.y - verticalSize);
        var leftEdge = new Vector3(center.x - horizontalSize, center.y);
        var topLeftEdge = new Vector3(center.x - (horizontalSize / 2), center.y + verticalSize);
        var topRightEdge = new Vector3(center.x + (horizontalSize / 2), center.y + verticalSize);

        edges.Add(rightEdge);
        edges.Add(bottomRightEdge);
        edges.Add(bottomLeftEdge);
        edges.Add(leftEdge);
        edges.Add(topLeftEdge);
        edges.Add(topRightEdge);

        return edges;
    }

    public static void CreateCityBorder(List<Vector3> edges, LineRenderer territoryLineRenderer)
    {
        var firstEdge = edges.FirstOrDefault();
        List<Vector3> edgesNear = new()
        {
            firstEdge
        };

        List<Vector3> availableEdges = new(edges);
        availableEdges.RemoveAt(0);

        foreach(var edge in edges)
        {
            var actualEdge = edgesNear.LastOrDefault();
            var edgesSorted = edges.OrderBy(edge => Vector3.Distance(actualEdge, edge)).ToList();

            foreach(var edgeSorted in edgesSorted)
            {
                if (availableEdges.Contains(edgeSorted))
                {
                    edgesNear.Add(edgeSorted);
                    availableEdges.Remove(edgeSorted);
                    break;
                }
            }
        }

        edgesNear.Add(firstEdge);

        territoryLineRenderer.startWidth = 0.05f;
        territoryLineRenderer.endWidth = 0.05f;
        territoryLineRenderer.positionCount = edgesNear.Count;
        territoryLineRenderer.SetPositions(edgesNear.ToArray());
    }
}
