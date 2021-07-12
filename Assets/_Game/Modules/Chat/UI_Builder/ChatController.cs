using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Entities;

public class ChatController : MonoBehaviour
{
    private Button sendButton;
    private Label labelText;
    private ScrollView scrollView;
    private TextField chatInputField;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        sendButton = root.Q<Button>("send-button");
        labelText = root.Q<Label>("label-text");
        chatInputField = root.Q<TextField>("chat-input");

        //chatInputField
        sendButton.clicked += SendButtonPressed;
    }

    void SendButtonPressed()
    {
        labelText.text = chatInputField.text;

        Net_ChatMessage chatMessage = new Net_ChatMessage(labelText.text);
        chatMessage.Send();

        chatInputField.value = "";
    }
}
