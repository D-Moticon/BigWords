using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HoverUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMP_Text itemNameText;
    public Image itemImage;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemTypeText;
    public Image itemTypeFrameImage;
    IHoverable currentHoverable;
    public bool followMouse = false;
    public Vector2 followOffset;

    private RectTransform rectTransform;

    private void Awake()
    {
        // Cache the RectTransform for efficiency.
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Singleton.Instance.selectionHandler.currentHoverable == null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            currentHoverable = null;
            return;
        }

        if (Singleton.Instance.selectionHandler.currentHoverable != currentHoverable)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            currentHoverable = Singleton.Instance.selectionHandler.currentHoverable;
            itemNameText.text = currentHoverable.GetHoverableName();
            itemDescriptionText.text = currentHoverable.GetHoverableDescription();
            itemTypeText.text = currentHoverable.GetHoverableType();
            itemImage.sprite = currentHoverable.GetHoverableSprite();
            itemTypeFrameImage.color = currentHoverable.GetHoverableColor();
            followOffset = currentHoverable.GetHoverUIOffset();
        }

        if (followMouse)
        {
            UpdateMousePosition();
        }

        
    }

    void UpdateMousePosition()
    {
        // Get the current mouse position in both screen and world space
        Vector2 mousePosScreen = Input.mousePosition;
        Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);

        // Position the UI with the current follow offset
        transform.position = mousePosWorld + followOffset;

        // Check the world corners of the RectTransform
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        bool offTop = false;
        bool offBottom = false;

        // Convert each corner to screen space and check boundaries
        for (int i = 0; i < 4; i++)
        {
            Vector3 screenCorner = Camera.main.WorldToScreenPoint(corners[i]);
            if (screenCorner.y > Screen.height) offTop = true;
            if (screenCorner.y < 0) offBottom = true;
        }

        // If UI goes off the top and we are offsetting upwards, flip the y offset.
        if (offTop && followOffset.y > 0)
        {
            followOffset.y *= -1;
            transform.position = mousePosWorld + followOffset;
        }
        // If UI goes off the bottom and we are offsetting downwards, flip the y offset.
        else if (offBottom && followOffset.y < 0)
        {
            followOffset.y *= -1;
            transform.position = mousePosWorld + followOffset;
        }
    }
}
