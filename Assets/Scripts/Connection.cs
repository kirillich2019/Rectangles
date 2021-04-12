using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Connection : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private Rectangle rectangle1;
    private Rectangle rectangle2;

    public Rectangle Rectangle1 => rectangle1;

    public Rectangle Rectangle2 => rectangle2;

    public void Initialize(Rectangle rectangle1, Rectangle rectangle2)
    {
        this.rectangle1 = rectangle1;
        this.rectangle2 = rectangle2;
    }

    public void UpdateConnection()
    {
        if(rectangle1 == null || rectangle2 == null)
            return;
        
        lineRenderer.SetPositions(new[] {rectangle1.transform.position, rectangle2.transform.position});
        lineRenderer.startColor = rectangle1.Color;
        lineRenderer.endColor = rectangle2.Color;
    }
}