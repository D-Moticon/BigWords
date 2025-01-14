using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance { get; private set; }

    public GameManager gameManager;
    public AudioManager audioManager;
    public WordChecker wordChecker;
    public SelectionHandler selectionHandler;
    public CardCreator cardCreator;
    public PrefabReferences prefabReferences;
    public UIManager uiManager;
    public LetterPicker letterPicker;
    public FeelPoolManager feelPoolManager;

    private void OnEnable()
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Break();
        }
    }
}
