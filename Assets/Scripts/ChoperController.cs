using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoperController : MonoBehaviour
{
    private Rigidbody rb;
    public float rotorSpeed=5;
    float time = 0f;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        time +=Time.deltaTime;

        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * rotorSpeed;
        rb.MovePosition(rb.position+moveVelocity*Time.deltaTime);
        //gameObject.transform.rotation = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 60, 0), time);
    }
}
