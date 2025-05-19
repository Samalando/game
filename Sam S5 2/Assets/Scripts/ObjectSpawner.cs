using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnInterval = 2f;
    public bool canSpawn = true;

    private float timer = 0f;

    void Update()
    {
        if (canSpawn)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnObject();
                timer = 0f;
            }
        }
    }

    void SpawnObject()
    {
        if (objectToSpawn == null)
        {
            canSpawn = false;
            timer = 0f;
         }
        else
        {
            Instantiate(objectToSpawn, transform.position, transform.rotation);
        }
       
    }

   
}