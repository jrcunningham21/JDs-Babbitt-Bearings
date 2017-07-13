using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Net;
using Antlr.Runtime;
using JDsDataModel;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeUpload;
using JDsWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JDsWebApp.Areas.EmployeePortal.Models.EmployeeManagement;
using NLog;
using JDsWebApp.Areas.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using JDsWebApp.Areas.EmployeePortal.Models.Vacation;
using JDsWebApp.Helpers;
using JDsWebApp.Areas.EmployeePortal.Models.VacationReport;
using System.Threading.Tasks;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    public class SkillsController : Controller
    {
        // GET: EmployeePortal/Skills
        public ActionResult Index()
        {
            return View();
        }

        #region Skills
        [HttpGet]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult GetSkillList()
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var skillsListFromDb = db.Skills.Select(x => x.Name).ToList();

                try
                {
                    var skillList = new SkillListModel();

                    skillList.SkillNames = skillsListFromDb;



                    return Json(skillList, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }


        [HttpGet]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult GetEmployeeSkills()
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var employees = db.Employees.ToList();

                try
                {
                    var allEmpSkills = new List<EmployeeSkillsModel>();
                    var counter = 0;

                    foreach (var emp in employees)
                    {
                        var empSkills = emp.Skills.Select(x => x.SkillId).ToList();

                        // Array to keep track of whether employee has skill
                        // The index of the array is corresponding to the SkillId
                        bool[] employeeBoolArray = new bool[db.Skills.Count()];

                        // ID of first Skill
                        int skillIdCounter = 1;

                        for (int i = 0; i < employeeBoolArray.Length; i++)
                        {
                            if (empSkills.Contains(skillIdCounter))
                            {
                                employeeBoolArray[i] = true;
                            }
                            else
                            {
                                employeeBoolArray[i] = false;
                            }

                            skillIdCounter++;
                        }

                        allEmpSkills.Add(new EmployeeSkillsModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.Name,
                            SkillIds = empSkills,
                            EmpHasSkillBoolArray = employeeBoolArray
                        });

                        counter++;
                    }

                    return Json(allEmpSkills, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }


        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public void SkillCheckBoxClicked(int empID, int skillID, bool hasSkill)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                db.UpdateEmployeeSkill(empID, skillID, hasSkill);
            }
        }
        #endregion
    }
}