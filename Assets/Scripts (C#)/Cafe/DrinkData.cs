using UnityEngine;

[CreateAssetMenu(fileName = "DrinkData", menuName = "Data_Cafe/DrinkData")]
public class DrinkData : ScriptableObject
{
    [Header("# Main Info")]
    public string drinkName;
    public string[] requiredIngredients;
    public Sprite drinkIcon;

    [Header("# Level Info")]
    public int unlockLevel;

    [Header("# Made")]
    public bool hasMade = false;
    
}
