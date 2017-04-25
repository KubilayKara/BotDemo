using JiraSimulation;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotFrameworkDemo.Dialogs
{
    [LuisModel("1de929c1-b424-46a4-a644-edd3323b39a7", "7ad9e83d30154d0aa6061ff8ba0ba4bb")]
    [Serializable]
        public class SimpleJiraLuisDialog : LuisDialog<object>
    {

    

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You Can \"List\",\"Select\" and \"Delete\" issues.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("ListIssues")]
        public async Task ListIssues(IDialogContext context, LuisResult result)
        {
            var mng = JiraSimulationManager.GetInstance();
            string msg = "Issue List";
            foreach (var item in mng.GetIssues())
            {
                msg = msg + string.Format($"\n Code:{item.IssueCode}\tTitle:{item.Title}");
            }
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("DeleteIssue")]
        public async Task DeleteIssue(IDialogContext context, LuisResult result)
        {
            List<string> issues = JiraSimulationManager.GetInstance().GetIssues().Select(i => $"[{i.IssueCode}] {i.Title}").ToList();
            PromptOptions<string> options = new PromptOptions<string>("Select issue to remove.",
                "Sorry please try again", "ok, never mind..", issues, 2);
            PromptDialog.Choice<string>(context, RemoveIssueAsync, options);


        }

        private async Task RemoveIssueAsync(IDialogContext context, IAwaitable<string> result)
        {
            string selection = await result;
            string issueCode = selection.Split('[', ']')[1];
            bool removeResult = JiraSimulationManager.GetInstance().RemoveIssue(issueCode);
            if (removeResult)
                await context.PostAsync($"Issue[{issueCode}] removed successfully.");
            else
                await context.PostAsync($"Cannot remove Issue[{issueCode}].");
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I didn't get that...");

            context.Wait(MessageReceived);

        }

      
    }
}