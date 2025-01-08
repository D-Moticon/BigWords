using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance { get; private set; }

    public GameManager gameManager;
    public WordChecker wordChecker;
    public CardHandler tileHandler;
    public CardCreator cardCreator;
    public PrefabReferences prefabReferences;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        Application.targetFrameRate = 165;
    }

    private void Update()
    {
        
    }
}
