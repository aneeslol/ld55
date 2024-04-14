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
        var button = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            button = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            button = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            button = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            button = 4;
        }

        if (button != -1)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    FloorClicked?.Invoke(button, hit.point);
                }
            }
        }
    }
}
