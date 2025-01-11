using UnityEngine;

[ExecuteAlways]
public class ObjectLayoutGroup : MonoBehaviour
{
    [Header("Grid Settings")]
    public float xSpacing = 2f;     // Horizontal distance between objects
    public float ySpacing = 2f;     // Vertical distance between objects
    public int columns = 3;         // Number of columns in the grid

    [Header("Centering")]
    public bool centerOnParent = false;  // If true, center the entire grid around this transform

    [Header("Offset")]
    public Vector3 offset = Vector3.zero; // Additional offset from the parent’s center

    /// <summary>
    /// Whenever something changes in the inspector, re-layout the children.
    /// </summary>
    private void OnValidate()
    {
        LayoutChildren();
    }

    /// <summary>
    /// Called in the editor and at runtime whenever a child is added/removed.
    /// Ensures the layout updates dynamically.
    /// </summary>
    private void OnTransformChildrenChanged()
    {
        LayoutChildren();
    }

    /// <summary>
    /// Arrange child objects in a grid layout.
    /// </summary>
    public void LayoutChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        // Calculate how many rows we actually need
        int rowCount = Mathf.CeilToInt(childCount / (float)columns);

        // If we're centering, we need to figure out the total width/height of the grid
        // so we can shift everything so that it’s centered around (0,0,0).
        // For N columns, total width in X is (columns - 1) * xSpacing
        // For rowCount rows, total height in Y is (rowCount - 1) * ySpacing
        float totalWidth = (columns - 1) * xSpacing;
        float totalHeight = (rowCount - 1) * ySpacing;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // Determine row and column
            int row = i / columns;
            int col = i % columns;

            // Base local position
            float localX = col * xSpacing;
            float localY = -row * ySpacing; // negative if you want the grid to extend downward
            float localZ = 0f;

            // If centering, shift so that (0,0,0) is the center of the entire grid
            if (centerOnParent)
            {
                // Shift in x by half the total width
                localX -= totalWidth * 0.5f;
                // Shift in y by half the total height (but keep negative sign for row)
                localY += totalHeight * 0.5f;
            }

            // Assign the position + offset
            child.localPosition = new Vector3(localX, localY, localZ) + offset;
        }
    }

    /// <summary>
    /// Always draw two rows of spheres (for visualization) in the Scene view, 
    /// even if there are no children. This shows the spacing and centering.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // We want exactly two rows of spheres for visualization
        int totalSpheres = columns * 2; // 2 rows

        // For these "dummy" two rows, figure out the total width and total height
        float totalWidth = (columns - 1) * xSpacing;
        float totalHeight = (2 - 1) * ySpacing; // 2 rows => only 1 gap in vertical direction

        for (int i = 0; i < totalSpheres; i++)
        {
            int row = i / columns;
            int col = i % columns;

            // Base sphere position
            float sphereX = col * xSpacing;
            float sphereY = -row * ySpacing;
            float sphereZ = 0f;

            // If we're centering, shift so that transform.position is the grid’s center
            if (centerOnParent)
            {
                sphereX -= totalWidth * 0.5f;
                sphereY += totalHeight * 0.5f;
            }

            // Convert local coords to world coords by adding transform.position and offset
            Vector3 spherePos = transform.position
                                + offset
                                + new Vector3(sphereX, sphereY, sphereZ);

            Gizmos.DrawSphere(spherePos, 0.1f);
        }
    }
}
