using JDsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDsWebApp.Areas.Services
{
    public static class VacationCalculator
    {
        /// <summary>
        /// Calculates the number of vacation hours given the start date and datetime of the year.
        /// </summary>
        /// <param name="start">Hire Date</param>
        /// <param name="comparisonDateTime">Year to get the number of vacation hours</param>
        /// <param name="isVacationTime"></param>
        /// <returns>Number of vacation hours avaliable of the year of the comparison date</returns>
        public static int CalculateVacationHours(DateTime? start, DateTime? comparisonDateTime = null, bool isVacationTime = true)
        {
            if (!start.HasValue)
                return 0;

            DateTime empStartDate = start.Value;
            DateTime endDate = comparisonDateTime ?? DateTime.Now;

            var elapsedTime = endDate - empStartDate;
            int oneYear = 365;
            int earnedVacation = 0;

            if (isVacationTime)
            {
                // Less than 1 year
                if (elapsedTime.TotalDays < oneYear)
                    earnedVacation = 0;

                // between 1 and 2 years
                else if (elapsedTime.TotalDays < 2*oneYear)
                {
                    earnedVacation = 40;
                }

                // between 2 and 6 years
                else if (elapsedTime.TotalDays < 6*oneYear)
                {
                    earnedVacation = 80;
                }

                // between 6 and 7 years
                else if (elapsedTime.TotalDays < 7*oneYear)
                {
                    earnedVacation = 88;
                }

                // between 7 and 8 years
                else if (elapsedTime.TotalDays < 8*oneYear)
                {
                    earnedVacation = 96;
                }

                // between 8 and 9 years
                else if (elapsedTime.TotalDays < 9*oneYear)
                {
                    earnedVacation = 104;
                }

                // between 9 and 10 years
                else if (elapsedTime.TotalDays < 10*oneYear)
                {
                    earnedVacation = 112;
                }

                // between more than 10
                else if (elapsedTime.TotalDays >= 10*oneYear)
                {
                    earnedVacation = 120;
                }
            }

            else
            {
                // Add into that, personal time
                // between 1 and 2 years
                if (elapsedTime.TotalDays < 2 * oneYear && elapsedTime.TotalDays > oneYear)
                {
                    earnedVacation += 8;
                }

                // after 2 years
                else if (elapsedTime.TotalDays >= 2 * oneYear)
                {
                    earnedVacation += 16;
                }
            }

            return earnedVacation;
        }

        /// <summary>
        /// Calculates the number of vacation hours used given the type, employee and date to use as a reference of which time period to use. If given a vacation ignores that vacation in the calculation.
        /// </summary>
        /// <param name="employeeId">ID of the employee to get the number of vacation hours used</param>
        /// <param name="yearReference">Date in between hire anniversaries</param>
        /// <param name="vacationID">ID of the vacation to ignore in the calculation of the hours used</param>
        /// <param name="isVacationTime"></param>
        /// <returns></returns>
        public static int CalculateHoursUsed(int employeeId, DateTime? yearReference = null, int vacationID = 0, bool isVacationTime = true)
        {
            int hoursUsedThisYear = 0;

            // Calulcate the number of hours used in the employee's current year, from the most recent anniversary of their hire date till now
            using (var db = new JDsDBEntities())
            {
                var employee = db.Employees.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (employee?.HireDate == null)
                {
                    return 0;
                }

                DateTime referenceDate = yearReference ?? DateTime.Now;

                DateTime startHireAnniversary = new DateTime(referenceDate.Year, employee.HireDate.Value.Month, employee.HireDate.Value.Day);
                DateTime endHireAnniversary = new DateTime(referenceDate.Year + 1, employee.HireDate.Value.Month, employee.HireDate.Value.Day);

                if (startHireAnniversary > referenceDate)
                {
                    startHireAnniversary = startHireAnniversary.AddYears(-1);
                    endHireAnniversary = endHireAnniversary.AddYears(-1);
                }

                //The easy calculation where the vacations are betweeen start hire anniversary and end hire
                var vacations = db.Vacations.Where(x => x.VacationId != vacationID &&
                                                        x.IsVacationTime == isVacationTime &&
                                                        x.StartDate >= startHireAnniversary &&
                                                        x.EndDate < endHireAnniversary &&
                                                        x.EmployeeId == employeeId);
                if (vacations.Any())
                {
                    hoursUsedThisYear = vacations.Sum(x => x.NumberOfVacationHoursUsed);
                }

                //Calculate the number of hours if any vacations started before the start hire anniversary but ends after the start hire anniversary
                var previousVacations = db.Vacations.Where(x => x.VacationId != vacationID &&
                                                                x.IsVacationTime == isVacationTime &&
                                                                x.StartDate < startHireAnniversary &&
                                                                x.EndDate >= startHireAnniversary &&
                                                                x.EmployeeId == employeeId);
                if (previousVacations.Any())
                {
                    hoursUsedThisYear += previousVacations.ToList()
                        .Select(x => GetNumberOfVacationHoursUsedForPossibleVacation(employeeId, startHireAnniversary, x.EndDate, x.NumberOfVacationHoursUsed != 4))
                        .Sum(x => x);
                }

                //Calculate the number of hours if any vacations started before the end hire anniversary but ends after the end hire anniversary
                var futureVacations = db.Vacations.Where(x => x.VacationId != vacationID &&
                                                              x.IsVacationTime == isVacationTime &&
                                                              x.StartDate < endHireAnniversary &&
                                                              x.EndDate >= endHireAnniversary &&
                                                              x.EmployeeId == employeeId);
                hoursUsedThisYear += futureVacations
                    .ToList()
                    .Select(x => GetNumberOfVacationHoursUsedForPossibleVacation(employeeId, x.StartDate, endHireAnniversary, x.NumberOfVacationHoursUsed != 4))
                    .Sum(x => x);
            }

            return hoursUsedThisYear;
        }

        /// <summary>
        /// Checks if datetime is a weekend or a holiday.
        /// </summary>
        /// <param name="day">Date to Check</param>
        /// <returns>True or false if day is a weekend or holiday</returns>
        public static bool IsDateTimeWeekendOrHoliday(DateTime day)
        {
            if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            //Holidays Check

            //New Years
            if (day.Month == 1 & day.Day == 1)
            {
                return true;
            }

            //Memorial Day
            DateTime memorialDay = new DateTime(day.Year, 5, 31);
            while (memorialDay.DayOfWeek != DayOfWeek.Monday)
            {
                memorialDay = memorialDay.AddDays(-1);
            }

            if (memorialDay.Equals(day))
            {
                return true;
            }

            // July 4th
            if (day.Month == 7 & day.Day == 4)
            {
                return true;
            }

            //Labor Day
            DateTime laborDay = new DateTime(day.Year, 9, 1);
            while (laborDay.DayOfWeek != DayOfWeek.Monday)
            {
                laborDay = laborDay.AddDays(1);
            }

            if (laborDay.Equals(day))
            {
                return true;
            }

            //Thanksgiving
            if (day.Month == 11 && day.Day == Enumerable.Range(0, 30)
                .Where(x => new DateTime(day.Year, 11, x).DayOfWeek == DayOfWeek.Thursday)
                .ElementAt(3))
            {
                return true;
            }

            //Christmas
            if (day.Month == 12 & day.Day == 25)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return the number of vacation hours used for this employee from the start time to the end time. Assumed that any day take past one day is a full 8 hours per day.
        /// </summary>
        /// <param name="employeeId">Employee ID of the person taking the vacation</param>
        /// <param name="start">Start Date</param>
        /// <param name="end">End Date</param>
        /// <param name="isFullVacationDay">Optional: Used to taking a half day or full day. Only applicable if only taking one day off</param>
        /// <returns>The number of vacation hours used for the time period. If the time period goes over their hire date it will return the amount of time after the hire date.</returns>
        public static int GetNumberOfVacationHoursUsedForPossibleVacation(int employeeId, DateTime start, DateTime end, bool isFullVacationDay = true)
        {
            if (!isFullVacationDay)
            {
                if (IsDateTimeWeekendOrHoliday(start))
                {
                    return 0;
                }

                return 4;
            }

            using (var db = new JDsDBEntities())
            {
                DateTime? hireDate = db.Employees.SingleOrDefault(x => x.EmployeeId == employeeId)?.HireDate;

                DateTime startDate = start;
                DateTime endDate = end;

                if (hireDate.HasValue)
                {
                    DateTime? nearestHireAnniversary = IsDayAndMonthBetweenTwoDateTimes(hireDate.Value.Month, hireDate.Value.Day,
                        startDate, endDate);

                    if (nearestHireAnniversary.HasValue)
                    {
                        startDate = nearestHireAnniversary.Value;
                    }
                }

                int vacationTimeSpanInDays = Convert.ToInt32(Math.Ceiling((endDate - startDate).TotalDays));
                int vacationHours = 0;

                for (var i = 0; i <= vacationTimeSpanInDays; i++)
                {
                    if (!VacationCalculator.IsDateTimeWeekendOrHoliday(startDate.AddDays(i)))
                    {
                        vacationHours += 8;
                    }
                }

                return vacationHours;
            }
        }

        /// <summary>
        /// Check if specified month and day is between 2 DateTimes
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>The DateTime that is between the startDate and endDate if it there is one, otherwise null.</returns>
        public static DateTime? IsDayAndMonthBetweenTwoDateTimes(int month, int day, DateTime startDate, DateTime endDate)
        {
            DateTime firstComparisonDate = new DateTime(startDate.Year, month, day);
            DateTime secondComparisonDate = new DateTime(endDate.Year, month, day);

            if (startDate < firstComparisonDate && endDate >= firstComparisonDate)
            {
                return firstComparisonDate;
            }
            else if (startDate < secondComparisonDate && endDate >= secondComparisonDate)
            {
                return secondComparisonDate;
            }

            return null;
        }

        /// <summary>
        /// Checks if the employee has enough hours for the requested vacation. Takes into account if the vacation goes over their hire date.
        /// </summary>
        /// <param name="employeeId">Employee ID of the person taking the vacation</param>
        /// <param name="start">Start date of the requested vacation</param>
        /// <param name="end">End date of the requested vacation</param>
        /// <param name="isVacationTime">Is vacation time or sick day</param>
        /// <param name="numberOfHoursUsed">Out: Hours of time that would be taken(Positive or Negative)</param>
        /// <param name="vacationID">Optional: VacationID of the vacation if we're editing one</param>
        /// <returns>Boolean if the employee has the enough hours for the vacation. Also returns the number of hours used.</returns>
        public static bool HasAvaliableVacationHours(int employeeId, DateTime start, DateTime end, bool isVacationTime, out int numberOfHoursUsed, int vacationID = 0)
        {
            using (var db = new JDsDBEntities())
            {
                Employee employee = db.Employees.Single(x => x.EmployeeId == employeeId);

                //We need the number of vacation hours used during the employee's year that the vacation ends on
                //Since this is needed for both section of the vacation before and after their hire anniversary or 
                //if the vacation doesn't land within their hire anniversary, it's done early
                int totalHoursUsed = CalculateHoursUsed(employeeId, end, vacationID);
                int totalNumberOfHoursEarned = CalculateVacationHours(employee.HireDate, end, isVacationTime);

                DateTime? workAnniversary = null;

                if (employee.HireDate != null)
                {
                    workAnniversary = IsDayAndMonthBetweenTwoDateTimes(employee.HireDate.Value.Month,
                        employee.HireDate.Value.Day, start, end);
                }

                //If the employee doesn't have a hire date or the work anniversary isn't in between the vacation start or end the calculation is easy.
                if (employee.HireDate == null || workAnniversary == null)
                {
                    numberOfHoursUsed = GetNumberOfVacationHoursUsedForPossibleVacation(employeeId, start, end);

                    if (numberOfHoursUsed > totalNumberOfHoursEarned - totalHoursUsed)
                    {
                        return false;
                    }
                }

                //For the hire anniversary being in between the vacation time
                else
                {
                    //For after the hire anniversary
                    numberOfHoursUsed = GetNumberOfVacationHoursUsedForPossibleVacation(employeeId, workAnniversary.Value, end);

                    if (totalNumberOfHoursEarned - totalHoursUsed < numberOfHoursUsed)
                    {
                        return false;
                    }

                    //Before the hire anniversary
                    int numberOfHoursUsedForPreviousYear = GetNumberOfVacationHoursUsedForPossibleVacation(employeeId, start, workAnniversary.Value.AddDays(-1));
                    int totalHoursUsedForPreviousYear = CalculateHoursUsed(employeeId, start, vacationID);
                    totalNumberOfHoursEarned = CalculateVacationHours(employee.HireDate, start,
                        isVacationTime);

                    if (totalNumberOfHoursEarned - totalHoursUsedForPreviousYear < numberOfHoursUsedForPreviousYear)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
