using System;
using UnityEngine;
using TMPro;


public class AutoDialog : MonoBehaviour
{
    public static event Action doneTalking;
    public string[] dialogues;
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;
    private Animator npcAnim;

    private int dialogueIndex = 0;
    private bool IsPlayerInRange = false;
    private bool isDialogueActive = false;
    public float Countdown = 1f;
    private float countdownTimer;
   

    private void OnEnable()
    {
        PlayerInfo.onGateMoved += StartDialogue;
        PlayerInfo.onGateMoved += GateIsOpened;
    }

    private void OnDisable()
    {
        PlayerInfo.onGateMoved -= StartDialogue;
        PlayerInfo.onGateMoved -= GateIsOpened;
    }
    void Start()
    {
        npcAnim = GetComponent<Animator>();
        countdownTimer = Countdown;
    }
    void Update()
    {
        if (IsPlayerInRange && !isDialogueActive)
        {
            StartDialogue();
        }
        else if (isDialogueActive && Countdown <= 0)
        {
            DisplayNextSentence();
        }
        Timer();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            npcAnim.SetTrigger("StartDialogue");

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
        
        Countdown = countdownTimer;
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
        IsPlayerInRange = false;
        doneTalking?.Invoke();
    }

    void Timer()
    {
        Countdown = Countdown - Time.deltaTime;
        if (Countdown <= 0)
        {
            Countdown = 0;
        }
    }

    void GateIsOpened()
    {
        IsPlayerInRange = true;
    }

}