using UnityEngine;

public class Helpers
{
    public static T DeepCopy<T>(T obj)
    {
        // Convert to JSON, then back to an object
        string json = JsonUtility.ToJson(obj);
        return JsonUtility.FromJson<T>(json);
    }
}
