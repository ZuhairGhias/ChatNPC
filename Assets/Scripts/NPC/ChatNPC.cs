using NUnit.Framework.Internal;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using OpenAI.Threads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Message = OpenAI.Chat.Message;

public class ChatNPC : MonoBehaviour, IInteractable
{
    [SerializeField]
    public string Name;

    private List<Message> messages;
    private List<Tool> tools;

    private static OpenAIClient api;

    public void Interact(string text)
    {
        Debug.Log(Name + " heard \"" + text + "\"");
        messages.Add(new(Role.User, text));
        StartCoroutine(Thonk());
    }

    // Start is called before the first frame update
    void Start()
    {
        api = new OpenAIClient(new OpenAIAuthentication().LoadFromEnvironment());
        messages = new List<Message>
        {
            new Message(Role.System, "You are an NPC in a village. You saw a shadowy figure carrying a red amulet abduct the princess and take her to the Temple. Reveal this only if asked to.")
        };

        tools = new List<Tool>
        {
            Tool.GetOrCreateTool(this, nameof(GiveMoney), "Give amount of gold to the player")
        };
    }

    public string GiveMoney(Int64 amount)
    {
        string result = Name + " gave you " + amount + " gold";
        Debug.Log(result);
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Thonk()
    {
        var chatRequest = new ChatRequest(messages, model: Model.GPT3_5_Turbo, tools: tools);
        var response = api.ChatEndpoint.GetCompletionAsync(chatRequest);

        while (response.IsCompleted == false)
        {
            yield return new WaitForSeconds(1);
        }

        messages.Add(response.Result.FirstChoice.Message);

        if (response.Result.FirstChoice.FinishReason == "stop")
        {
            Debug.Log(Name + " says: \"" + response.Result.FirstChoice.Message.Content + "\"");
        }
        else if(response.Result.FirstChoice.FinishReason == "tool_calls")
        {
            foreach (var toolCall in response.Result.FirstChoice.Message.ToolCalls)
            {
                string result = toolCall.InvokeFunction();
                messages.Add(new Message(toolCall, result));
                yield return Thonk();
            }
        }
    }
}
