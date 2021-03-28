using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorHandler : Singleton<ErrorHandler>
{
    public class ErrorLog
    {
        public Exception Exception { get; private set; }
        public ErrorLog(Exception exception)
        {
            Exception = exception;
        }
    }
    [SerializeField]
    private GameObject errorMessageDisplayObj;
    [SerializeField]
    private TextMeshProUGUI ErrorMessage;

    static Queue<ErrorLog> logs = new Queue<ErrorLog>();
    bool inMessage = false;

    ErrorLog logToDisplay = null;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(ErrorHandlerRoutine());
    }

    IEnumerator ErrorHandlerRoutine()
    {
        while (true)
        {
            if (logs.Count > 0 && inMessage == false)
            {
                inMessage = true;
                logToDisplay = logs.Dequeue();
                DisplayErrorMessage(logToDisplay);
            }
            else
            {
                inMessage = false;
                GameManager.ErrorDetected = false;
            }

            yield return null;
        }
    }

    public void Continue()
    {
        GameManager.ErrorDetected = false;
        errorMessageDisplayObj.SetActive(false);
    }

    public static void PushError(ErrorLog error)
    {
        GameManager.ErrorDetected = true;
        logs.Enqueue(error);
    }

    void DisplayErrorMessage(ErrorLog message)
    {
        GameManager.ErrorDetected = true;
        errorMessageDisplayObj.SetActive(true);
        ErrorMessage.text = message.Exception.Message;
    }
}
