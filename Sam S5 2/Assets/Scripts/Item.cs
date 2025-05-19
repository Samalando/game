using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float itemExample;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            itemExample = itemExample + 10;

            other.gameObject.SetActive(false);

            print(itemExample);



        }
    }
}
