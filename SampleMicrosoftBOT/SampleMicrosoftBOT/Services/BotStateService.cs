using Microsoft.Bot.Builder;
using SampleMicrosoftBOT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleMicrosoftBOT.Services
{
    public class BotStateService
    {
        public BotStateService(UserState userState, ConversationState conversationState)
        {
            UserState = userState ?? throw new Exception();
            ConversationState = conversationState;
            Initialze();
        }

        public UserState UserState { get; }
        public ConversationState ConversationState { get; }

        public static string UserProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";
        public static string ConversationStateId { get; } = $"{nameof(BotStateService)}.ConversationState";

        public IStatePropertyAccessor<UserModel> UserModelAccessor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationStateAccessor { get; set; }


        public void Initialze()
        {
            UserModelAccessor = UserState.CreateProperty<UserModel>(UserProfileId);
            ConversationStateAccessor = ConversationState.CreateProperty<ConversationData>(ConversationStateId);
        }
    }
}
