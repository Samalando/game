using UnityEngine;
using TMPro;


public class NPCDialogueSystem : MonoBehaviour
{
    public string[] dialogues;
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;
    private Animator npcAnim;

    private int dialogueIndex = 0;
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    void Start()
    {
        npcAnim = GetComponent<Animator>();
    }
    void Update()
    {
        if (isPlayerInRange && !isDialogueActive)
        {
            StartDialogue();
        }
        else if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            DisplayNextSentence();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            npcAnim.SetTrigger("StartDialogue");

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            EndDialogue();
        }
    }

    void StartDialogue()
    {
        dialogueIndex = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        DisplayNextSentence();
    }

    void DisplayNextSentence()
    {
        if (dialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[dialogueIndex];
            dialogueIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}