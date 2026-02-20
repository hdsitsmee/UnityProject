using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Memory", menuName = "ScriptableObjects/MemoryData")]
public class MemoryData : ScriptableObject
{
    public string ghostName;      
    public int dropLevel;       
    public Sprite fragmentIcon;  

    [TextArea]
    public List<string> stories; 
}