using System.Linq;
using System.Web;
using System.Web.Mvc;
using JDsDataModel;

namespace JDsWebApp.Controllers
{
    /// <summary>
    /// Generic process controller to redirect based on PartId to appropriate process and step from the BigBoard.
    /// </summary>
    public class ProcessController : Controller
    {
        private readonly JDsDBEntities _db = new JDsDBEntities();

        // GET: Process, id is the PartId in the process
        public ActionResult Index(int id = 0)
        {
            var part = _db.Parts.Include("Processes").FirstOrDefault(x => x.PartId == id);

            if (part == null)
            {
                return new HttpStatusCodeResult(404, $"PartId={id} not found.");
            }

            var lastprocess = part.Processes.OrderByDescending(x => x.ProcessId).FirstOrDefault();

            if (lastprocess == null)
            {
                return new HttpStatusCodeResult(404, $"Process for PartId={id} not found.");
            }

            var laststep = _db.Steps.Where(x => x.ProcessId == lastprocess.ProcessId)
                .OrderByDescending(x => x.StepNumber)
                .FirstOrDefault();

            switch (part.PartTypeId)
            {
                case 1:
                    // Babbit Bearing
                    // 1:  Incoming Inspection
                    // 2:  Precast Roughout
                    // 3:  Spincast Process
                    // 4:  Postcast Cleanup
                    // 5:  Postcast Roughout
                    // 6:  Insulation Process
                    // 7:  Cleanup Process
                    // 8:  Final Machine Inspection
                    // 9:  Finish Bore Process
                    // 10: Final Assembly
                    // 11: Final Inspection
                    // 12: Delivery
                    // 13: Final Sign-Off

                    if (laststep == null)
                    {
                        return RedirectToAction("BB1_IncomingInspection", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });
                    }

                    switch (laststep.StepNumber)
                    {
                        case 1:
                            return RedirectToAction("BB1_IncomingInspection", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 2:
                            return RedirectToAction("BB2_PrecastRoughout", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 3:
                            return RedirectToAction("BB3_SpincastProcess", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 4:
                            return RedirectToAction("BB4_PostcastCleanup", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 5:
                            return RedirectToAction("BB5_PostcastRoughout", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 6:
                            return RedirectToAction("BB6_InsulationProcess", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 7:
                            return RedirectToAction("BB7_CleanupProcess", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 8:
                            return RedirectToAction("BB8_FinalMachineInspection", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 9:
                            return RedirectToAction("BB9_FinishBoreProcess", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 10:
                            return RedirectToAction("BB10_FinalAssembly", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 11:
                            return RedirectToAction("BB11_FinalInspection", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 12:
                            return RedirectToAction("BB12_Delivery", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        case 13:
                            return RedirectToAction("BB13_FinalSignOff", "ProcessBabbitBearing", new { id = lastprocess.ProcessId });

                        default:
                            return new HttpStatusCodeResult(404, $"Step number not found.");
                    }

                case 2:
                    // Slinger Ring
                    return new HttpStatusCodeResult(404, $"Sling ring not implemented.");

                case 3:
                    // Oil Shield
                    return new HttpStatusCodeResult(404, $"Oil shield not implemented.");

                default:
                    return new HttpStatusCodeResult(404, $"PartTypeId={part.PartTypeId} not found.");
            }
        }
    }
}