using UnityEngine;

public class ConversationUITester : MonoBehaviour
{
    public ConversationUIController ui;
    public GameObject traitWindow;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ui.Open("또 왔구만.\n이번에도 새로운 가능성을 열어볼텐가?",
                yes: () => { if (traitWindow) traitWindow.SetActive(true); },
                no: () => { /* "그래. 잘 가게나 */ });
        }
        if (Input.GetKeyDown(KeyCode.F2))
            ui.Close();
    }
}
