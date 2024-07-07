using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI speaker_text;
    [SerializeField] private TextMeshProUGUI speaker_dialog;
    private GameObject self;

    // Start is called before the first frame update
    void Start()
    {
        if (speaker_dialog == null || speaker_text == null)
        {
            Debug.Log("The speak instances not serialized. Err# 01.");
            return;
        }
        self = this.gameObject;
        self.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // EXTERNAL Method to display the dialog
    public void ShowDialog(string speaker, string dialog)
    {
        speaker_text.text = speaker;
        speaker_dialog.text = dialog;
        self.SetActive(true);
    }

    // EXTERNAL Method to hide the dialog
    public void HideDialog()
    {
        self.SetActive(false); 
    }

}
