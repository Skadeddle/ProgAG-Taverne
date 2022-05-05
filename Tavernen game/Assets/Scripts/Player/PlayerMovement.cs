using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform look;




    private static float xRotation, yRotation;


    Vector3 velocity;

    [Header("Speed")]
    [SerializeField]
    public float Speed = 14f;
    [SerializeField]
    float SprintSpeedModifier = 1.4f;
    [SerializeField]
    float AirControle = 0.5f;

    [Header("Gravity and jumping")]
    [SerializeField]
    public float gravity = -9.8f * 2f;
    [SerializeField]
    public float jumpHeight = 1f;

    [Header("Jump advanced settings")]
    [SerializeField]
    float OnGroundTime = 0.1f;
    [SerializeField]
    float OnJumpkeyTime = 0.1f;




    private CharacterController Controller;

    void Awake()
    {

        Controller = GetComponent<CharacterController>();
    }


    float x, y;

    public bool CancelMovement() {
        return false;
    }





    public float GetSpeed() {
        return VerticalMove;
    }

    public float GetYVel()
    {
        return velocity.y;
    }

    float VerticalMove = 0f;
    float HorizontalMove = 0f;

    bool sprint;
    public bool getIsSprinting() {
        return sprint;
    }


    private void Move()
    {



        VerticalMove = Input.GetAxisRaw("Vertical");
        HorizontalMove = Input.GetAxisRaw("Horizontal");

        if (CancelMovement())
        {
            VerticalMove = 0f;
            HorizontalMove = 0f;
        }
     
        if(SlowDown != 1) {
            VerticalMove *= SlowDown;
            HorizontalMove *= SlowDown;
        }


        float sp = Speed;

        sprint = (Input.GetKey(KeyCode.LeftShift) && VerticalMove > 0);
        if (sprint)
            sp *= SprintSpeedModifier;



        velocity = Vector3.Lerp(velocity, (transform.right * HorizontalMove + transform.forward * VerticalMove).normalized * sp + velocity.y * Vector3.up, Time.deltaTime * (onGround ? 15 : 15 * AirControle));


        if (onGround)
        {

            velocity.y = -2f;

            Controller.slopeLimit = 50f;
            Controller.stepOffset = 0.5f;
        }
        else
        {
            Controller.slopeLimit = 0f;
            Controller.stepOffset = 0f;
        }

        if (onGround)
            canJumpTimer = OnGroundTime;
        if (Input.GetKey(KeyCode.Space))
            hasPressedJumpTimer = OnJumpkeyTime;
   
        if (canJumpTimer > 0f && hasPressedJumpTimer > 0f && !CancelMovement())
        {
            hasPressedJumpTimer = 0;
            canJumpTimer = 0;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        velocity.y += gravity * Time.deltaTime;

        velocity *= 0.999f;

        Controller.Move(velocity * Time.deltaTime);

        canJumpTimer -= Time.deltaTime;
        hasPressedJumpTimer -= Time.deltaTime;

        onGround = Controller.isGrounded;


    }

    bool onGround;
    float canJumpTimer = 0;
    float hasPressedJumpTimer = 0;

    public Quaternion getRot()
    {
        return Quaternion.Euler(xRotation, yRotation, 0f);
    }

    public void setRot(Quaternion rot)
    {
        xRotation = rot.eulerAngles.x;

    }
    public void setPos(Vector3 pos)
    {
        Controller.enabled = false;
        Controller.transform.position = pos;
        Controller.enabled = true;

    }
    public Vector3 getPos()
    {
        return transform.position;

    }

    public void applyRecoil(float x,float y) {
        yRotation += x;
        xRotation += y;
    }



    private void Look()
    {
        float mouseSpeed = 8;

        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        if (CancelMovement())
        {
            mouseX = 0f;
            mouseY = 0f;
        }



        yRotation += mouseX;


        xRotation -= mouseY;



        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        transform.eulerAngles = new Vector3(0, yRotation, 0);

    }

    public void AddSlowDown(float s) {
        SlowDown = s;
    }
    float SlowDown = 1;


    private void Update()
    {

   
        Move();
        Look();

        look.rotation = Quaternion.Lerp(look.rotation, Quaternion.Euler(xRotation, yRotation, 0), Time.deltaTime * 30);


        SlowDown = 1;

    }



}
