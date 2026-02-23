using UnityEngine;
using TMPro;

public class MemoryAcquisitionPopup : MonoBehaviour
{
    public static MemoryAcquisitionPopup instance;
    public TextMeshProUGUI messageText;
    public GameObject visualParent;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowMemoryMessage(string ghostName)
    {
        if (visualParent == null || messageText == null) return;

        visualParent.SetActive(true);

        messageText.text = $"Acquired: {ghostName}'s Memory Fragment";

        CancelInvoke();
        Invoke(nameof(HidePopup), 1.5f);
    }

    void HidePopup()
    {
        if (visualParent != null)
            visualParent.SetActive(false);
    }
}