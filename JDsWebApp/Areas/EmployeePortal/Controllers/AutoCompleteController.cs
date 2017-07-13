using JDsDataModel;
using JDsWebApp.Areas.EmployeePortal.Models.AutoComplete;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{

    public class AutoCompleteController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // GET: EmployeePortal/AutoComplete
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAutoCompleteValues()
        {
            AutoCompleteListModel acList = new AutoCompleteListModel();

            using (JDsDBEntities db = new JDsDBEntities())
            {                            
                db.AutoCompletes.ToList().ForEach(x =>
                {
                    acList.Add(new AutoCompleteModel()
                    {
                        AutoCompleteID = x.AutoCompleteId,
                        ControlID = x.ControlId,
                        Value = x.Value
                    });
                });
            }

            return Json(acList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAutoCompleteValuesByControlId(int id)
        {
            AutoCompleteListModel acList = new AutoCompleteListModel();

            using (JDsDBEntities db = new JDsDBEntities())
            {
                db.AutoCompletes.Where(x => x.ControlId == id).ToList().ForEach(x =>
                {
                    acList.Add(new AutoCompleteModel()
                    {
                        AutoCompleteID = x.AutoCompleteId,
                        ControlID = x.ControlId,
                        Value = x.Value
                    });
                });
            }

            return Json(acList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNewPart(int id, string value)
        {
            // TODO: add validation to make sure part does not already exist            
            using (JDsDBEntities db = new JDsDBEntities())
            {
                try
                {
                    if (value != null)
                    {
                        db.AutoCompletes.Add(new AutoComplete
                        {
                            ControlId = id,
                            Value = value
                        });

                        db.SaveChanges();

                    }
                    var ac = db.AutoCompletes.Where(x => x.Value == value).FirstOrDefault();
                    return Json(ac, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _logger.Trace($"Error in AddNewPart:  { ex.ToString()} ");
                    return Json(new AutoComplete { Value = value }, JsonRequestBehavior.AllowGet);
                }

            }            
         
                    
        }
        
        [HttpPost]
        public ActionResult DeletePart(int id)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var itemToRemove = db.AutoCompletes.Find(id);

                if (itemToRemove != null)
                    db.AutoCompletes.Remove(itemToRemove);

                db.SaveChanges();
            }

            return Json(id, JsonRequestBehavior.AllowGet);
        }        
    }
}