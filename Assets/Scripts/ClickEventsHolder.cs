using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ClickEventsHolder", menuName = "ScriptableObjects/ClickEventsHolder", order = 0)]
public sealed class ClickEventsHolder : ScriptableObject
{
    public event Action<Rectangle> RectangleOnMouseDown;
    public event Action<Rectangle> RectangleOnMouseUp;

    public void OnRectangleOnMouseUp(Rectangle rectangle) => RectangleOnMouseUp?.Invoke(rectangle);

    public void OnRectangleOnMouseDown(Rectangle rectangle) => RectangleOnMouseDown?.Invoke(rectangle);
}