﻿using StellarChat.Shared.Contracts.Assistants;

namespace StellarChat.Client.Web.State;

public class ChatState
{
    public Guid ChatId { get; set; }
    public event Action? ChatIdChanged;

    public void SetChatId(Guid chatId)
    {
        ChatId = chatId;
        ChatIdChanged?.Invoke();
    }

    public string SelectedModel { get; set; } = string.Empty;
    public event Action? SelectedModelChanged;

    public void SetSelectedModel(string selectedModel)
    {
        SelectedModel = selectedModel;
        SelectedModelChanged?.Invoke();
    }

    public string UserName { get; set; } = string.Empty;
    public event Action? UserNameChanged;

    public void SetUserName(string userName)
    {
        UserName = userName;
        UserNameChanged?.Invoke();
    }

    public string UserAvatar { get; set; } = string.Empty;
    public event Action? UserAvatarChanged;

    public void SetUserAvatar(string userAvatar)
    {
        UserAvatar = userAvatar;
        UserAvatarChanged?.Invoke();
    }

    public AssistantResponse? SelectedAssistant { get; set; }
    public event Action? SelectedAssistantChanged;

    public void SetSelectedAssistant(AssistantResponse assistant)
    {
        SelectedAssistant = assistant;
        SelectedAssistantChanged?.Invoke();
    }

    public event Action? AssistantUpdated;

    public void NotifyAssistantUpdated()
    {
        AssistantUpdated?.Invoke();
    }
}
