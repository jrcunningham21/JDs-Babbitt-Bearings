using JDsDataModel;
using JDsDataModel.ViewModels;
using JDsWebApp.Models;
using JDSignOff = JDsDataModel.SignOff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NLog;

namespace JDsWebApp.Controllers
{
    public class SignOffController : Controller
    {
        readonly JDsDBEntities db = new JDsDBEntities();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public ActionResult SignOff(string skillName)
        {
            SignOffViewModel model = new SignOffViewModel()
            {
                SkillName = skillName
            };

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult SignOff(SignOffViewModel model)
        {
            try
            {
                Employee employee = db.Employees.FirstOrDefault(x => x.PIN == model.Pin);

                if (employee != null)
                {
                    // Assuming that manager should have all abilities
                    bool hasSkill = employee.Skills.FirstOrDefault(s => s.Name == "Management" || s.Name == model.SkillName) != null ? true : false;

                    if (hasSkill)
                    {
                        int signOffID = 0;

                        try
                        {
                            JDSignOff signOff = new JDSignOff()
                            {
                                EmployeeId = employee.EmployeeId,
                                RequiredSkillId = db.Skills.Single(s => s.Name == model.SkillName).SkillId,
                                SignOffDate = DateTime.Now
                            };

                            db.SignOffs.Add(signOff);
                            db.SaveChanges();

                            signOffID = signOff.SignOffId;
                        }
                        catch (Exception ex)
                        {
                            // is OK, they're a manager but don't have the specific skill
                            _logger.Trace("Acceptable error in Sign Off: manager without speific skill: " + ex.Message);
                        }

                        return Json(new { result = true, name = employee.Name, signOffID = signOffID });
                    }
                    else
                    {
                        ModelState.AddModelError("SignOffError", "Missing required skill.");
                    }
                }
                else
                {
                    ModelState.AddModelError("SignOffError", "Employee not found.");
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                _logger.Trace("Error in Sign Off: " + ex.Message);
                ModelState.AddModelError("SignOffError", "Error has occured.");
                return PartialView(model);
            }            
        }
    }
}
