﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public float health = 100f;
    public float bulletOffset = 0.6f;
    public float firerate = 0.5f;
    public float bulletSpeed = 5f;
    public GameObject bulletPrefab;


    private Buff currentBuff;
    float buffTime = 0;
    float buffTimeElapsed = 0;


    [SerializeField] float respawnTime;
    float timeBeforeRespawnElapsed;
    [SerializeField] SpriteRenderer sprite;

    public bool isDead;
    private Revive reviveCircle;

    private PlayerMovement pm;
    private float fireTimer;
    public GameObject reviveObject;

    public Transform bar;

    public Vector2 Position { get { return transform.position; } }

    private Vector2 lastDirection = new Vector2(1, 0);

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;

        //
        if (currentBuff != null)
        {
            buffTimeElapsed += Time.deltaTime;
            if (buffTimeElapsed >= buffTime)
            {
                RemoveBuff();
            }
        }
  
        //Check if the player has died
        if (isDead)
        {
            timeBeforeRespawnElapsed += Time.deltaTime;
            if (timeBeforeRespawnElapsed >= respawnTime)
            {
                //"respawn" the player
                transform.position = Vector2.zero;
                RevivePlayer();
            }
        }
        else
        {
            Death();
        }

        SetSize(health/100);
        Debug.Log(health);
    }

    public void SetSize(float health)
    {
        bar.localScale = new Vector3(health, 1f);
    }

    public void Death()
    {
        if(health <= 0)
        {
            //Prevent the player from moving
            pm.enabled = false;
            isDead = true;
            //sprite.material.color = new Color(51,51,51);
            Debug.Log("Spawned");

            pm.FreezeRB();

            Revive revCircle = Instantiate(reviveObject, transform.position + -Vector3.forward * 3f, Quaternion.identity).GetComponent<Revive>();
            revCircle.SetParameters(this);
            reviveCircle = revCircle;
        }
    }

    public virtual void Fire(Vector2 direction)
    {
        if (direction.magnitude < 0.1f)
            direction = lastDirection;

        // Make sure that the fire direction is normalized.
        direction = direction.normalized;

        if (!isDead)    //Dont allow player to shoot if theyre dead
        {
            if (fireTimer <= 0)
            {
                // Shoot code.
                // Create the bullet.
                GameObject bullet = Instantiate(bulletPrefab, Position + direction * bulletOffset, Quaternion.FromToRotation(Vector3.up, direction));

                // Get the bullets physics component.
                Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();

                // Set the bullets velocity.
                brb.velocity = direction * bulletSpeed;

                //At the end of shooting, make sure to reset the firerate.
                fireTimer = firerate;
            }
        }
        lastDirection = direction;
    }

    public void SetDirection(Vector2 direction)
    {
        pm.SetDirection(direction);
    }

    public void RevivePlayer()
    {
        pm.enabled = true;
        //Reset player paramters
        isDead = false;
        timeBeforeRespawnElapsed = 0;
        health = 100f;
        sprite.material.color = Color.white;

        if(reviveCircle != null)
        {
            Destroy(reviveCircle.gameObject);
        }

        pm.UnfreezeRB();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Apply Buff
        if (collision.collider.CompareTag("BuffPickup"))
        {
            ApplyBuff(collision.collider.gameObject.GetComponent<BuffPickup>().BuffToPickup);
            Destroy(collision.collider.gameObject);
        }
    }

    /// <summary>
    /// Applies the picked up buff
    /// </summary>
    /// <param name="buffToApply">The buff to apply to the player</param>
    private void ApplyBuff(Buff buffToApply)
    {

       // Debug.Log("Applied Buff");
        //If there is already a buff applied to the player, remove it
        if (currentBuff != null)
        {
            RemoveBuff();
        }

        //Apply buff stats
        currentBuff = buffToApply;
        this.health += buffToApply.health;
        this.firerate -= buffToApply.fireRate;
        this.bulletSpeed += buffToApply.bulletSpeed;
        buffTime = buffToApply.buffTime;
        buffTimeElapsed = 0;

    }

    /// <summary>
    /// Removes the currently applied buff
    /// </summary>
    private void RemoveBuff()
    {
      //  Debug.Log("Remove Buff");
        if (currentBuff != null)
        {
           // this.health -= currentBuff.health;
            this.firerate += currentBuff.fireRate;
            this.bulletSpeed -= currentBuff.bulletSpeed;
            currentBuff = null;
            buffTime = 0;
            buffTimeElapsed = 0;
        }
    }

}
