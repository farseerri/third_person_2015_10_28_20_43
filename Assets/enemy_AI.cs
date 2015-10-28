using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemy_AI : MonoBehaviour
{
    public float animSpeed = 0.3f;

    protected Animator animator;
    Transform[] bones;
    Rigidbody rigidbody;
    Transform self;
    GameObject enemy;

    float speed;
    float angle;

    public Transform headBone = null;

    
    public int zoomRate = 10;


    public float rotate_time = 0.1f;

    Quaternion camRot;


    bool attack = false;
    NavMeshAgent navMeshAgent;

    public float speedDampTime = 0.1f;              // Damping time for the Speed parameter.
    public float angularSpeedDampTime = 0.7f;       // Damping time for the AngularSpeed parameter
    public float angleResponseTime = 0.6f;          // Response time for turning an angle into angularSpeed.


    Attack_Parameter self_attack_parameter;

    Self_State_Enmu self_state;


    IEnumerator self_combat_Coroutine;
    //bool self_combat_Coroutine_is_running;
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

        self_state = Self_State_Enmu.move;

        animator = GetComponent<Animator>();

        self = GetComponent<Transform>();

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = 3.0f;
        //rigidbody.constraints = RigidbodyConstraints.FreezeRotation;


        bones = GetComponentsInChildren<Transform>() as Transform[];

        camRot = self.rotation;

        foreach (Transform t in bones)
        {
            if (t.GetComponent<Rigidbody>() && t.GetComponent<Rigidbody>() != rigidbody)
                t.GetComponent<Rigidbody>().isKinematic = true;
            if (t.GetComponent<Collider>() && t.GetComponent<Collider>() != self.GetComponent<Collider>())
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


        if (weapons.sword_Hand) weapons.sword_Hand.GetComponent<Renderer>().enabled = false;
        if (weapons.sword_Holster) weapons.sword_Holster.GetComponent<Renderer>().enabled = true; // On
        if (weapons.bow_Hand) weapons.bow_Hand.GetComponent<Renderer>().enabled = false;
        if (weapons.bow_Holster) weapons.bow_Holster.GetComponent<Renderer>().enabled = false;
        if (weapons.bow_Quiver) weapons.bow_Quiver.GetComponent<Renderer>().enabled = false;
        if (weapons.rifle_Hand) weapons.rifle_Hand.GetComponent<Renderer>().enabled = false;
        if (weapons.rifle_Holster) weapons.rifle_Holster.GetComponent<Renderer>().enabled = false;
        if (weapons.pistol_Hand) weapons.pistol_Hand.GetComponent<Renderer>().enabled = false;
        if (weapons.pistol_Holster) weapons.pistol_Holster.GetComponent<Renderer>().enabled = false;


        self_attack_parameter = new Attack_Parameter();

        animator.SetLayerWeight(1, 1f);


        navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();

        enemy = GameObject.Find("hero");
        animator = GetComponent<Animator>();

        IEnumerator custom_nav_Coroutine = Timer.Timer_IEnumerator(0.5f, custom_nav_fuc);

        StartCoroutine(custom_nav_Coroutine);

    }



    // Update is called once per frame
    void FixedUpdate()
    {
        switch (self_state)
        {
            case Self_State_Enmu.idle:
                {

                }
                break;


            case Self_State_Enmu.move:
                {
                    speed = Vector3.Project(navMeshAgent.desiredVelocity, transform.forward).magnitude;

                    angle = FindAngle(transform.forward, navMeshAgent.desiredVelocity, transform.up);
                    animator.SetInteger("Self_State", (int)(Self_State_Enmu.move));
                    animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
                    animator.SetFloat("Angle", angle, angularSpeedDampTime, Time.deltaTime);
                    navMeshAgent.Resume();
                }
                break;

            case Self_State_Enmu.attack:
                {
                    speed = Vector3.Project(navMeshAgent.desiredVelocity, transform.forward).magnitude;

                    angle = FindAngle(transform.forward, navMeshAgent.desiredVelocity, transform.up);
                    animator.SetInteger("Self_State", (int)(Self_State_Enmu.attack));
                    animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
                    animator.SetFloat("Angle", angle, angularSpeedDampTime, Time.deltaTime);
                    navMeshAgent.Stop();
                }
                break;


            default:
                {

                }
                break;
        }





    }

    void LateUpdate()
    {
        switch (self_state)
        {
            case Self_State_Enmu.idle:
                {

                }
                break;


            case Self_State_Enmu.move:
                {
                    self.transform.LookAt(enemy.transform);
                }
                break;

            case Self_State_Enmu.attack:
                {
                    self.transform.LookAt(enemy.transform);
                }
                break;


            default:
                {

                }
                break;
        }
    }





    float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        // If the vector the angle is being calculated to is 0...
        if (toVector == Vector3.zero)
            // ... the angle between them is 0.
            return 0f;

        // Create a float to store the angle between the facing of the enemy and the direction it's travelling.
        float angle = Vector3.Angle(fromVector, toVector);

        // Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
        Vector3 normal = Vector3.Cross(fromVector, toVector);

        // The dot product of the normal with the upVector will be positive if they point in the same direction.
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));

        // We need to convert the angle we've found from degrees to radians.
        angle *= Mathf.Deg2Rad;

        return angle;
    }



    void custom_nav_fuc()
    {


        switch (self_state)
        {
            case Self_State_Enmu.idle:
                {

                }
                break;


            case Self_State_Enmu.move:
                {

                    navMeshAgent.SetDestination(enemy.transform.position);
                    if (Vector3.Distance(enemy.transform.position, self.position) < 2f)
                    {
                        self_state = Self_State_Enmu.attack;
                        self_combat_Coroutine = Timer.Timer_IEnumerator(5f, custom_self_combat);
                        StartCoroutine(self_combat_Coroutine);
                    }


                }
                break;


            case Self_State_Enmu.attack:
                {

                    if (Vector3.Distance(enemy.transform.position, self.position) >= 2f)
                    {
                        self_state = Self_State_Enmu.move;
                        StopCoroutine(self_combat_Coroutine);
                    }
                }
                break;


            default:
                {

                }
                break;


        }

    }








    void custom_self_combat()
    {
        //navMeshAgent.SetDestination(enemy.transform.position);
        Attack_Dir_Enmu attack_dir = self_attack_parameter.update_radom_value();
        StartCoroutine(wave_weapon(attack_dir));
    }


    IEnumerator wave_weapon(Attack_Dir_Enmu attack_dir)
    {
        animator.SetInteger("Attack_Dir", (int)attack_dir);
        animator.SetBool("Attack", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("Attack", false);
    }
    

}
