using UnityEngine;
using UnityEngine.UI;

public class PatienceHUD : MonoBehaviour
{
    public Slider patienceSlider;

    void Update()
    {
        if (patienceSlider == null || GameManager.instance == null) return;

        bool active = GameManager.instance.orderActive;
        if (patienceSlider.gameObject.activeSelf != active)
            patienceSlider.gameObject.SetActive(active);

        if (active)
            patienceSlider.value = GameManager.instance.GetPatienceNormalized();

    }
}
