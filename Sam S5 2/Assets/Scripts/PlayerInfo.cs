using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Overlays;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    public float healthPoints;
    public float itemExample;
    public float lavaDamageMultiplier;
    public Vector3 targetPosition;
    public float glideSpeed;
    public GameObject gate;
    public TMP_Text healthAmount;
    public TMP_Text moneyAmount;
    public float money;
    public bool moving;
    public GameObject character;

    private void Update()
    {
        healthAmount.text = "hp: " + healthPoints;
        moneyAmount.text = "coins: " + money;
        if (moving == true)
        {
            GlideToPos();
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "medkit")
        {
            healthPoints = healthPoints + 10;

            other.gameObject.SetActive(false);

            print(healthPoints);



        }
        if (other.gameObject.tag == "item")
        {
            itemExample = itemExample + 1;

            other.gameObject.SetActive(false);

            print(itemExample);



        }
        if (itemExample >= 1)
        {
            if (other.gameObject.tag == "gate")
            {
                itemExample = itemExample - 1;

                moving = true;
                
                print(itemExample);

                float distanceToPlayer = Vector3.Distance(character.transform.position, transform.position);
                print(distanceToPlayer);
            }
        }

        if (other.gameObject.tag == "money")
        {
            money = money + 1;
            other.gameObject.SetActive(false);
        }
       
    }
    private void OnTriggerStay(Collider other)
    {

        if (healthPoints <= 100)
        {

            if (other.gameObject.tag == "healingZone")
            {
                healthPoints = healthPoints + 10 * Time.deltaTime;
                //Mathf.RoundToInt(healthPoints);

            }
        }

        if (healthPoints > 0)
        {
            if (other.gameObject.tag == "lava")
            {
                healthPoints = healthPoints - 1 * Time.deltaTime;
                if (healthPoints < 0)
                {
                    healthPoints = 0;
                }
            }
        }
        print(healthPoints);  
    }

    private void GlideToPos()
    {
        gate.transform.position = Vector3.MoveTowards(transform.position, targetPosition, glideSpeed * Time.deltaTime);
    }
}
