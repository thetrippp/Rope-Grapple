using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject GrappleOrigin;

    public float climbSpeed = 5f;
    public float moveForce = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxis("Horizontal");
        if (y != 0)
        {
            int n = GrappleOrigin.GetComponent<GrappleScript>().Points.Count;
            Vector2 temp = GrappleOrigin.GetComponent<GrappleScript>().hingePoint - rb.position;
            transform.position += (Vector3)temp.normalized * climbSpeed * y;
        }

        rb.AddForce(Vector3.right * x * moveForce * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
            GrappleOrigin.SetActive(!GrappleOrigin.activeSelf);
    }
}
