using System;
using UnityEngine;

public class HandlerForClickingOnRectangles : MonoBehaviour
{
    public event Action<ClickType, Rectangle> ClickedOnRectangle;
    
    [SerializeField] private float clickDurationInSeconds;
    [SerializeField] private ClickEventsHolder eventsHolder;

    private int numberOfClicks;
    private float lastClickTime;
    private Rectangle lastRectangle;
        
    private void OnEnable()
    {
        eventsHolder.RectangleOnMouseDown += EventsHolderOnRectangleOnMouseDown;
        eventsHolder.RectangleOnMouseUp += EventsHolderOnRectangleOnMouseUp;
    }

    private void OnDisable()
    {
        eventsHolder.RectangleOnMouseDown -= EventsHolderOnRectangleOnMouseDown;
        eventsHolder.RectangleOnMouseUp -= EventsHolderOnRectangleOnMouseUp;
    }
        
    private void EventsHolderOnRectangleOnMouseDown(Rectangle rectangle)
    {
        if(rectangle == null)
            throw new ArgumentNullException(nameof(rectangle));

        if (lastRectangle == null || lastRectangle != rectangle)
        {
            lastRectangle = rectangle;
            numberOfClicks = 1;
        } 
        else if (lastRectangle == rectangle)
        {
            if (Time.time - lastClickTime <= clickDurationInSeconds)
                numberOfClicks++;
            else
                numberOfClicks = 1;
        }
        else
        {
            lastRectangle = rectangle;
            numberOfClicks = 1;
        }
        
        lastClickTime = Time.time;
        
        switch (numberOfClicks)
        {
            case 1:
                OnClickedOnRectangle(ClickType.Single, rectangle);
                break;
            case 2:
                OnClickedOnRectangle(ClickType.Double, rectangle);
                break;
        }
    }

    private void EventsHolderOnRectangleOnMouseUp(Rectangle rectangle)
    {
        if(lastRectangle == null || lastRectangle != rectangle)
            return;
        
        OnClickedOnRectangle(ClickType.Up, rectangle);
    }

    protected virtual void OnClickedOnRectangle(ClickType clickType, Rectangle rectangle)
    {
        ClickedOnRectangle?.Invoke(clickType, rectangle);
    }
}