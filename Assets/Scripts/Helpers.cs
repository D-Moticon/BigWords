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

    public static Vector2 RotateVector2(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static float Vector2ToAngle(Vector2 vector2)
    {
        float result;

        if (vector2.x < 0)
        {
            result = 360 - (Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg * -1);
        }
        else
        {
            result = Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
        }


        if (result > 360)
        {
            result -= 360;
        }

        return result;
    }

    public static Quaternion Vector2ToRotation(Vector2 vector2)
    {
        float angle = 0;

        if (vector2.x < 0)
        {
            angle = 360 - (Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg * -1);
        }
        else
        {
            angle = Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
        }

        return Quaternion.Euler(0, 0, angle);
    }

    public static Vector2 AngleDegToVector2(float angle)
    {
        float angleRad = Mathf.Deg2Rad * angle;
        float x = Mathf.Cos(angleRad);
        float y = Mathf.Sin(angleRad);
        Vector2 vec = new Vector2(x, y);
        return vec;
    }

    public static Quaternion AngleDegToRotation(float angle)
    {
        return Quaternion.Euler(0, 0, angle);
    }

    public static Vector2 RotationToVector2(Quaternion rotation)
    {
        Vector2 dir = Helpers.AngleDegToVector2(rotation.eulerAngles.z);
        return dir;
    }

    public static float RoundToDecimal(float value, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * multiplier) / multiplier;
    }

    public static List<T> GetRandomElements<T>(this List<T> source, int quantity, List<T> exclude = null)
    {
        // Make a copy so we don't modify the original list
        List<T> workingList = new List<T>(source);

        // If exclude is provided, remove those items from the working list
        if (exclude != null && exclude.Count > 0)
        {
            workingList.RemoveAll(item => exclude.Contains(item));
        }

        // Safety check: clamp quantity to the working list count
        if (quantity > workingList.Count)
        {
            quantity = workingList.Count;
        }

        // Partial Fisher–Yates shuffle (only up to 'quantity')
        for (int i = 0; i < quantity; i++)
        {
            int r = Random.Range(i, workingList.Count);
            // Swap
            T temp = workingList[i];
            workingList[i] = workingList[r];
            workingList[r] = temp;
        }

        // Return the first 'quantity' elements of the shuffled list
        return workingList.GetRange(0, quantity);
    }
}
