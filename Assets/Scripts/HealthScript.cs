using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public bool died, inCombat;
    public int health, MaxHealth;
    public float healthRegenTimer, healthRegenCD;
    public float combatTimer, combatCD;
    public GameObject BallGameObject;
    public Material BallDmg0, BallDmg1 ,BallDmg2, BallDmg3, BallBroken;
    public Animator SpriteAnim;
    UIScript UIref;
    
    void Awake()
    {
        UIref = GameObject.Find("Canvas").GetComponent<UIScript>();
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
            died = true;
        }
    }

    public void healDmg()
    {
        if(health >= MaxHealth)
        {
            health = MaxHealth;
        }else{
            health = health + 1;
        }
        materialSwitch();
        //healthRegenTimer = 0f;
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
        if(died)
        {
            if(this.gameObject.CompareTag("Player"))
            {
                UIref.showLoseUI();
                Destroy(this.gameObject);
            }else{
                UIref.enemiesAlive -= 1;
                //leaves the hamster sprite exposed from its broken ball whilst disabling the AI and halting any particle effects
                Destroy(this.gameObject.GetComponent<EnemyAI>());
                Destroy(this.gameObject.GetComponentInChildren<ParticleSystem>());
                Destroy(this.gameObject, 1.5f);
            }
            died = false;
        }

        //CoD style out-of-combat healing for HP, scrapped for simpler healing with strawberries
        /*
        if((healthRegenTimer < healthRegenCD) && (health != MaxHealth) && (inCombat == false))
        {
            healthRegenTimer += Time.deltaTime;
        }else if((healthRegenTimer >= healthRegenCD) && (health != MaxHealth) && (inCombat == false))
        {
            healDmg();
        }else{
            healthRegenTimer = 0f;
        }*/

    }
}
