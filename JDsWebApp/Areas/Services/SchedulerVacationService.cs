
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using System;
using System.Data;
using JDsWebApp.Areas.EmployeePortal.Models.Vacation;
using System.Collections.Generic;
using System.Data.Entity;
using JDsDataModel;
using NLog;

namespace JDsWebApp.Areas.Services
{
    public class SchedulerVacationService : ISchedulerEventService<VacationViewModel>
    {        
        public virtual IQueryable<VacationViewModel> GetAll()
        {
            var list = new List<VacationViewModel>();
            using (var db = new JDsDBEntities())
            {
                var allVacations = db.Vacations.ToList();

                foreach (var v in db.Vacations)
                {
                    var vm = new VacationViewModel()
                    {
                        Title = v.Employee.Name,
                        EmployeeId = v.EmployeeId,
                        VacationEntryId = v.VacationId,
                        Start = v.StartDate,
                        End = v.StartDate.Equals(v.EndDate) ? v.EndDate : v.EndDate.AddMilliseconds(1),
                        IsFullVacationDay = v.NumberOfVacationHoursUsed > 4,
                        IsAllDay = v.NumberOfVacationHoursUsed == 8,
                        NumHours = v.NumberOfVacationHoursUsed,
                        IsVacationTime = v.IsVacationTime,
                        SignOffName = ""
                    };
                    list.Add(vm);
                }
            }
            return list.AsQueryable();           
        }

        public virtual void Insert(VacationViewModel task, ModelStateDictionary modelState)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                using (var db = new JDsDBEntities())
                {
                    Employee employee = db.Employees.Single(x => x.EmployeeId == task.EmployeeId);

                    //If there is an overlapping conflict don't save it.
                    if (db.Vacations.Any(
                            x => x.EmployeeId == task.EmployeeId && x.StartDate < task.End && task.Start < x.EndDate) && string.IsNullOrWhiteSpace(task.SignOffName))
                    {
                        throw new Exception("There is a overlapping conflict with and existing vacation");
                    }

                    if (task.Start > task.End)
                    {
                        throw new Exception("Start date cannnot be after the end date");
                    }

                    int numberOfHoursUsed = 0;
                    if (!VacationCalculator.HasAvaliableVacationHours(task.EmployeeId, task.Start, task.End, task.IsVacationTime, out numberOfHoursUsed) && task.SignOffId == null)
                    {
                        throw new Exception("The employee does not have enough vacation hours");
                    }

                    //Creating a new vacation
                    Vacation vacation = new Vacation();

                    vacation.StartDate = task.Start;
                    vacation.EndDate = task.End;
                    vacation.Day = DateTime.Now;
                    vacation.EmployeeId = task.EmployeeId;
                    vacation.VacationSignOffId = task.SignOffId;
                    vacation.IsVacationTime = task.IsVacationTime;
                    vacation.NumberOfVacationHoursUsed = numberOfHoursUsed;

                    db.Vacations.Add(vacation);
                    db.SaveChanges();
                    logger.Trace("Vacation insert success.");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Vacation insert failed:" + ex.Message);
            }
        }

        public virtual void Update(VacationViewModel task, ModelStateDictionary modelState)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                using (JDsDBEntities db = new JDsDBEntities())
                {
                    //If there is an overlapping conflict don't save it.
                    if (
                        db.Vacations.Any(
                            x => x.EmployeeId == task.EmployeeId && x.VacationId != task.VacationEntryId && x.StartDate < task.End && task.Start < x.EndDate) && !task.SignOffId.HasValue)
                    {
                        throw new Exception("There is a overlapping conflict with and existing vacation");
                    }

                    if (task.Start > task.End)
                    {
                        throw new Exception("Start date cannnot be after the end date");
                    }

                    int numberOfHoursUsed = 0;
                    if (!VacationCalculator.HasAvaliableVacationHours(task.EmployeeId, task.Start, task.End, task.IsVacationTime, out numberOfHoursUsed) && !task.SignOffId.HasValue)
                    {
                        throw new Exception("The employee does not have enough vacation hours");
                    }

                    var vacation = db.Vacations.FirstOrDefault(v => v.VacationId == task.VacationEntryId);
                    if (vacation != null)
                    {
                        vacation.StartDate = task.Start;
                        vacation.EndDate = task.End;
                        vacation.Day = DateTime.Now;
                        vacation.NumberOfVacationHoursUsed = VacationCalculator.GetNumberOfVacationHoursUsedForPossibleVacation(task.EmployeeId,
                            task.Start, task.End);
                        vacation.VacationSignOffId = task.SignOffId;
                        vacation.IsVacationTime = task.IsVacationTime;
                        vacation.EmployeeId = task.EmployeeId;


                        db.Entry(vacation).State = EntityState.Modified;
                    }
                    else
                    {
                        db.Vacations.Add(new Vacation
                        {
                            Day = DateTime.Now,
                            EmployeeId = task.EmployeeId,
                            NumberOfVacationHoursUsed = VacationCalculator.GetNumberOfVacationHoursUsedForPossibleVacation(task.EmployeeId,
                                task.Start, task.End),
                            VacationSignOffId = db.Employees.SingleOrDefault(x => x.Name == task.SignOffName)?.EmployeeId,
                            StartDate = task.Start,
                            EndDate = task.End
                    });
                    }
                       
                    db.SaveChanges();
                    logger.Trace("Vacation update success.");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Vacation update failed:" + ex.Message);
            }
        }

        public virtual void Delete(VacationViewModel task, ModelStateDictionary modelState)
        {
            using (var db = new JDsDBEntities())
            {
                try
                {
                    var vacationToDelete = db.Vacations.Where(v => v.VacationId == task.VacationEntryId).FirstOrDefault();
                    var employee = db.Employees.Where(e => e.EmployeeId == task.EmployeeId).FirstOrDefault();

                    db.Vacations.Remove(vacationToDelete);
                    
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Could not delete vacation - Error: " + e);
                }
            }
        }

        public void Dispose()
        {
            //db.Dispose();
        }
    }
}
