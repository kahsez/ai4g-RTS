using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{
    // Update is called once per frame
    protected override void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Efecto de luz sobre el ratón (se puede quitar)
        Position = new Vector3(mousePos.x, mousePos.y, 0);

        if (Input.GetKey(KeyCode.A))
        {
            _static.Orientation += 5f;
            transform.Rotate(Vector3.forward, 5f, Space.World);           
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _static.Orientation -= 5f;
            transform.Rotate(Vector3.forward, -5f, Space.World);
        }

        transform.position = this.Position;
    }

}
