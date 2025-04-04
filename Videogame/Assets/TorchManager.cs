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

    // This is the method you're trying to call
    public void TorchStateChanged(bool isLit)
    {
        litTorches += isLit ? 1 : -1;
        litTorches = Mathf.Clamp(litTorches, 0, totalTorches);
        UpdateUI();
        
        if (litTorches == totalTorches && totalTorches > 0)
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
            completionIndicator.color = (litTorches >= totalTorches && totalTorches > 0) 
                ? completeColor 
                : incompleteColor;
        }

        onTorchCountChanged?.Invoke(litTorches);
    }

    private void AllTorchesLit()
    {
        Debug.Log($"All {totalTorches} torches are lit!");
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
            TorchStateChanged(false);
        }
        UpdateUI();
    }

    // Helper properties
    public int LitTorches => litTorches;
    public int TotalTorches => totalTorches;
    public bool AllTorchesAreLit => totalTorches > 0 && litTorches >= totalTorches;
}