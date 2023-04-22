using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector2 lastPos;

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Mouse2)) {
            lastPos = mousePos;
        }

        if (Input.GetKey(KeyCode.Mouse2))
        {
            Camera.main.transform.Translate(lastPos - mousePos);

        }
        else {
            Vector2 mov = new Vector2();
            if (Input.GetKey(KeyCode.W)) mov.y += 1;
            if (Input.GetKey(KeyCode.S)) mov.y -= 1;
            if (Input.GetKey(KeyCode.A)) mov.x -= 1;
            if (Input.GetKey(KeyCode.D)) mov.x += 1;
            Camera.main.transform.Translate(mov * Time.deltaTime * Camera.main.orthographicSize);
        }

        Camera.main.orthographicSize = Camera.main.orthographicSize + Input.mouseScrollDelta.y * Camera.main.orthographicSize * -.1f;
        if (Input.mouseScrollDelta.y == 0) {
            int delta = 0;
            if (Input.GetKey(KeyCode.E)) delta++;
            if (Input.GetKey(KeyCode.Q)) delta--;
            Camera.main.orthographicSize = Camera.main.orthographicSize - Camera.main.orthographicSize * delta * Time.deltaTime * 5;
        }
    }
}
