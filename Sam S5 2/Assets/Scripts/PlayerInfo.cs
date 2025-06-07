using System;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    public static event Action onGateMoved;
    public float healthPoints;
    public float key1Final;
    public float lavaDamageMultiplier;
    public Vector3 targetPosition;
    public Vector3 targetPosition2;
    public float glideSpeed;
    public GameObject gate;
    public TMP_Text healthAmount;
    public TMP_Text moneyAmount;
    public float money;
    public bool moving;
    public bool movingGate2;
    public GameObject character;
    public GameObject gameOverScreen;
<<<<<<< HEAD
    private float key1to2;
    public GameObject endScreen;
    public float key2Sect;
    public GameObject pressE;
    public GameObject npc;
    
=======
    public GameObject npc;
    private bool npcVisable = true;
>>>>>>> 66655d772cb97ce2c95e63fe8e97b151157e0678


public GameObject gateTwo;

private void OnEnable()
{
    AfterDialog.bossOver += over;
}

private void OnDisable()
{
    AfterDialog.bossOver -= over;
}

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
<<<<<<< HEAD

        if (movingGate2 == true)
        {
            GlideToPosTwo();
        }

        if (key1to2 == 1)
        {
            print("key");
=======
        if (npcVisable == false)
        {
            npc.SetActive(false);
>>>>>>> 66655d772cb97ce2c95e63fe8e97b151157e0678
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

        if (other.gameObject.tag == "end")
        {
            endScreen.SetActive(true);
        }

        if (other.gameObject.tag == "key_1-final")
        {
            key1Final = key1Final + 1;

            other.gameObject.SetActive(false);
            

<<<<<<< HEAD

=======
>>>>>>> 66655d772cb97ce2c95e63fe8e97b151157e0678
            print(key1Final);



        }
<<<<<<< HEAD

        if (key1Final >= 1)
        {

            if (other.gameObject.tag == "gate_1-final")
            {
                key1to2 = key1to2 - 1;

                moving = true;
                

=======
        if (key1Final >= 1)
        {
            npcVisable = false;
            if (other.gameObject.tag == "gate_1-final")
            {
                key1Final = key1Final - 1;

                moving = true;
>>>>>>> 66655d772cb97ce2c95e63fe8e97b151157e0678

                print(key1Final);
                
                npcVisable = false;
                float distanceToPlayer = Vector3.Distance(character.transform.position, transform.position);
                print(distanceToPlayer);
            }


        }

        if (key1to2 >= 1)
        {
            if (other.gameObject.tag == "gate_1-f2")
            {
                key1to2 = key1to2 - 1;

                movingGate2 = true;
                
                
                float distanceToPlayer = Vector3.Distance(character.transform.position, transform.position);
                print(distanceToPlayer);
            }
        }

        if (other.gameObject.tag == "key_1-f2")
        {
            key1to2 = key1to2 + 1;
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "money")
        {
            money = money + 1;
            other.gameObject.SetActive(false);
        }
<<<<<<< HEAD
        
        if (other.gameObject.tag == "key_2-sect")
        {
            key2Sect = key2Sect + 1;
            other.gameObject.SetActive(false);
        }
=======
>>>>>>> 66655d772cb97ce2c95e63fe8e97b151157e0678

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
                healthPoints = healthPoints - 2 * Time.deltaTime;
                if (healthPoints < 0)
                {
                    healthPoints = 0;
                }
            }
        }
<<<<<<< HEAD

        if (healthPoints > 0)
            
        {
            
        }
        print(healthPoints);
        if (npc.gameObject)
        {
            pressE.SetActive(true);
        }
=======
        print(healthPoints);
>>>>>>> 66655d772cb97ce2c95e63fe8e97b151157e0678
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
    
<<<<<<< HEAD
    private void GlideToPosTwo()
    {
        gateTwo.transform.position = Vector3.MoveTowards(gateTwo.transform.position, targetPosition2, glideSpeed * Time.deltaTime);
        if (Vector3.Distance(gateTwo.transform.position, targetPosition2) < 0.01f)
        {
            moving = false;
            print("Gate has reached its destination");
            
        }

    }

    public void over()
    {
        key1to2 += 1;
    }
    
=======
>>>>>>> 66655d772cb97ce2c95e63fe8e97b151157e0678
}
