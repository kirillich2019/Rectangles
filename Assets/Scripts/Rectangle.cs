using UnityEngine;

public class Rectangle : MonoBehaviour
{
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private ClickEventsHolder clickEventsHolder;
    
    public Color Color { get; private set; }

    private void OnMouseDown() => clickEventsHolder.OnRectangleOnMouseDown(this);

    private void OnMouseUp() => clickEventsHolder.OnRectangleOnMouseUp(this);

    private void Start()
    {
        Color = Random.ColorHSV(0, 1, 0, 1, 1, 1);
        renderer.material.color = Color;
    }
}