﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams.Internal;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class CardActionsBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //await turnContext.SendActivityAsync(MessageFactory.Text($"MessagingExtensions echo: {turnContext.Activity.Text}"), cancellationToken);

            var connectorClient = turnContext.TurnState.Get<IConnectorClient>();
            var teamsConnectorClient = new TeamsConnectorClient(connectorClient.Credentials);
            teamsConnectorClient.BaseUri = new Uri(turnContext.Activity.ServiceUrl);
            var teamsContext = new TeamsContext(turnContext, teamsConnectorClient);

            string actualText = teamsContext.GetActivityTextWithoutMentions();
            if (actualText.Equals("Cards", StringComparison.OrdinalIgnoreCase))
            {
                // Demo card 1 - adaptive card with bot bulder actions
                AdaptiveCards.AdaptiveCard adaptiveCard = new AdaptiveCards.AdaptiveCard();
                adaptiveCard.Body.Add(new AdaptiveCards.AdaptiveTextBlock("Bot Builder actions"));

                var action1 = new CardAction("imback", "imBack", null, null, null, "text");
                var action2 = new CardAction("messageBack", "message back", null, "text received by bots", "text display to users", JObject.Parse(@"{ ""key"" : ""value"" }"));
                var action3 = new CardAction("invoke", "invoke", null, null, null, JObject.Parse(@"{ ""key"" : ""value"" }"));
                var action4 = new CardAction("openUrl", "Buckethead", null, null, null, "https://en.wikipedia.org/wiki/Buckethead");
                var action5 = new CardAction("invoke", "a special invoke", null, "text on an invoke", "display text", JObject.Parse(@"{ ""key"" : ""value"" }"));
                adaptiveCard.Actions.Add(action1.ToAdaptiveCardAction());
                adaptiveCard.Actions.Add(action2.ToAdaptiveCardAction());
                adaptiveCard.Actions.Add(action3.ToAdaptiveCardAction());
                adaptiveCard.Actions.Add(action4.ToAdaptiveCardAction());
                adaptiveCard.Actions.Add(action5.ToAdaptiveCardAction());

                // Task module action
                var taskModuleAction = new TaskModuleAction("Launch Task Module", @"{ ""hiddenKey"": ""hidden value from task module launcher"" }");

                // Demo card 2 - launch task module from adaptive card
                AdaptiveCards.AdaptiveCard taskModuleCard1 = new AdaptiveCards.AdaptiveCard();
                taskModuleCard1.Body.Add(new AdaptiveCards.AdaptiveTextBlock("Task Module Adaptive Card"));
                taskModuleCard1.Actions.Add(taskModuleAction.ToAdaptiveCardAction());

                // Demo card 3 - launch task module from hero card (or any bot-builder framework card)
                HeroCard taskModuleCard2 = new HeroCard("Launch Task Module", null, null, null, new List<CardAction> { taskModuleAction });


                var replyActivity = MessageFactory.Attachment(new Attachment[] { adaptiveCard.ToAttachment(), taskModuleCard1.ToAttachment(), taskModuleCard2.ToAttachment() });

                await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"You said: {turnContext.Activity.Text}"), cancellationToken);
            }
        }

        protected override Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return base.OnInvokeActivityAsync(turnContext, cancellationToken);
        }
    }
}