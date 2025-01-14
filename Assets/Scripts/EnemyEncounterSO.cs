using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyEncounterSO", menuName = "Scriptable Objects/EnemyEncounterSO")]
public class EnemyEncounterSO : ScriptableObject
{
    public static float minDesiredSeparation = 3f;
    public static float maxDesiredSeparation = 7f;

    [System.Serializable]
    public class EnemyInfo
    {
        public EnemySO enemy;
        public float startingHP=10f;
        public int quantity = 1;
    }

    public List<EnemyInfo> enemyInfos;

    public List<Actor> CreateEncounterEnemies(Vector2 centerPoint, Vector2 playfieldSize)
    {
        List<Actor> enemies = new List<Actor>();
        List<Vector2> placedPositions = new List<Vector2>();

        // Calculate playfield boundaries
        float minX = centerPoint.x - playfieldSize.x / 2f;
        float maxX = centerPoint.x + playfieldSize.x / 2f;
        float minY = centerPoint.y - playfieldSize.y / 2f;
        float maxY = centerPoint.y + playfieldSize.y / 2f;

        foreach (var ei in enemyInfos)
        {
            for (int q = 0; q < ei.quantity; q++)
            {
                // Create enemy instance
                Actor enemy = ei.enemy.CreateEnemyFromSO();

                Vector2 chosenPosition = Vector2.zero;
                int attempts = 0;
                const int maxAttempts = 20;
                bool validPositionFound = false;

                while (attempts < maxAttempts)
                {
                    // Generate a random position within the playfield
                    float randomX = Random.Range(minX, maxX);
                    float randomY = Random.Range(minY, maxY);
                    Vector2 candidatePosition = new Vector2(randomX, randomY);

                    // Check if candidate position meets the minimum separation requirement
                    bool isValid = true;
                    foreach (var pos in placedPositions)
                    {
                        if (Vector2.Distance(candidatePosition, pos) < minDesiredSeparation)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        chosenPosition = candidatePosition;
                        validPositionFound = true;
                        break;
                    }

                    attempts++;
                }

                // If no valid position found within attempts, fallback to a random position
                if (!validPositionFound)
                {
                    chosenPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                }

                // Set enemy position and health
                enemy.transform.position = chosenPosition;
                enemy.SetHealthAndMaxHealth(ei.startingHP);

                // Add enemy to list and record its position for future separation checks
                enemies.Add(enemy);
                placedPositions.Add(chosenPosition);
            }
        }

        return enemies;
    }
}
