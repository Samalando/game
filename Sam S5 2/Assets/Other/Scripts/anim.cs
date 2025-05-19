using UnityEngine;

public class anim : MonoBehaviour
{
    public Animator npcAnim;

    void Start()
    {
        npcAnim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            npcAnim.SetTrigger("StartDialogue");
        }
    }
}
