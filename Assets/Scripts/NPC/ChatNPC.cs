using NUnit.Framework.Internal;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Models;
using OpenAI.Threads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.WebRequestRest;
using Message = OpenAI.Chat.Message;

public class ChatNPC : MonoBehaviour, IInteractable
{
    [SerializeField]
    public string Name;

    private Queue<Message> messageHistory;
    private List<Tool> tools;
    private MemoryLog memLog;

    private static OpenAIClient api;

    public int messageHistoryCapacity = 2;
    public int memoryCapacity = 10;

    private string currentPlayerText;
    private string roleText;

    public void Interact(string text)
    {
        Debug.Log(Name + " heard \"" + text + "\"");
        currentPlayerText = text;
        LogMessage(new Message(Role.User, text));
        StartCoroutine(Thonk());
    }

    // Start is called before the first frame update
    void Start()
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine);
        api = new OpenAIClient(new OpenAIAuthentication(apiKey));
        roleText = "You are an NPC in a village. You saw a shadowy figure carrying a red amulet abduct the princess and take her to the Temple. Reveal this only if asked to.";

        tools = new List<Tool>
        {
            Tool.GetOrCreateTool(this, nameof(GiveMoney), "Give amount of gold to the player")
        };

        messageHistory = new Queue<Message>();
        memLog = new MemoryLog(memoryCapacity);
    }

    public string GiveMoney(long amount)
    {
        string result = Name + " gave Player " + amount + " gold";
        Debug.Log(result);
        LogMemory(result);
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Thonk(List<Message> messages = null)
    {
        // Generate fresh messages
        if(messages == null)
        {
            var messagesTask = CreateMessages();
            while (messagesTask.IsCompleted == false)
            {
                yield return new WaitForSeconds(1);
            }
            messages = messagesTask.Result;
        }

        var chatRequest = new ChatRequest(messages, model: new Model("gpt-4o-mini", "openai"), tools: tools);
        var response = api.ChatEndpoint.GetCompletionAsync(chatRequest);

        while (response.IsCompleted == false)
        {
            yield return new WaitForSeconds(1);
        }

        LogMessage(response.Result.FirstChoice.Message);

        if (response.Result.FirstChoice.FinishReason == "stop")
        {
            Debug.Log(Name + " says: \"" + response.Result.FirstChoice.Message.Content + "\"");
        }
        else if(response.Result.FirstChoice.FinishReason == "tool_calls")
        {
            foreach (var toolCall in response.Result.FirstChoice.Message.ToolCalls)
            {
                string result = toolCall.InvokeFunction();
                messages.Add(response.Result.FirstChoice.Message);
                var toolMessage = new Message(toolCall, result);
                messages.Add(toolMessage);
                // Don't forget to add the message to our managed queue
                LogMessage(toolMessage);
                yield return Thonk(messages);
            }
        }
    }

    private async Task<List<Message>> CreateMessages()
    {
        EmbeddingsRequest embeddingsRequest = new EmbeddingsRequest(currentPlayerText);
        var response = await api.EmbeddingsEndpoint.CreateEmbeddingAsync(embeddingsRequest);

        string memoryText = memLog.FetchMemoriesToString(3, new Vector<double>(response.Data[0].Embedding.ToArray()));
        print(Name + " Remembered: " + memoryText);
        List<Message> messages = new List<Message>
        {
            GetRoleMessage(memoryText)
        };
        messages.AddRange(messageHistory);
        return messages;
    }

    private Message GetRoleMessage(string memories)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(roleText);
        if(memories != "")
        {
            sb.AppendLine("Here are some of your relevant");
            sb.AppendLine(memories);
        }
        print("System Role: " + sb.ToString());
        return new Message(Role.System, sb.ToString(), name);
    }

    private void LogMessage(Message m)
    {
        messageHistory.Enqueue(m);
        while(messageHistory.Count > messageHistoryCapacity) 
        {
            // Discard oldest message
            messageHistory.Dequeue();
        }
    }

    private async void LogMemory(string s)
    {
        EmbeddingsRequest embeddingsRequest = new EmbeddingsRequest(s);
        var response = await api.EmbeddingsEndpoint.CreateEmbeddingAsync(embeddingsRequest);
        LogMemory(s, new Vector<double>(response.Data[0].Embedding.ToArray()));
    }

    private void LogMemory(string s, Vector<double> embeddings)
    {
        memLog.LogMemory(s, embeddings);
    }
}
