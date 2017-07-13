using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JDsDataModel;
using JDsDataModel.ViewModels.Processes.ProcessBabbittBearing;
using JDsWebApp.Controllers;
using JDsWebApp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JDsUnitTests
{
    [TestClass]
    public class ProcessBB_Controllers
    {
        private ProcessBabbitBearingController _controller;
        private JDsDBEntities _db;

        public ProcessBB_Controllers()
        {
            _db = new JDsDBEntities();
            _controller = new ProcessBabbitBearingController();
        }
        
        [TestMethod]
        public void BB1_IncomingInspection()
        {
            var result = ProcessHelper.Generate_BB1_IncomingInspectionViewModel(_db, 1);
            
            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB1_IncomingInspection()
        {
            var viewmodel = ProcessHelper.Generate_BB1_IncomingInspectionViewModel(_db, 1);
            viewmodel.DisassembledStenciledBy = "DisassembledStenciled";
            viewmodel.FinalInspectionApprovedBy = "FinalInspectionApproved";
            viewmodel.IncomingFilesApprovedBy = "IncomingFilesApproved";
            viewmodel.PartNotes = "This is the part notes.";
            viewmodel.Material = "Cast Iron";
            viewmodel.Insulation = "Insulated for size only";
            viewmodel.ARPinDepth = 2.0m;
            viewmodel.HasARPin = true;
            viewmodel.ARPinDiameter = 3.0m;
            viewmodel.IsInsulated = true;
            viewmodel.OverallLength = 4.0m;
            viewmodel.IsTC = true;
            viewmodel.Quantity = 1;
            // viewmodel.ID1Measurements = 
            // viewmodel.ID2Measurements = 
            // viewmodel.OD1Measurements = 
            // viewmodel.OD2Measurements = 
            // viewmodel.Seal =

            _controller.Save_BB1_IncomingInspection(viewmodel);

            viewmodel = ProcessHelper.Generate_BB1_IncomingInspectionViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("DisassembledStenciled", viewmodel.DisassembledStenciledBy);
            Assert.AreEqual("FinalInspectionApproved", viewmodel.FinalInspectionApprovedBy);
            Assert.AreEqual("IncomingFilesApproved", viewmodel.IncomingFilesApprovedBy);
            Assert.AreEqual("This is the part notes.", viewmodel.PartNotes);
            Assert.AreEqual("Cast Iron", viewmodel.Material);
            Assert.AreEqual("Insulated for size only", viewmodel.Insulation);
            Assert.AreEqual(2.0m, viewmodel.ARPinDepth);
            Assert.AreEqual(true, viewmodel.HasARPin);
            Assert.AreEqual(3.0m, viewmodel.ARPinDiameter);
            Assert.AreEqual(true, viewmodel.IsInsulated);
            Assert.AreEqual(4.0m, viewmodel.OverallLength);
            Assert.AreEqual(true, viewmodel.IsTC);
            Assert.AreEqual(1, viewmodel.Quantity);
        }

        [TestMethod]
        public void BB2_PrecastRoughout()
        {
            var result = ProcessHelper.Generate_BB2_PrecastRoughoutViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB2_PrecastRoughout()
        {
            var viewmodel = ProcessHelper.Generate_BB2_PrecastRoughoutViewModel(_db, 1);
            viewmodel.BaseMaterial = "WILL BE OVERWRITTEN BY MATERIAL IN STEP 1....";
            viewmodel.RoughOutBy = "RoughOut";

            _controller.Save_BB2_PrecastRoughout(viewmodel);

            viewmodel = ProcessHelper.Generate_BB2_PrecastRoughoutViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("Cast Iron", viewmodel.BaseMaterial);   // This will be pulled from step 1 as expected...
            Assert.AreEqual("RoughOut", viewmodel.RoughOutBy);
        }

        [TestMethod]
        public void BB3_SpincastProcess()
        {
            var result = ProcessHelper.Generate_BB3_SpincastProcessViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB3_SpincastProcess()
        {
            var viewmodel = ProcessHelper.Generate_BB3_SpincastProcessViewModel(_db, 1);
            viewmodel.BabbitTempBy = "BabbitTemp";
            viewmodel.BabbitTemp = 100.0m;
            viewmodel.BaseMaterial = "WILL BE OVERWRITTEN BY MATERIAL IN STEP 1....";
            viewmodel.CutApartBy = "CutApart";
            viewmodel.DeburredBy = "Deburred";
            viewmodel.PlasteredBy = "Plastered";
            viewmodel.PlateTemp = 99.0m;
            viewmodel.PlateTempBy = "PlateTemp";
            viewmodel.IsTinnedByVisible = true;
            viewmodel.RPM = 1000;
            viewmodel.RPMBy = "RPM";
            viewmodel.ScrubbedBy = "Scrubbed";
            viewmodel.SpuncastBy = "Spuncast";

            _controller.Save_BB3_SpincastProcess(viewmodel);

            viewmodel = ProcessHelper.Generate_BB3_SpincastProcessViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("BabbitTemp", viewmodel.BabbitTempBy);
            Assert.AreEqual(100.0m, viewmodel.BabbitTemp);
            Assert.AreEqual("Cast Iron", viewmodel.BaseMaterial);
            Assert.AreEqual("CutApart", viewmodel.CutApartBy);
            Assert.AreEqual("Deburred", viewmodel.DeburredBy);
            Assert.AreEqual("Plastered", viewmodel.PlasteredBy);
            Assert.AreEqual(99.0m, viewmodel.PlateTemp);
            Assert.AreEqual("PlateTemp", viewmodel.PlateTempBy);
            Assert.AreEqual(true, viewmodel.IsTinnedByVisible);
            Assert.AreEqual(1000, viewmodel.RPM);
            Assert.AreEqual("RPM", viewmodel.RPMBy);
            Assert.AreEqual("Scrubbed", viewmodel.ScrubbedBy);
            Assert.AreEqual("Spuncast", viewmodel.SpuncastBy);
        }

        [TestMethod]
        public void BB4_PostcastCleanup()
        {
            var result = ProcessHelper.Generate_BB4_PostcastCleanupViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB4_PostcastCleanup()
        {
            var viewmodel = ProcessHelper.Generate_BB4_PostcastCleanupViewModel(_db, 1);
            viewmodel.PlasterRemovedBy = "PlasterRemoved";
            viewmodel.WashedBy = "Washed";

            _controller.Save_BB4_PostcastCleanup(viewmodel);

            viewmodel = ProcessHelper.Generate_BB4_PostcastCleanupViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("PlasterRemoved", viewmodel.PlasterRemovedBy);
            Assert.AreEqual("Washed", viewmodel.WashedBy);
        }

        [TestMethod]
        public void BB5_PostcastRoughout()
        {
            var result = ProcessHelper.Generate_BB5_PostcastRoughoutViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB5_PostcastRoughout()
        {
            var viewmodel = ProcessHelper.Generate_BB5_PostcastRoughoutViewModel(_db, 1);
            viewmodel.ODInfo = "WILL BE OVERWRITTEN BY ODINFO IN STEP 1....";
            viewmodel.RoughedOutBy = "RoughedOut";
            viewmodel.IsPT = true;
            viewmodel.IsUT = true;
            // viewmodel.CID1Measurements =
            // viewmodel.CID2Measurements =
            // viewmodel.IID1Measurements =
            // viewmodel.IID2Measurements = 

            _controller.Save_BB5_PostcastRoughout(viewmodel);

            viewmodel = ProcessHelper.Generate_BB5_PostcastRoughoutViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("Insulated for size only", viewmodel.ODInfo);       // ODInfo from step 1
            Assert.AreEqual("RoughedOut", viewmodel.RoughedOutBy);
            Assert.AreEqual(true, viewmodel.IsPT);
            Assert.AreEqual(true, viewmodel.IsUT);
        }

        [TestMethod]
        public void BB6_InsulationProcess()
        {
            // THIS STEP IS SKIPPED IF THERE IS NO INSULATION
            var result = ProcessHelper.Generate_BB6_InsulationProcessViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB6_InsulationProcess()
        {
            // THIS STEP IS SKIPPED IF THERE IS NO INSULATION
            var viewmodel = ProcessHelper.Generate_BB6_InsulationProcessViewModel(_db, 1);
            viewmodel.GritBlastedBy = "GritBlasted";
            viewmodel.InsulationInstalledBy = "InsulationInstalled";
            viewmodel.InsulationMadeBy = "InsulationMade";
            viewmodel.SlingerRingCutOutBy = "SlingerRingCutOut";

            _controller.Save_BB6_InsulationProcess(viewmodel);

            viewmodel = ProcessHelper.Generate_BB6_InsulationProcessViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("GritBlasted", viewmodel.GritBlastedBy);
            Assert.AreEqual("InsulationInstalled", viewmodel.InsulationInstalledBy);
            Assert.AreEqual("InsulationMade", viewmodel.InsulationMadeBy);
            Assert.AreEqual("SlingerRingCutOut", viewmodel.SlingerRingCutOutBy);
        }

        [TestMethod]
        public void BB7_CleanupProcess()
        {
            var result = ProcessHelper.Generate_BB7_CleanupProcessViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB7_CleanupProcess()
        {
            // THIS STEP IS SKIPPED IF THERE IS NO INSULATION
            var viewmodel = ProcessHelper.Generate_BB7_CleanupProcessViewModel(_db, 1);
            viewmodel.CleanUpBy = "CleanUp";
            viewmodel.SlingerRingCutOutBy = "SlingerRingCutOut";
            viewmodel.IsSlingerRingVisible = true;

            _controller.Save_BB7_CleanupProcess(viewmodel);

            viewmodel = ProcessHelper.Generate_BB7_CleanupProcessViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("CleanUp", viewmodel.CleanUpBy);
            Assert.AreEqual("SlingerRingCutOut", viewmodel.SlingerRingCutOutBy);
            Assert.AreEqual(true, viewmodel.IsSlingerRingVisible);
        }

        [TestMethod]
        public void BB8_FinalMachineInspection()
        {
            var result = ProcessHelper.Generate_BB8_FinalMachineInspectionViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB8_FinalMachineInspection()
        {
            var viewmodel = ProcessHelper.Generate_BB8_FinalMachineInspectionViewModel(_db, 1);
            viewmodel.IsBondVerified = true;
            viewmodel.IsClean = true;
            viewmodel.IsDowelChecksGood = true;
            viewmodel.IsFlaggedForCustomerApproval = true;
            viewmodel.IsSplitLinesVerified = true;
            viewmodel.ProblemResolution = "Problem";
            viewmodel.ReadyForFinalMachineBy = "ReadyForFinalMachine";
            viewmodel.SizedApprovedBy = "ReadyForFinalMachine";
            // viewmodel.OD1Measurements =
            // viewmodel.OD2Measurements =

            _controller.Save_BB8_FinalMachineInspection(viewmodel);

            viewmodel = ProcessHelper.Generate_BB8_FinalMachineInspectionViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual(true, viewmodel.IsBondVerified);
            Assert.AreEqual(true, viewmodel.IsClean);
            Assert.AreEqual(true, viewmodel.IsDowelChecksGood);
            Assert.AreEqual(true, viewmodel.IsFlaggedForCustomerApproval);
            Assert.AreEqual(true, viewmodel.IsSplitLinesVerified);
            Assert.AreEqual("Problem", viewmodel.ProblemResolution);
            Assert.AreEqual("ReadyForFinalMachine", viewmodel.ReadyForFinalMachineBy);
            Assert.AreEqual("ReadyForFinalMachine", viewmodel.SizedApprovedBy);
        }

        [TestMethod]
        public void BB9_FinishBoreProcess()
        {
            var result = ProcessHelper.Generate_BB9_FinishBoreProcessViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB9_FinishBoreProcess()
        {
            var viewmodel = ProcessHelper.Generate_BB9_FinishBoreProcessViewModel(_db, 1);
            viewmodel.ClearanceSize = "1.0";
            viewmodel.CustomerBoreSize = "2.0";
            viewmodel.CustomerBoreSizeHorizontal = "3.0";
            viewmodel.CustomerODSize.Add(new PartDiameterMeasurement
            {
                ODComment = "0.40"
            });
            viewmodel.FinishedBoreBy = "FinishedBore";
            viewmodel.IsVerifiedSizesOk = true;
            viewmodel.Notes = "Test notes.";
            viewmodel.Runout1BackSize = 5.0m;
            viewmodel.Runout1FrontSize = 6.0m;
            viewmodel.Runout1MiddleSize = 7.0m;
            viewmodel.Runout2BoreSize = 9.0m;
            viewmodel.Runout2FaceSize = 11.0m;
            viewmodel.ShaftSize = "12.0";
            viewmodel.ShimSize = "13.0";
            viewmodel.Tolerance = "14.0";

            _controller.Save_BB9_FinishBoreProcess(viewmodel);

            viewmodel = ProcessHelper.Generate_BB9_FinishBoreProcessViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual(1.0m, viewmodel.ClearanceSize);
            Assert.AreEqual(2.0m, viewmodel.CustomerBoreSize);
            Assert.AreEqual(3.0m, viewmodel.CustomerBoreSizeHorizontal);
            Assert.AreEqual(4.0m, viewmodel.CustomerODSize);
            Assert.AreEqual("FinishedBore", viewmodel.FinishedBoreBy);
            Assert.AreEqual(true, viewmodel.IsVerifiedSizesOk);
            Assert.AreEqual("Test notes.", viewmodel.Notes);
            Assert.AreEqual(5.0m, viewmodel.Runout1BackSize);
            Assert.AreEqual(6.0m, viewmodel.Runout1FrontSize);
            Assert.AreEqual(7.0m, viewmodel.Runout1MiddleSize);
            Assert.AreEqual(9.0m, viewmodel.Runout2BoreSize);
            Assert.AreEqual(11m, viewmodel.Runout2FaceSize);
            Assert.AreEqual(12.0m, viewmodel.ShaftSize);
            Assert.AreEqual(13.0m, viewmodel.ShimSize);
            Assert.AreEqual(14.0m, viewmodel.Tolerance);
        }

        [TestMethod]
        public void BB10_FinalAssembly()
        {
            var result = ProcessHelper.Generate_BB10_FinalAssemblyViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB10_FinalAssembly()
        {
            var viewmodel = ProcessHelper.Generate_BB10_FinalAssemblyViewModel(_db, 1);
            viewmodel.DeburredBy = "Deburred";
            viewmodel.MillWorkBy = "MillWork";
            viewmodel.PartsInstalledBy = "PartsInstalled";
            viewmodel.VerifiedBy = "Verified";
            // viewmodel.PartsAccessories = 

            _controller.Save_BB10_FinalAssembly(viewmodel);

            viewmodel = ProcessHelper.Generate_BB10_FinalAssemblyViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("Deburred", viewmodel.DeburredBy);
            Assert.AreEqual("MillWork", viewmodel.MillWorkBy);
            Assert.AreEqual("PartsInstalled", viewmodel.PartsInstalledBy);
            Assert.AreEqual("Verified", viewmodel.VerifiedBy);
        }


        [TestMethod]
        public void BB11_FinalInspection()
        {
            var result = ProcessHelper.Generate_BB11_FinalInspectionViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB11_FinalInspection()
        {
            var viewmodel = ProcessHelper.Generate_BB11_FinalInspectionViewModel(_db, 1);
            viewmodel.ARPinDepth = 1.0m;
            viewmodel.ARPinDiameter = 2.0m;
            viewmodel.FinalInspectionBy = "FinalInspection";
            viewmodel.HasARPin = true;
            viewmodel.IsAcceptable = true;
            viewmodel.IsCustomerCalled = true;
            viewmodel.IsJobFailed = true;
            viewmodel.OverallLength = 3.0m;
            viewmodel.Quantity = 10;
            viewmodel.ReturnStepId = 1;
            viewmodel.TCInstalledQuantity = 10;
            // viewmodel.ID1Measurements = 
            // viewmodel.ID2Measurements = 
            // viewmodel.OD1Measurements = 
            // viewmodel.OD2Measurements = 
            // viewmodel.SealMeasurements =

            _controller.Save_BB11_FinalInspection(viewmodel);

            viewmodel = ProcessHelper.Generate_BB11_FinalInspectionViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual(1.0m, viewmodel.ARPinDepth);
            Assert.AreEqual(2.0m, viewmodel.ARPinDiameter);
            Assert.AreEqual("FinalInspection", viewmodel.FinalInspectionBy);
            Assert.AreEqual(true, viewmodel.HasARPin);
            Assert.AreEqual(true, viewmodel.IsAcceptable);
            Assert.AreEqual(true, viewmodel.IsCustomerCalled);
            Assert.AreEqual(true, viewmodel.IsJobFailed);
            Assert.AreEqual(3.0m, viewmodel.OverallLength);
            Assert.AreEqual(10, viewmodel.Quantity);
            Assert.AreEqual(1, viewmodel.ReturnStepId);
            Assert.AreEqual(10, viewmodel.TCInstalledQuantity);
        }

        [TestMethod]
        public void BB12_Delivery()
        {
            var result = ProcessHelper.Generate_BB12_DeliveryViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB12_Delivery()
        {
            var dt = DateTime.UtcNow;
            var req = dt.AddDays(5);

            var viewmodel = ProcessHelper.Generate_BB12_DeliveryViewModel(_db, 1);
            viewmodel.PackedBy = "Packed";
            viewmodel.ShippedVia = "USPS";
            viewmodel.ShipDate = dt;
            viewmodel.RequiredDate = req;
            viewmodel.TrackingNumber = "T123890ADBDAF";
            
            _controller.Save_BB12_Delivery(viewmodel);

            viewmodel = ProcessHelper.Generate_BB12_DeliveryViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("Packed", viewmodel.PackedBy);
            Assert.AreEqual("USPS", viewmodel.ShippedVia);
            Assert.AreEqual(dt, viewmodel.ShipDate);
            Assert.AreEqual(req, viewmodel.RequiredDate);
            Assert.AreEqual("T123890ADBDAF", viewmodel.TrackingNumber);
        }

        [TestMethod]
        public void BB13_FinalSignOff()
        {
            var result = ProcessHelper.Generate_BB13_FinalSignOffViewModel(_db, 1);

            // More asserts here
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Save_BB13_FinalSignOff()
        {
            var viewmodel = ProcessHelper.Generate_BB13_FinalSignOffViewModel(_db, 1);
            viewmodel.InspectedBy = "Inspected";

            _controller.Save_BB13_FinalSignOff(viewmodel);

            viewmodel = ProcessHelper.Generate_BB13_FinalSignOffViewModel(_db, 1);
            Assert.IsNotNull(viewmodel);
            Assert.AreEqual("Inspected", viewmodel.InspectedBy);
        }
        
    }
}
