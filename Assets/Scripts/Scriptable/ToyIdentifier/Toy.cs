using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/New Toy Algorithm/New Toy", order = 1)]
public class Toy : ScriptableObject
{
    public List<float> distances;
    public string toolName;
    public string toolIndex;
    public ToyIdentifier ToyIdentifier;

}
