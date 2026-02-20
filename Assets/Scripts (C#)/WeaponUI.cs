using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    Text levelText;
    Slider mySlider;

    private void Awake()
    {
        levelText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();


    }
    private void LateUpdate()
    {
       
    }
}
