﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryBullet : Bullet
{
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetComponent<BaseEnemy>())
        {
            BaseEnemy b = collision.transform.GetComponent<BaseEnemy>();
            b.health -= (10.0f - damage);
        }
        else if (collision.transform.GetComponent<PlayerTargetingEnemy>())
        {
            PlayerTargetingEnemy tg = collision.transform.GetComponent<PlayerTargetingEnemy>();
            tg.health -= (10.0f - damage);
        }
        else if (collision.transform.GetComponent<ShootingEnemy>())
        {

            ShootingEnemy se = collision.transform.GetComponent<ShootingEnemy>();
            se.health -= (10.0f - damage);
        }
        else if(collision.transform.GetComponent<BossEnemy>())
        {
            BossEnemy be = collision.transform.GetComponent<BossEnemy>();
            be.health -= (10.0f - damage);
        }

        base.OnCollisionEnter2D(collision);
    }
}
