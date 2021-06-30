using UnityEngine;
using UnityEngine.UIElements;
using Unity.Entities;

public class ChatController : MonoBehaviour
{
    public Button sendButton;
    public Label labelText;
    public ScrollView scrollView;
    public TextField chatInputField;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        sendButton = root.Q<Button>("send-button");
        labelText = root.Q<Label>("label-text");
        chatInputField = root.Q<TextField>("chat-input");

        //chatInputField
        sendButton.clicked += SendButtonPressed;



        labelText.text = "HolaMundo";
    }

    void SendButtonPressed()
    {
        labelText.text = chatInputField.text;
        //chatInputField.text = "";
    }
}
