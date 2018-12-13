using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        this.transform.position += this.transform.rotation * GetMoveInput();
        this.transform.eulerAngles += GetRotateInput();
        Camera.main.fieldOfView += GetFieldOfViewInput(Camera.main);
    }

    public static Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3();
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            move.z += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            move.z -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            move.x += 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            move.x -= 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            move.y += 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            move.y -= 1;
        }
        move = move.normalized * Time.deltaTime;
        return move;
    }

    private static Vector3 lastmousepos;
    public static Vector3 GetRotateInput()
    {
        Vector3 rotate = new Vector3();
        if (Input.GetMouseButton(1))
        {
            var move = Input.mousePosition - lastmousepos;
            rotate.y = move.x / Screen.width * 180f;
            rotate.x = -move.y / Screen.height * 90f;
        }
        lastmousepos = Input.mousePosition;

        return rotate;
    }

    public static float GetFieldOfViewInput(Camera camera)
    {
        if (Input.mouseScrollDelta.y == 0) return 0;
        var y = camera.fieldOfView;
        var d = -Input.mouseScrollDelta.y;
        if (y >= 1 && y + d < 1)
        {
            d += y - 1 + 0.00001f;
            y = 1 - 0.00001f;
        }
        if (y <= 1 && 1 / y - d < 1)
        {
            d -= 1 / y - 1 - 0.00001f;
            y = 1 + 0.00001f;
        }
        if (y > 1)
        {
            y += d;
        }
        else
        {
            y = 1 / y;
            y -= d;
            y = 1 / y;
        }
        return y - camera.fieldOfView;
    }
}
