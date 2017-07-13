using JD_s_Babbitt_Bearings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings.Controllers
{
    public class CurrentJobsController : Controller
    {
        //
        // GET: /CurrentJobs/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetJobs(GridRequest request)
        {
            var mockJobs = new List<MockJob>();

            var mockJob1 = new MockJob();
            mockJob1.JobID = 3210;
            mockJob1.Customer = "ACME";
            mockJob1.DueDate = DateTime.Today;
            mockJob1.JobStatus = "In Progress";
            mockJobs.Add(mockJob1);

            var mockJob2 = new MockJob();
            mockJob2.JobID = 6713;
            mockJob2.Customer = "Haliburton";
            mockJob2.DueDate = DateTime.Today.AddDays(1);
            mockJob2.JobStatus = "Not Started";
            mockJobs.Add(mockJob2);

            var mockJob3 = new MockJob();
            mockJob3.JobID = 2146;
            mockJob3.Customer = "Wesson";
            mockJob3.DueDate = DateTime.Today.AddDays(-1);
            mockJob3.JobStatus = "Complete";
            mockJobs.Add(mockJob3);

            var result = new GridResult();
            result.current = request.current;
            result.rowCount = request.rowCount;
            result.total = 3;
            result.rows = mockJobs;

            return Json(result);
        }
        
        public JsonResult GridResult<T>(GridRequest request, List<T> list)
        {

            return Json("");
        }
    }

    public class GridResult
    {
        public int current { get; set; }
        public int rowCount { get; set; }
        public int total { get; set; }
        public List<MockJob> rows { get; set; }
    }

    public class GridRequest
    {
        public int current { get; set; }
        public int rowCount { get; set; }
        public Dictionary<string, string> sort { get; set; }
        public string searchPhrase { get; set; }
    }

}
