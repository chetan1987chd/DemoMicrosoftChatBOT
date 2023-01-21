using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using SampleMicrosoftBOT.Models;
using SampleMicrosoftBOT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleMicrosoftBOT.Bots
{
    public class GreetingBot : ActivityHandler
    {
        private readonly BotStateService _stateService;

        public GreetingBot(BotStateService stateService)
        {
            _stateService = stateService;
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserModel userModel = await _stateService.UserModelAccessor.GetAsync(turnContext, () => new UserModel());
            ConversationData conversationData = await _stateService.ConversationStateAccessor.GetAsync(turnContext, () => new ConversationData());

            if (!string.IsNullOrEmpty(userModel.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Hi {0}, how are you doing today ?", userModel.Name)), cancellationToken);
            }
            else
            {
                if(conversationData.PromptedUserForName)
                {
                    userModel.Name = turnContext.Activity.Text?.Trim();

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thankyou {userModel.Name}. How can I help you today>"), cancellationToken);

                    conversationData.PromptedUserForName = false;
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Whats your name ?"), cancellationToken);
                    conversationData.PromptedUserForName = true;
                }

                //save change
                await _stateService.UserModelAccessor.SetAsync(turnContext, userModel);
                await _stateService.ConversationStateAccessor.SetAsync(turnContext, conversationData);

                await _stateService.UserState.SaveChangesAsync(turnContext);
                await _stateService.ConversationState.SaveChangesAsync(turnContext);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
