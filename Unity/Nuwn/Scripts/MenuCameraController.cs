using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    float moveSpeed = 0.2f;
    Rigidbody rb;
    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var mousePosition = Camera.main.ViewportToScreenPoint(Input.mousePosition);
       // var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePosition).normalized;
        rb.velocity = new Vector3(dir.x * moveSpeed, dir.y * moveSpeed);

        //TODO: FixedJoint proper mouse movement
        var pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, startPos.x - 0.3f, startPos.x + 0.3f);
        pos.y = Mathf.Clamp(transform.position.y, startPos.y - 0.3f, startPos.y + 0.3f);
        transform.position = pos;
            
    }
}
