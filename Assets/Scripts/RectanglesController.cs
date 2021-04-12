using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RectanglesController : MonoBehaviour
{
    [SerializeField] private LayerMask rectanglesLayer;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private RectangleFactory rectangleFactory;
    [SerializeField] private HandlerForClickingOnRectangles clickingOnRectanglesHandler;
    [SerializeField] private Connection connectionPrefab;
    [SerializeField] private float moveRectangleDelayInSeconds;

    private readonly Vector3 ELEVATION = new Vector3(0, 0.5f, 0);
    private Coroutine moveRectangle;
    private Rectangle selectedRectangle;

    private readonly Dictionary<Rectangle, Dictionary<Rectangle, Connection>> rectConDictionary =
        new Dictionary<Rectangle, Dictionary<Rectangle, Connection>>();

    private void OnEnable()
    {
        playerInput.PlayerPressedField += PlayerPressedField;
        clickingOnRectanglesHandler.ClickedOnRectangle += RectangleClicked;
    }

    private void OnDisable()
    {
        playerInput.PlayerPressedField -= PlayerPressedField;
        clickingOnRectanglesHandler.ClickedOnRectangle -= RectangleClicked;
    }

    private void RectangleClicked(ClickType clickType, Rectangle rectangle)
    {
        switch (clickType)
        {
            case ClickType.Single:
                if (selectedRectangle != null && selectedRectangle != rectangle)
                {
                    CreateConnections(selectedRectangle, rectangle);
                    selectedRectangle = null;
                }
                else
                    selectedRectangle = rectangle;

                StartMoveRectangle(rectangle);
                break;
            case ClickType.Double:
                DeleteRectangle(rectangle);
                break;
            case ClickType.Up:
                if (moveRectangle != null)
                    StopMoveRectangle();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(clickType), clickType, null);
        }
    }

    private void CreateConnections(Rectangle rect1, Rectangle rect2)
    {
        if (rect1 == null || rect2 == null)
            return;

        if (ConnectionExist(rect1, rect2))
            return;
        
        var newConnection = InstantiateNewConnection(rect1, rect2);
        SaveConnection(rect1, rect2, newConnection);
    }

    private bool ConnectionExist(Rectangle rect1, Rectangle rect2) =>
        rect1 != null && rectConDictionary.ContainsKey(rect1) && rectConDictionary[rect1] != null &&
        rectConDictionary[rect1].ContainsKey(rect2);

    private Connection InstantiateNewConnection(Rectangle rect1, Rectangle rect2)
    {
        var newConnection = Instantiate(connectionPrefab);
        newConnection.Initialize(rect1, rect2);
        newConnection.UpdateConnection();
        return newConnection;
    }
    
    private void SaveConnection(Rectangle rect1, Rectangle rect2, Connection connection)
    {
        if (rectConDictionary.ContainsKey(rect1))
        {
            if(rectConDictionary[rect1] == null)
                rectConDictionary[rect1] = new Dictionary<Rectangle, Connection>();
            rectConDictionary[rect1][rect2] = connection;
        }
        else
            rectConDictionary[rect1] = new Dictionary<Rectangle, Connection> {{rect2, connection}};

        if (rectConDictionary.ContainsKey(rect2))
        {
            if(rectConDictionary[rect2] == null)
                rectConDictionary[rect2] = new Dictionary<Rectangle, Connection>(); 
            rectConDictionary[rect2][rect1] = connection;
        }
        else
            rectConDictionary[rect2] = new Dictionary<Rectangle, Connection> {{rect1, connection}};
    }

    private void StartMoveRectangle(Rectangle rectangle)
    {
        moveRectangle = StartCoroutine(MoveRectangle(rectangle));
    }

    private void StopMoveRectangle()
    {
        if (moveRectangle != null)
            StopCoroutine(moveRectangle);
    }
    
    private IEnumerator MoveRectangle(Rectangle rectangle)
    {
        yield return new WaitForSeconds(moveRectangleDelayInSeconds);
        selectedRectangle = null;

        var rectangleArea = new RectangleArea(rectangle.transform.localScale / 2, rectanglesLayer);

        Connection[] connections = null;
        if (rectConDictionary.ContainsKey(rectangle))
            connections = rectConDictionary[rectangle].Values.ToArray();
        var isconnectionNotNullOrEmpty = connections != null && connections.Length > 0;

        while (true)
        {
            if (rectangle == null)
                yield break;

            var pos = playerInput.GetMousePositionOnField();
            if (pos == null)
            {
                yield return null;
                continue;
            }

            pos += ELEVATION;
            var areaIsFree = rectangleArea.CheckAreaIsFree(pos.Value, rectangle.gameObject);

            if (!areaIsFree)
            {
                yield return null;
                continue;
            }

            rectangle.transform.position = pos.Value;
            if (isconnectionNotNullOrEmpty)
                foreach (var connection in connections)
                    connection.UpdateConnection();

            yield return null;
        }
    }

    private void DeleteRectangle(Rectangle rectangle)
    {
        TryDeleteConnections(rectangle);
        Destroy(rectangle.gameObject);
    }

    private void TryDeleteConnections(Rectangle rect)
    {
        if(rect == null || !rectConDictionary.ContainsKey(rect))
            return;

        var connections = rectConDictionary[rect].Values.ToArray();
        for (var i = 0; i < connections.Length; i++)
        {
            var connection = connections[i];
         
            if(connection == null)
                continue;
            
            rectConDictionary[connection.Rectangle1].Remove(connection.Rectangle2);
            rectConDictionary[connection.Rectangle2].Remove(connection.Rectangle1);

            if(connection.gameObject != null)
                Destroy(connection.gameObject);
        }

        rectConDictionary.Remove(rect);
    }

    private void PlayerPressedField(Vector3 clickPos)
    {
        var spawnPos = clickPos + ELEVATION;
        rectangleFactory.TrySpawnNewRectangle(spawnPos);
    }
}