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
    [LuisModel("ee414b02-840c-4193-acd4-a71d7d04fd91", "be69cba2b7b84cabb92732266e97ff92")]
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

        string _currentIssue;
        [LuisIntent("DeleteIssue")]
        public async Task DeleteIssue(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Count > 0)
            {
                string issueCode = result.Entities[0].Entity as string;
                _currentIssue = issueCode;
                PromptDialog.Confirm(context,AfterConfirmRemoveIssueAsync, $"Are you sure you want to Delete {issueCode}?");
            }
            else
            {
                List<string> issues = JiraSimulationManager.GetInstance().GetIssues().Select(i => $"[{i.IssueCode}] {i.Title}").ToList();
                PromptOptions<string> options = new PromptOptions<string>("Select issue to remove.",
                    "Sorry please try again", "ok, never mind..", issues, 2);
                PromptDialog.Choice<string>(context, RemoveIssueAsync, options);
            }

        }
        private async Task AfterConfirmRemoveIssueAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool selection = await result;
            if (await result)
            {
                bool removeResult = JiraSimulationManager.GetInstance().RemoveIssue(this._currentIssue);
                if (removeResult)
                    await context.PostAsync($"Issue[{this._currentIssue}] removed successfully.");
                else
                    await context.PostAsync($"Cannot remove Issue[{this._currentIssue}].");
            }
            else
            {
                await context.PostAsync($"Operation Cancelled");
            }
        }
        private async Task RemoveIssueAsync(IDialogContext context, IAwaitable<string> result)
        {
            string selection = await result;
            string issueCode = selection.Contains("[") ? selection.Split('[', ']')[1] : selection;
            bool removeResult = JiraSimulationManager.GetInstance().RemoveIssue(issueCode);
            if (removeResult)
                await context.PostAsync($"Issue[{issueCode}] removed successfully.");
            else
                await context.PostAsync($"Cannot remove Issue[{issueCode}].");
            //  context.Wait(MessageReceived);
        }



        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I didn't get that...");
            context.Wait(MessageReceived);

        }


    }
}