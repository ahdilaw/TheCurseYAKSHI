using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Windows;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogue;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnHideDialog;
    private PlayerControl playerControls;
    private bool is_I_Pressed = false;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerControls = new PlayerControl();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    int currentLine = 0;
    Dialog dialog;
    bool isTyping;

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        this.dialog = dialog;
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HandleUpdate()
    {
        bool is_I_held = playerControls.Interaction.Interact.ReadValue<float>() > 0.1f;

        if (is_I_held && !isTyping && !is_I_Pressed)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                dialogBox.SetActive(false);
                currentLine = 0;
                OnHideDialog?.Invoke();
            }
            is_I_Pressed = true;
        }
        else if (!is_I_held)
        {
            is_I_Pressed = false;
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogue.text = "";

        foreach (var letter in line.ToCharArray())
        {
            dialogue.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
