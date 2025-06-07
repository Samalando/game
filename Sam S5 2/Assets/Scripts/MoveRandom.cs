using System;
using System.Collections;
using System.Collections.Generic;
using MalbersAnimations.Controller;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveRandom : MonoBehaviour
{
    public float tome = 5f;
    private float tomeInicial;
    public Vector3 areaSize = new Vector3(10f, 0f, 10f);
    public float glideSpeed = 10f;
    public Vector3 areaCenter = Vector3.zero;
    private Vector3 targetPosition;

    private void Start()
    {
        tomeInicial= tome;
        PosRandom();
    }
    
    void Update()
    {
        if (tome <= 0)
        {
            PosRandom();
            tome = tomeInicial; 
        }
        GlidePos();
        Timer(); 
        
    }

    void Timer()
    {
        tome = tome - Time.deltaTime;
    }

    void GlidePos()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, glideSpeed * Time.deltaTime);
    }
    void PosRandom()
    {
        float ranX = Random.Range(areaCenter.x - areaSize.x , areaCenter.x + areaSize.x );
        float ranZ = Random.Range(areaCenter.z - areaSize.z , areaCenter.z + areaSize.z );
        targetPosition = new Vector3(ranX, transform.position.y, ranZ);
    }
}
