using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MimicLibrary", menuName = "Mimics/Mimic Library")]
public class MimicLibrary : ScriptableObject
{
    public List<MimicData> mimics = new List<MimicData>();
} 