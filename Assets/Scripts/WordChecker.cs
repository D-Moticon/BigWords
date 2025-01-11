using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

/// <summary>
/// A single script that can load a JSON dictionary from Resources,
/// parse it, and check if a list of chars forms a valid word.
/// </summary>
public class WordChecker : MonoBehaviour
{
    // You could expose this in the Inspector if you want to configure it in Unity:
    [SerializeField]
    private string dictionaryResourcePath = "Dictionary/dictionary";

    // Internal set of valid words
    private HashSet<string> wordSet;

    /// <summary>
    /// Load and parse the dictionary as soon as this component is created.
    /// </summary>
    private void Awake()
    {
        LoadDictionary(dictionaryResourcePath);
    }

    /// <summary>
    /// Load the dictionary from a JSON file in the Resources folder 
    /// that looks like:
    /// {
    ///   "a": 1,
    ///   "aa": 1,
    ///   "aaa": 1,
    ///   "aah": 1,
    ///   "aahed": 1,
    ///   ...
    /// }
    /// </summary>
    private void LoadDictionary(string resourcePath)
    {
        TextAsset dictionaryJson = Resources.Load<TextAsset>(resourcePath);

        if (dictionaryJson == null)
        {
            Debug.LogError($"Could not find dictionary JSON file at: {resourcePath}");
            return;
        }

        // Deserialize into a Dictionary<string, int>
        // Keys = words, Values = 1
        var dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(dictionaryJson.text);

        // Convert keys to a HashSet for quick membership checks
        wordSet = new HashSet<string>(dict.Keys);

        Debug.Log($"Dictionary loaded. Total words: {wordSet.Count}");
    }

    /// <summary>
    /// Checks if the given list of chars forms a valid word.
    /// </summary>
    /// <param name="chars">The letters of the word.</param>
    /// <returns>True if the word is in the dictionary, false otherwise.</returns>
    public bool IsValidWord(string word)
    {
        if (wordSet == null || wordSet.Count == 0)
        {
            Debug.LogWarning("Word set not loaded or is empty!");
            return false;
        }

        // Check membership in the HashSet

        bool isValid = wordSet.Contains(word);

        //Debug.Log($"Is {word} valid? {isValid}");

        return wordSet.Contains(word);
    }

    public static bool CheckWord(string word)
    {
        return Singleton.Instance.wordChecker.IsValidWord(word);
    }
}
