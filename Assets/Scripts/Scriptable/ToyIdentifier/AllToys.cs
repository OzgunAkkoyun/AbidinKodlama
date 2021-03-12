using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/New Toy Algorithm/Toy List", order = 1)]
public class AllToys : ScriptableObject
{
    public List<Toy> toys;

    public string IdentifyTool(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        var touchList = new List<Vector2>(){p1,p2,p3};
        ToyIdentifier toyIdentifier = new ToyIdentifier(touchList);

        return toyIdentifier.FindZeroOneTriangle(toyIdentifier, toys);
    }
}
