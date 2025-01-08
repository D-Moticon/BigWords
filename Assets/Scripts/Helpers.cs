using UnityEngine;
using System.Collections.Generic;

public static class Helpers
{
    public static T DeepCopy<T>(T obj)
    {
        // Convert to JSON, then back to an object
        string json = JsonUtility.ToJson(obj);
        return JsonUtility.FromJson<T>(json);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Shuffle<T>(T[] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public static int GetRandomWeightedIndex(float[] weights)
    {
        if (weights == null || weights.Length == 0)
        {
            return -1;
        }

        float w = 0.0f;
        float total = 0.0f;

        for (int i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsPositiveInfinity(w))
            {
                return i;
            }

            else if (w >= 0f && !float.IsNaN(w))
            {
                total += weights[i];
            }

        }

        float r = Random.value;
        float s = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / total;
            if (s >= r) return i;
        }

        return -1;
    }


    public static float RemapClamped(this float aValue, float aIn1, float aIn2, float aOut1, float aOut2)
    {
        float t = (aValue - aIn1) / (aIn2 - aIn1);
        if (t > 1f)
            return aOut2;
        if (t < 0f)
            return aOut1;
        return aOut1 + (aOut2 - aOut1) * t;
    }

    public static void SetSpriteSize(SpriteRenderer spriteRenderer, float desiredWidth, float desiredHeight)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogWarning("SpriteRenderer or Sprite is null. Cannot resize.");
            return;
        }

        // Get the current sprite bounds (in world units) before scaling
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // Calculate the scale we need based on desired size
        float scaleX = desiredWidth / spriteSize.x;
        float scaleY = desiredHeight / spriteSize.y;

        // Apply the new scale to the SpriteRenderer's transform
        spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
