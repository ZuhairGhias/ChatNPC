using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPTManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(GetModelInfo("gpt-3.5-turbo"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetModelInfo(string name)
    {
        Debug.Log("Fetching Model");    
        var api = new OpenAIClient(new OpenAIAuthentication().LoadFromEnvironment());

        var messages = new List<Message>
        {
            new Message(Role.System, "You are a helpful assistant."),
            new Message(Role.User, "Who won the world series in 2020?"),
            new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
            new Message(Role.User, "Where was it played?"),
        };

        var chatRequest = new ChatRequest(messages, Model.GPT3_5_Turbo);
        var response = api.ChatEndpoint.GetCompletionAsync(chatRequest);

        while (response.IsCompleted == false)
        {
            yield return new WaitForSeconds(1);
        }
        Debug.Log(response.Result.ToString());

        var choice = response.Result.FirstChoice;
        Debug.Log($"[{choice.Index}] {choice.Message.Role}: {choice.Message} | Finish Reason: {choice.FinishReason}");
    }
}
