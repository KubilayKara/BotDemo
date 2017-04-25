using JiraSimulation;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotFrameworkDemo.Dialogs
{
    [Serializable]
    public class SimpleJiraDialog : IDialog<object>
    {

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        Issue _currentIssue;
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Contains("Help"))
            {
                await context.PostAsync($"You Can \"List\",\"Select\" and \"Delete\" issues.");
                context.Wait(MessageReceivedAsync);

            }
            else if (activity.Text.StartsWith("List"))
            {
                var mng = JiraSimulationManager.GetInstance();
                string msg = "Issue List";
                foreach (var item in mng.GetIssues())
                {
                    msg = msg + string.Format($"\n Code:{item.IssueCode}\tTitle:{item.Title}");
                }
                await context.PostAsync(msg);
                context.Wait(MessageReceivedAsync);

            }
            else if (activity.Text.StartsWith("Select"))
            {
                string issueCode = activity.Text.Split(' ')[1];
                var selectedIssue = JiraSimulationManager.GetInstance().GetIssue(issueCode);

                if (selectedIssue == null)
                    await context.PostAsync("There is not a valid Issue");
                else
                {
                    this._currentIssue = selectedIssue;
                    string msg = string.Format($"\n Code:{selectedIssue.IssueCode}\tTitle:{selectedIssue.Title}\t Content:{selectedIssue.Content}");
                    await context.PostAsync(msg);

                }
                context.Wait(MessageReceivedAsync);

            }
            else if (activity.Text.StartsWith("Delete"))
            {

                #region Choice
                //List<string> issues = JiraSimulationManager.GetInstance().GetIssues().Select(i=> $"[{i.IssueCode}] {i.Title}").ToList()  ;
                //PromptOptions<string> options = new PromptOptions<string>("Select issue to remove.",
                //    "Sorry please try again", "ok, never mind..", issues, 2);
                //PromptDialog.Choice<string>(context, RemoveIssueAsync, options);


                #endregion

                if (_currentIssue == null)
                    await context.PostAsync("There is not a selected Issue");
                else
                {
                    ////Consfirm
                    //PromptDialog.Confirm(context, RemoveIssueAsync, $"Are you sure you want to Delete {_currentIssue.IssueCode}?");

                    bool removeResult = JiraSimulationManager.GetInstance().RemoveIssue(this._currentIssue.IssueCode);
                    if (removeResult)
                        await context.PostAsync($"Issue[{this._currentIssue.IssueCode}] removed successfully.");
                    else
                        await context.PostAsync($"Cannot remove Issue[{this._currentIssue.IssueCode}].");

                }
                context.Wait(MessageReceivedAsync);

            }
            else
            {
                await context.PostAsync("Sorry, I didn't get that...");

                context.Wait(MessageReceivedAsync);
            }
        }
        #region Confirm
        ///Confirm
        //private async Task RemoveIssueAsync(IDialogContext context, IAwaitable<bool> result)
        //{
        //    if (await result)
        //    {
        //        bool removeResult = JiraSimulationManager.GetInstance().RemoveIssue(this._currentIssue.IssueCode);
        //        if (removeResult)
        //            await context.PostAsync($"Issue[{this._currentIssue.IssueCode}] removed successfully.");
        //        else
        //            await context.PostAsync($"Cannot remove Issue[{this._currentIssue.IssueCode}].");
        //    }
        //    else
        //    {
        //        await context.PostAsync($"Operation Cancelled");
        //    }
        //  //  context.Wait(MessageReceivedAsync);
        //} 
        #endregion


        #region Choice
        //private async Task RemoveIssueAsync(IDialogContext context, IAwaitable<string> result)
        //{
        //    string selection = await result;
        //    string issueCode = selection.Split('[',']')[1];
        //    bool removeResult = JiraSimulationManager.GetInstance().RemoveIssue(issueCode);
        //    if (removeResult)
        //        await context.PostAsync($"Issue[{issueCode}] removed successfully.");
        //    else
        //        await context.PostAsync($"Cannot remove Issue[{issueCode}].");
        //    context.Wait(MessageReceivedAsync);
        //} 
        #endregion
    }
}