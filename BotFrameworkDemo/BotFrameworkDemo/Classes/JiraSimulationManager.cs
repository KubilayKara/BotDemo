using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSimulation
{
    public class JiraSimulationManager
    {

        private List<Issue> _issueListList;


        public static JiraSimulationManager GetInstance()
        {
            if (_instance == null)
                _instance = new JiraSimulationManager();
            return _instance;
        }

        private static JiraSimulationManager _instance;
        private JiraSimulationManager()
        {
            //Issues
            this._issueListList = new List<Issue>();
            this._issueListList.Add(new Issue { IssueCode = "001", Title = "OOM Inceleme", Content = "Tabletlerde OOM alan ekranları inceleyelim", AssigneeCode = "T32081", Status = IssueStatus.Inprogress, Deadline = new DateTime(2017, 6, 1) });
            this._issueListList.Add(new Issue { IssueCode = "002", Title = "Style", Content = "Style dosyalarının branchapp'a taşınması", AssigneeCode = "T32081", Status = IssueStatus.Inprogress, Deadline = new DateTime(2017, 9, 10) });
            this._issueListList.Add(new Issue { IssueCode = "003", Title = "Region gibi userControl", Content = "Region gibi çalışan bir user control altyapısı sağlayalım.", AssigneeCode = "T32081", Status = IssueStatus.Inprogress, Deadline = new DateTime(2017, 10, 15) });
            this._issueListList.Add(new Issue { IssueCode = "004", Title = "Core Next Next Setup", Content = "Next Next ile kuralabilecek bir code ortamı ve configusarsyon DB'si", AssigneeCode = "T05999", Status = IssueStatus.Inprogress, Deadline = new DateTime(2017, 8, 20) });
            this._issueListList.Add(new Issue { IssueCode = "005", Title = "2018 Hedef", Content = "Kişisel deflerin belirlenmesi.", AssigneeCode = "T05999", Status = IssueStatus.Inprogress, Deadline = new DateTime(2017, 9, 1) });

        }


        public List<Issue> GetIssues()
        {
            return this._issueListList.ToList();
        }

        public List<Issue> GetIssues(string accountCode)
        {
            return this._issueListList.Where(i => i.AssigneeCode == accountCode).ToList();
        }

        public void AddIssue(Issue issue)
        {
            this._issueListList.Add(issue);
        }
        public bool RemoveIssue(string issueCode)
        {
            var issue = this.GetIssue(issueCode);
            if (issue == null)
                return false;
            this._issueListList.Remove(issue);
            return true;
        }
        public bool ValidateIssueCode(string issueCode)
        {
            return !this._issueListList.Any(i => i.IssueCode == issueCode);
        }


        public Issue GetIssue(string issueCode)
        {
            return this._issueListList.FirstOrDefault(i => i.IssueCode == issueCode);
        }
    }
}
