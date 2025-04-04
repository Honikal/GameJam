using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TorchManager : MonoBehaviour
{
    public static TorchManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Text torchCounterText;
    [SerializeField] private Image completionIndicator;
    [SerializeField] private Color incompleteColor = Color.red;
    [SerializeField] private Color completeColor = Color.green;

    [Header("Events")]
    public UnityEvent onAllTorchesLit;
    public UnityEvent<int> onTorchCountChanged;

    private int totalTorches = 0;
    private int litTorches = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        if (completionIndicator != null)
        {
            completionIndicator.color = incompleteColor;
        }
    }

    public void TorchStateChanged(bool isLit)
    {
        litTorches += isLit ? 1 : -1;
        litTorches = Mathf.Clamp(litTorches, 0, totalTorches);
        UpdateUI();
        
        if (AllTorchesAreLit)
        {
            AllTorchesLit();
        }
    }

    private void UpdateUI()
    {
        if (torchCounterText != null)
        {
            torchCounterText.text = $"{litTorches}/{totalTorches}";
        }

        if (completionIndicator != null)
        {
            completionIndicator.color = AllTorchesAreLit ? completeColor : incompleteColor;
        }

        onTorchCountChanged?.Invoke(litTorches);
    }

    private void AllTorchesLit()
    {
        Debug.Log("YOU WIN! All torches are lit!");
        onAllTorchesLit?.Invoke();
    }

    public void RegisterTorch()
    {
        totalTorches++;
        UpdateUI();
    }

    public void UnregisterTorch(bool wasLit)
    {
        totalTorches = Mathf.Max(0, totalTorches - 1);
        if (wasLit)
        {
            litTorches--;
        }
        UpdateUI();
    }

    // Helper properties
    public int LitTorches => litTorches;
    public int TotalTorches => totalTorches;
    public bool AllTorchesAreLit => totalTorches > 0 && litTorches >= totalTorches;
}