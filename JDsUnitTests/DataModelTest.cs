using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JDsDataModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace JDsUnitTests
{
    [TestClass]
    public class DataModelTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var db = new JDsDBEntities())
            {
                int count = db.Employees.Count();

                var results = db.Skills.Select(x => x.Name).ToList();

                var emp = new Employee()
                {
                    Name = "TOM"
                };

                db.Employees.Add(emp);
                db.SaveChanges();

                var newCount = db.Employees.Count();

                Assert.AreEqual(count + 1, newCount);

                db.Employees.Remove(emp);

                db.SaveChanges();
            }
        }


        [TestMethod]
        public void TestMethod2()
        {
            // look at the time sheet entries and update them to make sure that every numHoursWorked
            // corresponds to the start/end times

            using (var db = new JDsDBEntities())
            {
                db.TimesheetEntries.ToList().ForEach(x =>
                {
                    if (x.StartTime.HasValue && x.EndTime.HasValue)
                    {
                        var newNumHoursWorked = x.EndTime - x.StartTime;
                        x.NumHoursWorked = (decimal)newNumHoursWorked.Value.TotalHours;
                    }
                });

                db.SaveChanges();
            }
        }
    }
}
