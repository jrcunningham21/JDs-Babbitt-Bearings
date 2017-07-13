using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JD_s_Babbitt_Bearings.Models
{
    public class MockJob
    {

        public int JobID { get; set; }
        public string Customer { get; set; }
        public DateTime DueDate { get; set; }
        public string JobStatus { get; set; }
    }
}