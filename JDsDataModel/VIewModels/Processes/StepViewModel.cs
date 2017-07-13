using JDsDataModel.ViewModels.Processes.ProcessBabbittBearing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace JDsDataModel.ViewModels.Processes
{
    [Serializable]
    public class StepViewModel
    {
        public string Version { get; set; }

        [DisplayName("Customer:")]
        public string CustomerLabel { get; set; }

        [DisplayName("Contact:")]
        public string ContactLabel { get; set; }

        [DisplayName("Work:")]
        public string JobSummary { get; set; }

        [DisplayName("Job:")]
        public int JobId { get; set; }

        [DisplayName("Process:")]
        public long ProcessId { get; set; }

        [DisplayName("Step:")]
        public long StepId { get; set; }

        [DisplayName("Part:")]
        public int PartId { get; set; }

        [DisplayName("UT Required:")]
        public bool IsUTRequired { get; set; }

        [DisplayName("PT Required:")]
        public bool IsPTRequired { get; set; }

        public bool IsDisplayUT { get; set; }

        public bool IsDisplayPT { get; set; }

        [DisplayName("Step Completed:")]
        public bool IsCompleted { get; set; }

        public string StepData { get; set; }

        public bool IsInsulated { get; set; }

        /// <summary>
        /// Used to keep track if the response is to only save or save and continue to the next step.
        /// Default is false and should not be serialized since it's only used for request routing.
        /// </summary>
        [JsonIgnore]
        public bool IsOnlySave { get; set; }

        /// <summary>
        /// Set the Customer Label, the Contact Label, and the Job Summary
        /// (the bits that get displayed at the top of each workflow screen
        /// </summary>
        /// <param name="processId"></param>
        public void InitJobInfoFromProcessId(long processId)
        {
            using (var db = new JDsDBEntities())
            {
                var proc = db.Processes.FirstOrDefault(x => x.ProcessId == processId);
                var part = proc?.Part;
                var job = proc?.Part?.Job;
                var contact = proc?.Part?.Job?.Contact;
                var customer = proc?.Part?.Job?.Contact?.Customer;

                // Fill out the customer label
                if (customer != null)
                {
                    // format: JD's job number - Customer Company name
                    this.CustomerLabel = string.Format("{0} {1}", job.JobId, customer.CompanyName);
                }

                // Fill out the contact label
                if (contact != null)
                {
                    // format: name+ number
                    string num = contact.CellPhone;
                    if (string.IsNullOrEmpty(num))
                        num = contact.WorkPhone;
                    if (string.IsNullOrEmpty(num))
                        num = contact.Fax;

                    this.ContactLabel = string.Format("{0} {1} {2}", contact.FirstName, contact.LastName, num);
                }

                // Fill out the part job summary
                // format: itemcode / scope / id
                if (part != null)
                {
                    var itemCode = string.IsNullOrEmpty(part.ItemCode) ? "n/a" : part.ItemCode;
                    var scope = string.IsNullOrEmpty(part.WorkScope) ? "n/a" : part.WorkScope;
                    var id = string.IsNullOrEmpty(part.IdentifyingInfo) ? "n/a" : part.IdentifyingInfo;

                    this.JobSummary = string.Format("{0} / {1} / {2}", itemCode, scope, id);
                }
                

                var Steps = db.Steps.Where(s => s.ProcessId == processId);
                
                //StepData keep track of completed steps
                var stepData = string.Empty;
                const int stepCount = 13;
                for (int i = 1; i <= stepCount; i++)
                {
                    if(Steps.Any(f => f.StepNumber == i))
                    {
                        var Step = Steps.Where(s => s.StepNumber == i).FirstOrDefault();
                        if (Step.StringValue == null)
                            stepData += "false, ";
                        else
                        {
                            dynamic stepInfo = JsonConvert.DeserializeObject(Step.StringValue);
                            if (stepInfo.IsCompleted == false)
                            {
                                stepData += "false, ";
                            }
                            else
                            {
                                stepData += "true, ";
                            }
                        }
                    }
                    else
                    {
                        stepData += "false, ";
                    }
                    //stepData += finishedSteps.Any(f => f.StepNumber == i) ? "true, " : "false, ";
                }

                StepData = "[" + stepData + "]";

                // carry the insulated flag on step 1 across steps
                var step1Data = Steps.FirstOrDefault(s => s.StepNumber == 1)?.StringValue ?? "false";

                Regex re = new Regex(@"""IsInsulated"": true");

                IsInsulated = re.IsMatch(step1Data);
            }
        }
    }
}
