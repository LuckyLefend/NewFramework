using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReduceVertexPolygonCollider2D
{
    //[MenuItem("GameObject/Reduce Vertex of PolygonCollider", true)]
    //static bool ValidateRemoveShapes()
    //{
    //    foreach (GameObject objects in Selection.gameObjects)
    //    {
    //        PolygonCollider2D collider = objects.GetComponent<PolygonCollider2D>();
    //        return (collider != null && collider.pathCount >= 1);
    //    }
    //    return false;
    //}

    //[MenuItem("GameObject/Reduce Vertex of PolygonCollider")]
    public static void RemoveShapes(GameObject gameObj)
    {
        var collider = gameObj.GetComponent<PolygonCollider2D>();
        if (collider == null)
        {
            return;
        }
        Undo.RecordObject(collider, "Remove Interior Shapes");

        var path1 = GetOutlinePath(collider);
        var path2 = RemovePlane(path1);
        var path3 = RemoveClosePolygon(path2);

        collider.SetPath(0, path3);
    }

    static Vector2[] GetOutlinePath(PolygonCollider2D collider)
    {
        int lExteriorShape = 0;
        float lLeftmostPoint = Mathf.Infinity;
        for (int i = 0, length = collider.pathCount; i < length; ++i)
        {
            Vector2[] lPath = collider.GetPath(i);
            foreach (Vector2 lPoint in lPath)
            {
                if (lPoint.x < lLeftmostPoint)
                {
                    lExteriorShape = i;
                    lLeftmostPoint = lPoint.x;
                }
            }
        }
        collider.pathCount = 1;
        return collider.GetPath(lExteriorShape);
    }

    static Vector2[] RemoveClosePolygon(Vector2[] lPath)
    {
        List<Vector2> verticesList = new List<Vector2>(lPath);
        foreach (var node in lPath)
        {
            if (verticesList.Exists(item => Vector3.Distance(node, item) < 0.3f && item != node))
            {
                verticesList.Remove(node);
                continue;
            }
        }
        return verticesList.ToArray();
    }

    static Vector2[] RemovePlane(Vector2[] lPath)
    {
        var verticesList = new List<Vector2>(lPath);
        for (int i = 1; i < lPath.Length - 1; i += 2)
        {
            var first = lPath[i - 1];
            var second = lPath[i];
            var third = lPath[i + 1];

            var angle1 = Vector2.Angle(first, second);
            var angle2 = Vector2.Angle(second, third);

            if (Mathf.Abs(angle1 - angle2) < 30)
            {
                verticesList.Remove(second);
                continue;
            }
        }
        return verticesList.ToArray();
    }
}
