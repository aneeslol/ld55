using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] Rigidbody Body;

    public static event Action<int, Vector3> FloorClicked;

    void Start()
    {

    }

    void Update()
    {
        var mouseButton = -1;
        if (Input.GetMouseButtonDown(0))
        {
            mouseButton = 0;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            mouseButton = 1;
        }

        if (mouseButton != -1)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    FloorClicked?.Invoke(mouseButton, hit.point);
                }
            }
        }
    }
}
