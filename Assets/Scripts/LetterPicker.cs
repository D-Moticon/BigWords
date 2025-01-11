using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LetterData
{
    public string letter;
    public float frequency;
    public int score;

    public char letterChar => string.IsNullOrEmpty(letter) ? '\0' : letter[0];
}

[System.Serializable]
public class AlphabetData
{
    public List<LetterData> letters;
}

public class LetterPicker : MonoBehaviour
{
    public AlphabetData alphabetData;

    void Awake()
    {
        // Load the JSON file from Resources
        TextAsset jsonFile = Resources.Load<TextAsset>("alphabet"); // Make sure the file is named "alphabet.json" in a Resources folder
        if (jsonFile != null)
        {
            alphabetData = JsonUtility.FromJson<AlphabetData>(jsonFile.text);
        }
        else
        {
            Debug.LogError("Could not load alphabet.json from Resources!");
        }
    }

    public LetterData GetRandomLetter()
    {
        if (alphabetData == null || alphabetData.letters == null || alphabetData.letters.Count == 0)
        {
            Debug.LogError("Alphabet data is not loaded.");
            return null;
        }

        // Calculate total weight
        float totalWeight = 0f;
        foreach (var ld in alphabetData.letters)
        {
            totalWeight += ld.frequency;
        }

        // Pick a random value between 0 and totalWeight
        float randomValue = Random.Range(0f, totalWeight);
        float sum = 0f;

        // Loop through letters and find where the random value falls
        foreach (var ld in alphabetData.letters)
        {
            sum += ld.frequency;
            if (randomValue <= sum)
            {
                return ld;
            }
        }

        // Fallback (should not typically happen)
        return alphabetData.letters[alphabetData.letters.Count - 1];
    }
}
