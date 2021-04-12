using UnityEngine;

public class RectangleArea
{
    private readonly Vector3 halfAreaExtends;
    private readonly LayerMask layerMask;
    private readonly Collider[] buffer = new Collider[5];

    public RectangleArea(Vector3 halfAreaExtends, LayerMask layerMask)
    {
        this.halfAreaExtends = halfAreaExtends;
        this.layerMask = layerMask;
    }

    public bool CheckAreaIsFree(Vector3 areaCenterPos, GameObject exclude = null)
    {
        var colliders = Physics.OverlapBox(areaCenterPos, halfAreaExtends, Quaternion.identity, layerMask);
        var rectanglesCount = colliders.Length;

        if (exclude == null)
            return rectanglesCount == 0;
        
        foreach (var collider in colliders)
        {
            if (collider.gameObject == exclude)
            {
                rectanglesCount--;
                break;
            }
        }
        return rectanglesCount == 0;
    }
}