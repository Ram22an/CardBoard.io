using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FillAlgorithm : MonoBehaviour
{
    [SerializeField] private TrailRenderer PlayerTail;
    private List<Vector2> trailPoints = new List<Vector2>();

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovement.Instance.IsOutSide = false;
            PlayerTail.enabled = false;
            trailPoints = InfiniteTrailHandler.instance.GetPositions();
            CaptureTerritory();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovement.Instance.IsOutSide = true;
            PlayerTail.enabled = true;
        }
    }

    private void CaptureTerritory()
    {
        if (trailPoints.Count < 3) return; // Convex hull needs at least 3 points

        List<Vector2> hull = GrahamScan(trailPoints);
        FillCapturedArea(hull);
    }

    private List<Vector2> GrahamScan(List<Vector2> points)
    {
        points.Sort((a, b) => a.y == b.y ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));
        Stack<Vector2> hull = new Stack<Vector2>();

        foreach (var point in points)
        {
            while (hull.Count >= 2 && CrossProduct(hull.ToArray()[hull.Count - 2], hull.Peek(), point) <= 0)
            {
                hull.Pop();
            }
            hull.Push(point);
        }

        int lowerCount = hull.Count;
        for (int i = points.Count - 2; i >= 0; i--)
        {
            while (hull.Count > lowerCount && CrossProduct(hull.ToArray()[hull.Count - 2], hull.Peek(), points[i]) <= 0)
            {
                hull.Pop();
            }
            hull.Push(points[i]);
        }

        hull.Pop(); // Remove duplicate last point
        return new List<Vector2>(hull);
    }

    private float CrossProduct(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }

    /// <summary>
    /// Creates an extruded mesh from the convex hull with a thickness of 0.25 units along the y-axis.
    /// </summary>
    /// <param name="hull">The list of 2D points forming the convex hull.</param>
    private void FillCapturedArea(List<Vector2> hull)
    {
        // Set the extrusion height to 0.25 units.
        float height = 0.25f;
        int n = hull.Count;

        Mesh mesh = new Mesh();

        // Create vertices: for each hull point, create a bottom vertex (y = 0) and a top vertex (y = height)
        Vector3[] vertices = new Vector3[n * 2];
        for (int i = 0; i < n; i++)
        {
            // Bottom face vertex at y = 0
            vertices[i] = new Vector3(hull[i].x, 0, hull[i].y);
            // Top face vertex at y = height
            vertices[i + n] = new Vector3(hull[i].x, height, hull[i].y);
        }

        // Calculate triangles:
        // - Bottom face: (n-2) triangles (using a triangle fan)
        // - Top face: (n-2) triangles (using a triangle fan)
        // - Sides: n quads, each built from 2 triangles (n * 2 triangles)
        // Total triangles = (n-2) + (n-2) + (2*n) = 4n - 4.
        // Each triangle uses 3 indices, so the triangles array length is (4n - 4) * 3.
        int[] triangles = new int[(4 * n - 4) * 3];
        int t = 0; // triangle index pointer

        // --- Build the Bottom Face ---
        // Use a triangle fan; here we choose clockwise winding so that the normal points downward.
        // Note: Adjust the order if needed based on your rendering settings.
        for (int i = 1; i < n - 1; i++)
        {
            triangles[t++] = 0;
            triangles[t++] = i + 1;
            triangles[t++] = i;
        }

        // --- Build the Top Face ---
        // Use a triangle fan with counter-clockwise winding so that normals point upward.
        int topOffset = n;
        for (int i = 1; i < n - 1; i++)
        {
            triangles[t++] = topOffset;
            triangles[t++] = topOffset + i;
            triangles[t++] = topOffset + i + 1;
        }

        // --- Build the Side Faces ---
        // For each edge of the hull, create a quad (composed of two triangles) to connect the top and bottom faces.
        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;

            // First triangle of the side quad
            triangles[t++] = i;            // bottom current
            triangles[t++] = next;         // bottom next
            triangles[t++] = i + n;        // top current

            // Second triangle of the side quad
            triangles[t++] = next;         // bottom next
            triangles[t++] = next + n;     // top next
            triangles[t++] = i + n;        // top current
        }

        // Assign vertices and triangles to the mesh.
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Create a new GameObject to display the extruded mesh.
        GameObject fillArea = new GameObject("CapturedArea", typeof(MeshFilter), typeof(MeshRenderer));
        fillArea.GetComponent<MeshFilter>().mesh = mesh;
        fillArea.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
    }
}//class


//previous code
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//public class FillAlgorithm : MonoBehaviour
//{
//    [SerializeField] private TrailRenderer PlayerTail;
//    private List<Vector2> trailPoints = new List<Vector2>();
//    public void OnTriggerEnter(Collider other)
//    {
//        if (other.tag == "Player")
//        {
//            PlayerMovement.Instance.IsOutSide = false;
//            PlayerTail.enabled = false;
//            trailPoints = InfiniteTrailHandler.instance.GetPositions();
//            CaptureTerritory();
//        }
//    }
//    public void OnTriggerExit(Collider other)
//    {
//        if (other.tag == "Player")
//        {
//            PlayerMovement.Instance.IsOutSide = true;
//            PlayerTail.enabled = true;
//        }
//    }
//    private void CaptureTerritory()
//    {
//        if (trailPoints.Count < 3) return; // Convex hull needs at least 3 points

//        List<Vector2> hull = GrahamScan(trailPoints);
//        FillCapturedArea(hull);
//    }

//    private List<Vector2> GrahamScan(List<Vector2> points)
//    {
//        points.Sort((a, b) => a.y == b.y ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));
//        Stack<Vector2> hull = new Stack<Vector2>();

//        foreach (var point in points)
//        {
//            while (hull.Count >= 2 && CrossProduct(hull.ToArray()[hull.Count - 2], hull.Peek(), point) <= 0)
//            {
//                hull.Pop();
//            }
//            hull.Push(point);
//        }

//        int lowerCount = hull.Count;
//        for (int i = points.Count - 2; i >= 0; i--)
//        {
//            while (hull.Count > lowerCount && CrossProduct(hull.ToArray()[hull.Count - 2], hull.Peek(), points[i]) <= 0)
//            {
//                hull.Pop();
//            }
//            hull.Push(points[i]);
//        }

//        hull.Pop(); // Remove duplicate last point
//        return new List<Vector2>(hull);
//    }

//    private float CrossProduct(Vector2 a, Vector2 b, Vector2 c)
//    {
//        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
//    }

//    private void FillCapturedArea(List<Vector2> hull)
//    {
//        Mesh mesh = new Mesh();
//        Vector3[] vertices = new Vector3[hull.Count];
//        int[] triangles = new int[(hull.Count - 2) * 3];

//        for (int i = 0; i < hull.Count; i++)
//        {
//            vertices[i] = new Vector3(hull[i].x, 0, hull[i].y);
//            //vertices[i] = new Vector3(hull[i].x, 0, hull[i].y);
//        }

//        for (int i = 0; i < hull.Count - 2; i++)
//        {
//            //triangles[i * 3] = 0;
//            //triangles[i * 3 + 1] = i + 1;
//            //triangles[i * 3 + 2] = i + 2;
//            triangles[i * 3] = 0;
//            triangles[i * 3 + 1] = i + 2;
//            triangles[i * 3 + 2] = i + 1;
//        }

//        mesh.vertices = vertices;
//        mesh.triangles = triangles;
//        mesh.RecalculateNormals();

//        GameObject fillArea = new GameObject("CapturedArea", typeof(MeshFilter), typeof(MeshRenderer));
//        fillArea.GetComponent<MeshFilter>().mesh = mesh;
//        fillArea.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
//    }

//}//class