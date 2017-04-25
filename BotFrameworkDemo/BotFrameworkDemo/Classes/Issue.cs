using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSimulation
{
    [Serializable]
    public class Issue
    {
        //[Prompt("Please enter an unused IssiueCode")]
        //[Describe("Unique Issue Code")]
        public string IssueCode { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Deadline { get; set; }
        public string AssigneeCode { get; set; }
        public IssueStatus Status { get; set; }


        #region Form Dialog
        public static IForm<Issue> MakeForm()
        {
            FormBuilder<Issue> order = new FormBuilder<Issue>();
            return order
                .Message("Enter the issue fields")
                .AddRemainingFields()
                .Build();
                //.Field(nameof(IssueCode), validate: IssueCodeValidator)
                //.Field(nameof(Title))
                //.AddRemainingFields()
                //.OnCompletion(async (session, issue) =>
                //{
                //    JiraSimulationManager.GetInstance().AddIssue(issue);
                //})
                //    .Message("Thank you, I have submitted your message.")
                //    .Build();
        }

        private static ValidateAsyncDelegate<Issue> IssueCodeValidator = async (state, response) =>
        {
            var result = new ValidateResult { IsValid = true, Value = response };
            if (JiraSimulationManager.GetInstance().ValidateIssueCode(response as string))
            {
                result.Feedback = string.Format(@"ok");
                result.IsValid = true;
            }
            else
            {
                result.Feedback = string.Format(@"This issueCode is already included.");
                result.IsValid = false;
            }

            return await Task.FromResult(result);
        };
        #endregion
    }

    public  enum IssueStatus
    {
        None,
        Open,
        Inprogress,
        Closed
    }
}
