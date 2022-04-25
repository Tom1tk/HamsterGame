using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public bool dead, inCombat;
    public int health, MaxHealth;
    public float healthRegenTimer, healthRegenCD;
    public float combatTimer, combatCD;
    public GameObject BallGameObject;
    public Material BallDmg0, BallDmg1 ,BallDmg2, BallDmg3, BallBroken;
    public Animator SpriteAnim;
    
    void Awake()
    {
        inCombat = false;
        health = MaxHealth;
        healthRegenTimer = 0f;
    }

    public void takeDmg()
    {
        health = health - 1;
        materialSwitch();
        SpriteAnim.SetTrigger("angry");

        if(health == 0)
        {
            dead = true;
        }
    }

    public void healDmg()
    {
        health = health + 1;
        materialSwitch();
        healthRegenTimer = 0f;
    }

    public void combatStarted()
    {
        inCombat = true;
    }
    public void combatEnded()
    {
        inCombat = false;
    }

    void materialSwitch()
    {
        switch(health)
            {
                case 0:
                    BallGameObject.GetComponent<MeshRenderer>().material = BallBroken;
                    break;
                case 1:
                    BallGameObject.GetComponent<MeshRenderer>().material = BallDmg3;
                    break;
                case 2:
                    BallGameObject.GetComponent<MeshRenderer>().material = BallDmg2;
                    break;
                case 3:
                    BallGameObject.GetComponent<MeshRenderer>().material = BallDmg1;
                    break;
                case 4:
                    BallGameObject.GetComponent<MeshRenderer>().material = BallDmg0;
                    break;
            }
    }

    void FixedUpdate()
    {
        if(dead)
        {
            Destroy(this.gameObject, 1f);
        }

        if((healthRegenTimer < healthRegenCD) && (health != MaxHealth) && (inCombat == false))
        {
            healthRegenTimer += Time.deltaTime;
        }else if((healthRegenTimer >= healthRegenCD) && (health != MaxHealth) && (inCombat == false))
        {
            healDmg();
        }else{
            healthRegenTimer = 0f;
        }

    }
}
