using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMovement : MonoBehaviour
{
    public bool displayDebugText;

    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Animator spriteAnim;

    UIScript UIref;

    [SerializeField]
    ParticleSystem ChargeFX;
    [SerializeField]
    ParticleSystem SpeedFX;
    [SerializeField]
    ParticleSystem DodgeFX;

    [SerializeField]
    Controls _controls;

    Vector2 movementInput;

    float dodgeInput;

    [SerializeField]
    Camera tpCamera;
    Transform cameraTransform;

    [SerializeField]
    Text debugText;

    public float maxVelocity;
    public float inputForce;
    public float dodgeForce;
    public float dodgeTimer;
    public float dodgeCooldown;
    public float boostForce;
    public float boostMax;
    public float boostLv;
    float boostFXdur;
    public bool charging;
    public float playerMagnitudeBeforePhysicsUpdate;
    

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        ChargeFX.Stop();

        _controls = new Controls();
        rb = GetComponent<Rigidbody>();
        spriteAnim = GetComponentInChildren<Animator>();
        tpCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        UIref = GameObject.Find("Canvas").GetComponent<UIScript>();
        cameraTransform = tpCamera.GetComponent<Transform>();

        dodgeTimer = dodgeCooldown;
        boostLv = 0f;
        charging = false;

        _controls.Player.Movement.started += ctx => movementInput = ctx.ReadValue<Vector2>();
        _controls.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        _controls.Player.Movement.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();

        _controls.Player.Boost.performed += _ => boostPress();
        _controls.Player.Boost.canceled += _ => boostRelease();

        _controls.Player.Dodge.started += ctx => dodgeInput = ctx.ReadValue<float>();
        _controls.Player.Dodge.performed += __ => dodge();
    }

    void FixedUpdate()
    {   
        playerMagnitudeBeforePhysicsUpdate = rb.velocity.magnitude;

        Vector3 directionInput = new Vector3(movementInput.x, 0f, movementInput.y);
        
        Vector3 relativeDirection = directionInput.x * cameraTransform.right + directionInput.z * new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z);

        relativeDirection.Normalize();

        if(rb.velocity.magnitude < maxVelocity && !charging)
        {
            rb.AddForce(relativeDirection * inputForce);
        }

        if(charging && boostLv < boostMax)
        {
            boostLv += Time.deltaTime;
        }

        if(dodgeTimer < dodgeCooldown)
        {
            dodgeTimer += Time.deltaTime;
        }else if(dodgeTimer >= dodgeCooldown)
        {
            UIref.hideDodgeUI();
        }

        spriteAnim.SetFloat("speed", rb.velocity.magnitude);

        if(displayDebugText)
        {
            debugText.text = 
            " player input= " + movementInput.ToString() +
            "\n mouse input= " + _controls.Player.Camera.ReadValue<Vector2>().ToString() +
            "\n \n directionInput= " + directionInput.ToString() + 
            "\n \n camera right transform = " + cameraTransform.right.ToString() +
            "\n camera forward transform= " + cameraTransform.forward.ToString() +
            "\n \n relatve direction= " + relativeDirection.ToString() +
            "\n \n player speed= " + rb.velocity.magnitude.ToString() +
            "\n \n player boost= " + boostLv.ToString() +
            "\n boost FX duration= " + boostFXdur.ToString() + 
            "\n \n player dodge= " + dodgeInput.ToString() +
            "\n dodge timer= " + dodgeTimer.ToString() +
            ""
            ;
        }
    }

    void boostPress()
    {
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        UIref.showBoostUI();
        boostLv = 0.1f;
        charging = true;
        ChargeFX.Play();
    }

    void boostRelease()
    {
        ChargeFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        SpeedFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = SpeedFX.main;
        main.duration = (boostLv*3f)/10f;

        rb.AddForce(new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z) * (boostLv*boostForce), ForceMode.Impulse);
        boostLv = 0f;
        charging = false;
        
        boostFXdur = main.duration;
        SpeedFX.Play();
        UIref.hideBoostUI();
    }

    void dodge()
    {
        if(dodgeTimer >= dodgeCooldown)
        {
            if(dodgeInput < 0f)
            {
                DodgeFX.Play();
                UIref.showDodgeUI();
                rb.AddForce(new Vector3(-cameraTransform.right.x, 0f, -cameraTransform.right.z) * (dodgeForce), ForceMode.Impulse);
                dodgeTimer = 0f;
            }else if(dodgeInput > 0f)
            {
                DodgeFX.Play();
                UIref.showDodgeUI();
                rb.AddForce(new Vector3(cameraTransform.right.x, 0f, cameraTransform.right.z) * (dodgeForce), ForceMode.Impulse);
                dodgeTimer = 0f;
            }
        }
    }

    private void OnEnable()
    {
        _controls.Player.Enable();
    }
    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Enemy")
        {   
            //get player and enemy speed before collision
            float playerCollisionSpeed = playerMagnitudeBeforePhysicsUpdate;
            float otherCollisionSpeed = other.gameObject.GetComponent<EnemyAI>().enemyMagnitudeBeforePhysicsUpdate;

            /*
            Debug.Log("player collision speed: " + playerCollisionSpeed); 
            Debug.Log("enemy collision speed: " + otherCollisionSpeed);
            */
            
            //whoever was going slower before the collision takes damage
            if (otherCollisionSpeed > playerCollisionSpeed && otherCollisionSpeed > 30f)
            {
                this.gameObject.GetComponent<HealthScript>().takeDmg();
                Debug.LogWarning("enemy was the faster object, player takes dmg");

            }else if(playerCollisionSpeed > otherCollisionSpeed && playerCollisionSpeed > 30f)
            {
                other.transform.gameObject.GetComponent<HealthScript>().takeDmg();
                Debug.LogWarning("player was the faster object, enemy takes dmg");
            }
        }
    }
}
