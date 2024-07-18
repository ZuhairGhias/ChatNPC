using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Extensions;

public class PlayerChatInput : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField inputField;

    //public static Action<string> onEndEdit;

    // Start is called before the first frame update
    void Start()
    {
        DebugUtils.HandleErrorIfNullGetComponent(inputField, this);
        inputField.DeactivateInputField(true);
        inputField.SetActive(false);
        //inputField.onEndEdit.AddListener(CaptureInput);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableInput()
    {
        inputField.SetActive(true);
        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }

    //public void CaptureInput(string text)
    //{
    //    inputField.SetActive(true);
    //    inputField.DeactivateInputField(true);
    //    onEndEdit.Invoke(text);
    //}

    public string CaptureInput()
    {
        inputField.SetActive(false);
        inputField.DeactivateInputField(true);
        return inputField.text;
    }
}
