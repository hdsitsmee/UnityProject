using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DrinkRecipeBook", menuName = "Data_Cafe/DrinkRecipeBook")]
public class DrinkRecipeBook : ScriptableObject
{
    public List<DrinkData> allRecipes;
}
