using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Playerstate { Idle, Move, Attack}
public class PlayerController : MonoBehaviour
{
    Camera cam;

    private CharacterController controller;

    private Playerstate state;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField] 
    private float moveY;

    [SerializeField]
    private int hp;

    [SerializeField]
    private float concentrate;

    private void Awake()
    {
        controller= GetComponent<CharacterController>();
    }
    public void Start()
    {
        cam = Camera.main;
        state = Playerstate.Idle;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        Move();
        Jump();
        Attack();
    }

    public void Move()
    {
        if(state != Playerstate.Attack)
        {
            Vector3 forwardVec = new Vector3(Camera.main.transform.forward.x,0f,Camera.main.transform.forward.z).normalized;
            Vector3 rightVec = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;

            Vector3 moveInput = Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
            if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();

            Vector3 moveVec = forwardVec* moveInput.z + rightVec* moveInput.x;

            controller.Move(moveVec * moveSpeed * Time.deltaTime);
        }
    }

    //private void Rotate()
    //{
    //    Vector3 forwardVec = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
    //    transform.forward = forwardVec;
    //}

    public void Jump()
    {
        if (state != Playerstate.Attack)
        {
            moveY += Physics.gravity.y * Time.deltaTime;


            if (Input.GetButtonDown("Jump"))
            {
                moveY = jumpSpeed;
            }
            else if (controller.isGrounded && moveY < 0)
            {
                //moveY += Physics.gravity.y * 0.1f;
                moveY = 0;
            }


            controller.Move(Vector3.up * moveY * Time.deltaTime);
        }

    }

    public void Attack()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            //Ray ray = GetComponent<Ray>();
            //RaycastHit hit;
            
        }
    }

    public bool IsGround()
    {

        return false;
    }
}
