using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Overlays;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    public static event Action onGateMoved;
    public float healthPoints;
    public float key1Final;
    public float lavaDamageMultiplier;
    public Vector3 targetPosition;
    public float glideSpeed;
    public GameObject gate;
    public TMP_Text healthAmount;
    public TMP_Text moneyAmount;
    public float money;
    public bool moving;
    public GameObject character;
    public GameObject gameOverScreen;
    public GameObject npc;
    private bool npcVisable = true;

    private void Update()
    {
        healthAmount.text = "hp: " + Mathf.RoundToInt(healthPoints);
        moneyAmount.text = "coins: " + money;
        if (moving == true)
        {
            GlideToPos();
        }
        if (healthPoints <= 0)
        {
            gameOverScreen.SetActive(true);
        }
        if (npcVisable == false)
        {
            npc.SetActive(false);
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
        if (other.gameObject.tag == "key_1-final")
        {
            key1Final = key1Final + 1;

            other.gameObject.SetActive(false);
            

            print(key1Final);



        }
        if (key1Final >= 1)
        {
            npcVisable = false;
            if (other.gameObject.tag == "gate_1-final")
            {
                key1Final = key1Final - 1;

                moving = true;

                print(key1Final);
                
                npcVisable = false;
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
        gate.transform.position = Vector3.MoveTowards(gate.transform.position, targetPosition, glideSpeed * Time.deltaTime);
        if (Vector3.Distance(gate.transform.position, targetPosition) < 0.01f)
        {
            moving = false;
            print("Gate has reached its destination");
            onGateMoved?.Invoke();
        }

    }
    
}
