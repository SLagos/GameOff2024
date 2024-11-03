using Unity.Netcode;
using UnityEngine;

public class MovementController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    Rigidbody rb;

    float moveHorizontal, moveVertical;
    Vector3 movement;

    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(!IsOwner) return;
        Move();
    }

    private void Move()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        rb.linearVelocity = movement * moveSpeed;
        transform.LookAt(transform.position + movement);
    }


}