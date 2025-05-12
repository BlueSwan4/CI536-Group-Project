using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public List<string> lines = new();
    public float textSpeed;
    private int index;

    private GameObject dialogueSource = null;

    // dialogue events, can use these if you want to time animations and other things correctly
    // each event references the dialogue's source game object
    public static event Action<GameObject> DialogueStartedEvent;
    public static event Action<GameObject> DialogueEndedEvent;

    // Start is called before the first frame update
    void OnEnable() // was switched to onenable so dialogue would cycle correctly
    {
        textComponent.text = string.Empty;
        StartDialogue();
        GameManager.Instance.UpdateGameState(GameState.InDialogue);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        DialogueStartedEvent.Invoke(dialogueSource);
        Debug.Log("Dialogue started");
        index = 0;
        StartCoroutine(TypeLine());
    }


    IEnumerator TypeLine()
    {
        Debug.Log("Index: " + index);
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

    }

    void NextLine()
    {
        if (index < lines.Count - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            Debug.Log("Cycling");
            gameObject.SetActive(false);
            // raise dialogue end event
            DialogueEndedEvent.Invoke(dialogueSource);
            GameManager.Instance.UpdateGameState(GameState.Wandering);
        }
    }

    public void SetLines(List<string> newLines, GameObject source)
    {
        dialogueSource = source;
        // use this to set dialogue shown to match those stored on an npc
        lines.Clear();

        foreach (string newLine in newLines)
        {
            lines.Add(newLine);
        }
    }
}
