using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class MyDialog : MonoBehaviour {
    public TextMeshProUGUI Caption, Question, YesText;
    UnityAction yesAction, noAction;
    public void Show(string caption, string question, UnityAction yesAction, UnityAction noAction = null) {
        Show(yesAction, noAction);
        Caption.text = caption;
        Question.text = question;
    }
    public void Show(string caption, string question, string yesText, UnityAction yesAction, UnityAction noAction = null) {
        Show(yesAction, noAction);
        YesText.text = yesText;
        Caption.text = caption;
        Question.text = question;
    }
    public void Show(UnityAction yesAction, UnityAction noAction = null) {
        this.yesAction = yesAction;
        this.noAction = noAction;
        gameObject.SetActive(true);
    }
    public void Yes() {
        gameObject.SetActive(false);
        yesAction.Invoke();
    }
    public void No() {
        gameObject.SetActive(false);
        if (noAction != null) noAction.Invoke();
    }
}
