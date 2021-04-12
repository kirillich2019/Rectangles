using UnityEngine;

public class RectangleFactory : MonoBehaviour
{
    [SerializeField] private Rectangle rectangle;
    [SerializeField] private LayerMask rectanglesLayer;
    private Vector3 halfRectangleScale;
    private RectangleArea rectangleArea;

    private void Awake()
    {
        halfRectangleScale = rectangle.transform.localScale / 2;
        rectangleArea = new RectangleArea(halfRectangleScale, rectanglesLayer);
    }

    public Rectangle TrySpawnNewRectangle(Vector3 spawnPos)
    {
        var areaIsFree = rectangleArea.CheckAreaIsFree(spawnPos);
        
        if(!areaIsFree)
            return null;

        var newRectangle = Instantiate(rectangle, spawnPos, Quaternion.identity);
        return newRectangle;
    }
}
