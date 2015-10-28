using UnityEngine;
using System.Collections;

public class Custom_hero_ctrl : MonoBehaviour
{


    public float animSpeed = 1.2f;

    protected Animator animator;
    Transform[] bones;
    Rigidbody rigidbody;
    Transform hero;
    public float axisX;
    public float axisY;

    public float MouseX;
    public float MouseY;
    public float Mouse_SW;

    public float rotSpeed = 90.0f;
    bool doJumpDown;

    bool cam_rotate;
    bool Mouse_left_down;
    bool Mouse_right_down;
    bool Mouse_left_up;
    bool Mouse_right_up;
    bool Mouse_left_pressed;
    bool Mouse_right_pressed;

    public float setFloatDampTime = 0.15f;
    public float moveSpeed = 1.5f;

    public Transform headBone = null;


    public int zoomRate = 10;


    float desDist;

    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    public float rotationDampening = 3.0f;
    public float zoomDampening = 5.0f;
    float xAngl = 0.0f;
    float yAngl = 0.0f;


    public int minAngleY = -50;
    public int maxAngleY = 60;
    public Camera cam;
    public float heroHeight = 2.0f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;

    public float rotate_time = 0.1f;


    public float jumpHeight = 4.0f;

    bool grounded;

    Quaternion camRot;

    public LayerMask groundLayers = -1;
    CapsuleCollider capsuleCollider;
    RaycastHit groundHit;
    public float groundedDistance = 1.5f; // from capsule center


    Attack_Dir_Enmu attack_dir = Attack_Dir_Enmu.high;

    public Weapons weapons;
    [System.Serializable]
    public class Weapons
    {
        public GameObject sword_Hand = null;
        public GameObject sword_Holster = null;
        public GameObject bow_Hand = null;
        public GameObject bow_Holster = null;
        public GameObject bow_Quiver = null;
        public GameObject rifle_Hand = null;
        public GameObject rifle_Holster = null;
        public GameObject pistol_Hand = null;
        public GameObject pistol_Holster = null;
    }



    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        hero = GetComponent<Transform>();

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = 3.0f;
        //rigidbody.constraints = RigidbodyConstraints.FreezeRotation;


        bones = GetComponentsInChildren<Transform>() as Transform[];
        capsuleCollider = GetComponent<CapsuleCollider>();
        yAngl = cam.transform.eulerAngles.y;

        camRot = hero.rotation;

        foreach (Transform t in bones)
        {
            if (t.GetComponent<Rigidbody>() && t.GetComponent<Rigidbody>() != rigidbody)
                t.GetComponent<Rigidbody>().isKinematic = true;
            if (t.GetComponent<Collider>() && t.GetComponent<Collider>() != hero.GetComponent<Collider>())
                t.GetComponent<Collider>().isTrigger = true;
        }


        if (headBone == null)
        {
            foreach (Transform t in bones)
            {
                if (t.name == "head")
                    headBone = t;
            }
        }
        animator.SetLayerWeight(1, 1f);



        if (weapons.sword_Hand) weapons.sword_Hand.GetComponent<Renderer>().enabled = false;
        if (weapons.sword_Holster) weapons.sword_Holster.GetComponent<Renderer>().enabled = true; // On
        if (weapons.bow_Hand) weapons.bow_Hand.GetComponent<Renderer>().enabled = false;
        if (weapons.bow_Holster) weapons.bow_Holster.GetComponent<Renderer>().enabled = false;
        if (weapons.bow_Quiver) weapons.bow_Quiver.GetComponent<Renderer>().enabled = false;
        if (weapons.rifle_Hand) weapons.rifle_Hand.GetComponent<Renderer>().enabled = false;
        if (weapons.rifle_Holster) weapons.rifle_Holster.GetComponent<Renderer>().enabled = false;
        if (weapons.pistol_Hand) weapons.pistol_Hand.GetComponent<Renderer>().enabled = false;
        if (weapons.pistol_Holster) weapons.pistol_Holster.GetComponent<Renderer>().enabled = false;


    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        axisX = Input.GetAxis("Vertical");
        axisY = Input.GetAxis("Horizontal");

        MouseX = Input.GetAxis("Mouse X");
        MouseY = Input.GetAxis("Mouse Y");
        Mouse_SW = Input.GetAxis("Mouse ScrollWheel");
        Mouse_left_down = Input.GetMouseButtonDown(0);
        Mouse_right_down = Input.GetMouseButtonDown(1);
        Mouse_left_up = Input.GetMouseButtonUp(0);
        Mouse_right_up = Input.GetMouseButtonUp(1);

        Mouse_left_pressed = Input.GetMouseButton(0);
        Mouse_right_pressed = Input.GetMouseButton(1);


        cam_rotate = Input.GetKey(KeyCode.R);
        doJumpDown = Input.GetButtonDown("Jump");
        bool jumpOffNext = false;
        grounded = Physics.Raycast(hero.position + hero.up * capsuleCollider.center.y,hero.up * -1, out groundHit, groundedDistance, groundLayers);

        animator.SetFloat("MouseX", MouseX, setFloatDampTime, Time.deltaTime);
        animator.SetFloat("AxisX", axisX, setFloatDampTime, Time.deltaTime);
        animator.SetFloat("AxisY", axisY, setFloatDampTime, Time.deltaTime);


        desDist -= Mouse_SW * Time.deltaTime * zoomRate * Mathf.Abs(desDist);
        desDist = Mathf.Clamp(desDist, minDistance, maxDistance);

        animator.SetBool("Grounded", grounded);




        if (Input.GetKey(KeyCode.W))
        {
            hero.transform.Translate(Vector3.forward * Time.deltaTime * 3.5f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            hero.transform.Translate(Vector3.back * Time.deltaTime * 3.5f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            hero.transform.Translate(Vector3.left * Time.deltaTime * 3.5f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            hero.transform.Translate(Vector3.right * Time.deltaTime * 3.5f);
        }


        if (grounded)
        {
            if (doJumpDown)
            {
                animator.SetBool("Jump", true);
                rigidbody.velocity = hero.up * jumpHeight;
            }
        }




        if (cam_rotate)
        {
            calculate_angl();
            camRot = Quaternion.Euler(yAngl, xAngl, 0);
        }
        else if (Mouse_right_pressed)
        {
            calculate_angl();
            camRot = Quaternion.Euler(yAngl, xAngl, 0);

            Quaternion rotate = Quaternion.Euler(0, xAngl, 0);
            hero.transform.rotation = Quaternion.Lerp(transform.rotation, rotate, rotate_time);
        }





        Vector3 headPos = new Vector3(0, -heroHeight / 1.2f, 0);
        Vector3 camPos = hero.position - (camRot * Vector3.forward * desDist + headPos);

        cam.transform.rotation = camRot;
        cam.transform.position = camPos;



        if (Mouse_left_pressed)
        {
            if (Mathf.Abs(MouseX) >= Mathf.Abs(MouseY))
            {
                if (MouseX > 0.3)
                {
                    attack_dir = Attack_Dir_Enmu.left;
                }
                else if(MouseX < -0.3)
                {
                    attack_dir = Attack_Dir_Enmu.right;
                }

            }
            else
            {
                if (MouseY > 0.3)
                {
                    attack_dir = Attack_Dir_Enmu.high;
                }
                else if (MouseY < -0.3)
                {
                    attack_dir = Attack_Dir_Enmu.low;
                }
            }

            animator.SetInteger("Attack_Dir", (int)attack_dir);
            animator.SetBool("Attack_Down", false);
            animator.SetBool("Attack_Up", true);
        }
        else
        {
            animator.SetBool("Attack_Up", false);
            animator.SetBool("Attack_Down", true);
        }



        if (Mouse_right_pressed)
        {
            if (Mathf.Abs(MouseX) >= Mathf.Abs(MouseY))
            {
                if (MouseX > 0.3)
                {
                    attack_dir = Attack_Dir_Enmu.left;
                }
                else if (MouseX < -0.3)
                {
                    attack_dir = Attack_Dir_Enmu.right;
                }

            }
            else
            {
                if (MouseY > 0.3)
                {
                    attack_dir = Attack_Dir_Enmu.high;
                }
                else if (MouseY < -0.3)
                {
                    attack_dir = Attack_Dir_Enmu.low;
                }
            }
            animator.SetInteger("Attack_Dir", (int)attack_dir);

            animator.SetBool("Attack_Canneled", true);
        }
        else
        {
            animator.SetBool("Attack_Canneled", false);
        }



    }


    void calculate_angl()
    {
        xAngl += MouseX * xSpeed * 0.02f;

        yAngl -= MouseY * ySpeed * 0.02f;

        yAngl = ClampAngle(yAngl, minAngleY, maxAngleY);
    }




    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }





}
