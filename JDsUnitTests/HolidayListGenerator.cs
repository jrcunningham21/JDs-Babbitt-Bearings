using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace JDsUnitTests
{
    [TestClass]
    public class HolidayListGenerator
    {
        [TestMethod]
        public void GenerateHolidays()
        {
            for (int ii = 2014; ii < 2050; ii++)
            {
                var holidays = getHolidayList(ii);

                foreach (var holiday in holidays)
                {
                    //string entry = /*holiday.HolidayName + ": " +*/ "Date.parse(\""+holiday.Date.ToShortDateString() + "\"),";
                    string entry = string.Format("\"{0}\",", holiday.Date.ToString("M/d/yyyy"));
                    Console.WriteLine(entry);
                    File.AppendAllText(@"C:\Temp\holidays.txt", entry + Environment.NewLine);
                }
            }
        }


        public class Holiday
        {
            public string HolidayName { get; set; }
            public DateTime Date { get; set; }

            public Holiday(string holidayName, DateTime date)
            {
                HolidayName = holidayName;
                Date = date;
            }
        }

        public static List<Holiday> getHolidayList(int vYear)
        {
            int FirstWeek = 1;
            int SecondWeek = 2;
            int ThirdWeek = 3;
            int FourthWeek = 4;
            int LastWeek = 5;

            List<Holiday> HolidayList = new List<Holiday>();

            //   http://www.usa.gov/citizens/holidays.shtml      
            //   http://archive.opm.gov/operating_status_schedules/fedhol/2013.asp

            // New Year's Day            Jan 1
            HolidayList.Add(new Holiday("NewYears", new DateTime(vYear, 1, 1)));

            // Martin Luther King, Jr. third Mon in Jan
            //HolidayList.Add(new Holiday("MLK", GetNthDayOfNthWeek(new DateTime(vYear, 1, 1), DayOfWeek.Monday, ThirdWeek)));

            // Washington's Birthday third Mon in Feb
            //HolidayList.Add(new Holiday("WashingtonsBDay", GetNthDayOfNthWeek(new DateTime(vYear, 2, 1), DayOfWeek.Monday, ThirdWeek)));

            // Memorial Day          last Mon in May
            HolidayList.Add(new Holiday("MemorialDay", GetNthDayOfNthWeek(new DateTime(vYear, 5, 1), DayOfWeek.Monday, LastWeek)));

            // Independence Day      July 4
            HolidayList.Add(new Holiday("IndependenceDay", new DateTime(vYear, 7, 4)));

            // Labor Day             first Mon in Sept
            HolidayList.Add(new Holiday("LaborDay", GetNthDayOfNthWeek(new DateTime(vYear, 9, 1), DayOfWeek.Monday, FirstWeek)));

            // Columbus Day          second Mon in Oct
            //HolidayList.Add(new Holiday("Columbus", GetNthDayOfNthWeek(new DateTime(vYear, 10, 1), DayOfWeek.Monday, SecondWeek)));

            // Veterans Day          Nov 11
            //HolidayList.Add(new Holiday("Veterans", new DateTime(vYear, 11, 11)));

            // Thanksgiving Day      fourth Thur in Nov
            HolidayList.Add(new Holiday("Thanksgiving", GetNthDayOfNthWeek(new DateTime(vYear, 11, 1), DayOfWeek.Thursday, FourthWeek)));

            // Christmas Day         Dec 25
            HolidayList.Add(new Holiday("Christmas", new DateTime(vYear, 12, 25)));

            //saturday holidays are moved to Fri; Sun to Mon
            foreach (var holiday in HolidayList)
            {
                if (holiday.Date.DayOfWeek == DayOfWeek.Saturday)
                {
                    holiday.Date = holiday.Date.AddDays(-1);
                }
                if (holiday.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    holiday.Date = holiday.Date.AddDays(1);
                }
            }

            //return
            return HolidayList;

        }

        private static System.DateTime GetNthDayOfNthWeek(DateTime dt, DayOfWeek dayofWeek, int WhichWeek)
        {
            //specify which day of which week of a month and this function will get the date
            //this function uses the month and year of the date provided

            //get first day of the given date
            System.DateTime dtFirst = new DateTime(dt.Year, dt.Month, 1);

            //get first DayOfWeek of the month
            System.DateTime dtRet = dtFirst.AddDays(6 - (int)dtFirst.AddDays(-1 * ((int)dayofWeek + 1)).DayOfWeek);

            //get which week
            dtRet = dtRet.AddDays((WhichWeek - 1) * 7);

            //if day is past end of month then adjust backwards a week
            if (dtRet >= dtFirst.AddMonths(1))
            {
                dtRet = dtRet.AddDays(-7);
            }

            //return
            return dtRet;

        }
        


    }
}
