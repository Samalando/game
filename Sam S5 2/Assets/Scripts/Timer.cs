using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float TimeVar = 5;
     

    // Update is called once per frame
    void Update()
    {
        TimeVar = TimeVar - Time.deltaTime;
        if (TimeVar <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
