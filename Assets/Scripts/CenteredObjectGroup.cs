using UnityEngine;

[ExecuteAlways]
public class CenteredObjectGroup : MonoBehaviour
{
    [Header("Arrangement")]
    [Tooltip("If true, the objects will be arranged in a vertical line; if false, a horizontal line.")]
    public bool vertical = false;

    [Tooltip("Spacing between each child object.")]
    public float spacing = 1f;

    [Header("Offset")]
    [Tooltip("Additional offset applied to the final positions.")]
    public Vector3 offset = Vector3.zero;

    /// <summary>
    /// Called whenever something changes in the Inspector.
    /// </summary>
    private void OnValidate()
    {
        LayoutChildren();
    }

    /// <summary>
    /// Called when a child is added or removed, ensuring the layout updates dynamically.
    /// </summary>
    private void OnTransformChildrenChanged()
    {
        LayoutChildren();
    }

    /// <summary>
    /// Arranges the children so that they are centered around this
    /// object's local origin, either horizontally or vertically.
    /// </summary>
    public void LayoutChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        // Calculate the total span of the children for spacing
        // For n children, total span = (n - 1) * spacing
        float totalSpan = (childCount - 1) * spacing;

        // Place each child so that the group is centered at local (0,0,0)
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // If vertical == false, arrange horizontally (X changes, Y=0)
            // Otherwise, arrange vertically (Y changes, X=0).
            if (!vertical)
            {
                // Horizontal arrangement
                float xPos = i * spacing - (totalSpan / 2f);
                child.localPosition = new Vector3(xPos, 0f, 0f) + offset;
            }
            else
            {
                // Vertical arrangement
                float yPos = -(i * spacing - (totalSpan / 2f));
                // Note: negative sign to move objects "down" if that’s your preference
                // Remove '-' if you want upward movement
                child.localPosition = new Vector3(0f, yPos, 0f) + offset;
            }
        }
    }

    /// <summary>
    /// Optional gizmo drawing to show spacing preview in the Scene view. 
    /// (You can remove this if you don't need a visual.)
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        int childCount = transform.childCount;
        float totalSpan = (Mathf.Max(1, childCount) - 1) * spacing;

        // If there are no children, draw two 'dummy' positions 
        // so you can see roughly what spacing looks like.
        int gizmoCount = (childCount == 0) ? 2 : childCount;

        for (int i = 0; i < gizmoCount; i++)
        {
            float localX = 0f;
            float localY = 0f;

            if (!vertical)
            {
                // Horizontal
                localX = i * spacing - (totalSpan / 2f);
            }
            else
            {
                // Vertical
                localY = -(i * spacing - (totalSpan / 2f));
            }

            Vector3 gizmoPos = transform.position + offset + new Vector3(localX, localY, 0f);
            Gizmos.DrawSphere(gizmoPos, 0.1f);
        }
    }
}
