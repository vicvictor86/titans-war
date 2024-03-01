using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CityBorder : MonoBehaviour
{
    
    public static void CreateCityBorder(SpriteRenderer spriteRenderer, LineRenderer territoryLineRenderer)
    {
        Bounds spriteBounders = spriteRenderer.bounds;
        Vector3 center = spriteBounders.center;
        Vector3 extents = spriteBounders.extents;
        List<Vector3> edges = new();

        var rightEdge = new Vector3(center.x + extents.x, center.y);
        var bottomRightEdge = new Vector3(center.x + (extents.x / 2), center.y - (extents.y - 0.25f));
        var bottomLeftEdge = new Vector3(center.x - (extents.x / 2), center.y - (extents.y - 0.25f));
        var leftEdge = new Vector3(center.x - extents.x, center.y);
        var topLeftEdge = new Vector3(center.x - (extents.x / 2), center.y + (extents.y - 0.25f));
        var topRightEdge = new Vector3(center.x + (extents.x / 2), center.y + (extents.y - 0.25f));

        edges.Add(rightEdge);
        edges.Add(bottomRightEdge);
        edges.Add(bottomLeftEdge);
        edges.Add(leftEdge);
        edges.Add(topLeftEdge);
        edges.Add(topRightEdge);
        edges.Add(rightEdge);

        territoryLineRenderer.startWidth = 0.05f;
        territoryLineRenderer.endWidth = 0.05f;
        territoryLineRenderer.positionCount = edges.Count;
        territoryLineRenderer.SetPositions(edges.ToArray());

        //Debug.Log($"Sprite Center: {center}");
        //Debug.Log($"Sprite Extents: {extents}");
        //Debug.Log($"Canto Direito - x: {center.x + extents.x}, y: {center.y}");
        //Debug.Log($"Canto Inferior Direito - x: {center.x + (extents.x / 2)}, y: {center.y - (extents.y - 0.25)}");
        //Debug.Log($"Canto Inferior Esquerdo - x: {center.x - (extents.x / 2)}, y: {center.y - (extents.y - 0.25)}");
        //Debug.Log($"Canto Esquerdo - x: {center.x - extents.x}, y: {center.y}");
        //Debug.Log($"Canto Superior Esquerdo - x: {center.x - (extents.x / 2)}, y: {center.y + (extents.y - 0.25)}");
        //Debug.Log($"Canto Superior Direito - x: {center.x + (extents.x / 2)}, y: {center.y + (extents.y - 0.25)}");
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
