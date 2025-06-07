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
    public TMP_Text textBox;
    public GameObject uiText;
    
   

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
            Countdown = countdownTimer;
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
        Countdown= countdownTimer;
        dialogueIndex = 0;
        isDialogueActive = true;
        uiText.SetActive(true);
        
        DisplayNextSentence();
    }

    void DisplayNextSentence()
    {
        
        if (dialogueIndex < dialogues.Length)
        {
            textBox.text = dialogues[dialogueIndex];
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
        uiText.SetActive(false);
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