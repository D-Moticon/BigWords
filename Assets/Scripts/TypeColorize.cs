using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TypeColorize : MonoBehaviour
{
    public void ColorizeHue(Color targetColor)
    {
        
        // Extract the hue from the target color
        Color.RGBToHSV(targetColor, out float targetHue, out _, out _);

        // Check if this GameObject has a SpriteRenderer
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            // Get current color
            Color currentColor = spriteRenderer.color;
            // Extract original HSV
            Color.RGBToHSV(currentColor, out _, out float originalSaturation, out float originalValue);

            // Create a new color with the target hue and the original saturation/value
            Color newColor = Color.HSVToRGB(targetHue, originalSaturation, originalValue);

            // Assign the new color
            spriteRenderer.color = newColor;
        }
        // Otherwise check if this GameObject has a TextMeshPro
        else if (TryGetComponent<TMP_Text>(out var textMeshPro))
        {
            // Get current color
            Color currentColor = textMeshPro.color;
            // Extract original HSV
            Color.RGBToHSV(currentColor, out _, out float originalSaturation, out float originalValue);

            // Create a new color with the target hue and the original saturation/value
            Color newColor = Color.HSVToRGB(targetHue, originalSaturation, originalValue);

            // Assign the new color
            textMeshPro.color = newColor;
        }

        // Otherwise check if this GameObject has a TextMeshPro
        else if (TryGetComponent<Image>(out var img))
        {
            // Get current color
            Color currentColor = img.color;
            // Extract original HSV
            Color.RGBToHSV(currentColor, out _, out float originalSaturation, out float originalValue);

            // Create a new color with the target hue and the original saturation/value
            Color newColor = Color.HSVToRGB(targetHue, originalSaturation, originalValue);

            // Assign the new color
            img.color = newColor;
        }

        // Otherwise, you might want to handle the case of no supported component found
        else
        {
            Debug.LogWarning("No SpriteRenderer or TextMeshPro component found on this GameObject.");
        }
        
    }
}
