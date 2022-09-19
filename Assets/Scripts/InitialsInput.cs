using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InitialsInput : MonoBehaviour
{
    TMP_InputField inputField;

    bool isFilledOut = false;

    public bool IsFilledOut { get => isFilledOut; set => isFilledOut = value; }
    public string InputFieldText { get => inputField.text; }

    // Start is called before the first frame update
    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        inputField.Select();
        inputField.ActivateInputField();

        if (inputField.text.Length >= inputField.characterLimit)
        {
            IsFilledOut = true;
            inputField.caretColor = Color.black;
        }
        else
        {
            IsFilledOut = false;
            inputField.caretColor = Color.white;
        }
    }
}
