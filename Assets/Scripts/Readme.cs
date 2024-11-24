using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Readme", menuName = "Create Readme")]
public class Readme : ScriptableObject
{
    public string Title;
    public List<string> Sections = new List<string>();
}