using System;
using UnityEngine;

public sealed class PlayerInput : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Camera cam;

    public event Action<Vector3> PlayerPressedField;
        
    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pos = GetMousePositionOnField();
            if(pos == null)
                return;
            InvokePlayerPressedField(pos.Value);
        }
    }

    private void InvokePlayerPressedField(Vector3 pos) => PlayerPressedField?.Invoke(pos);

    public Vector3? GetMousePositionOnField()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 1000, layerMask))
        {
            return hit.point;
        }

        return null;
    }
}