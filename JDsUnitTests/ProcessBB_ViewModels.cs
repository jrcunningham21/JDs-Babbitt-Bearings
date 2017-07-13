using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using JDsDataModel;
using JDsDataModel.ViewModels;
using JDsDataModel.ViewModels.Processes;
using JDsDataModel.ViewModels.Processes.ProcessBabbittBearing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JDsUnitTests
{
    [TestClass]
    public class ProcessBB_ViewModels
    {
        private JDsDBEntities _db;

        public ProcessBB_ViewModels()
        {
            _db = new JDsDBEntities();

            if (Directory.Exists(Properties.Settings.Default.BaseTestPath))
            {
                Directory.CreateDirectory(Properties.Settings.Default.BaseTestPath);
            }
        }

        [TestMethod]
        public void StepViewModel()
        {
            var vm = new StepViewModel();
            PopulateStepViewModel(vm);

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"StepViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<StepViewModel>(json);
            AssertStepViewModel(vm);
        }

        [TestMethod]
        public void DeliveryStepViewModel()
        {
            var vm = new DeliveryStepViewModel();
            PopulateStepViewModel(vm);
            vm.PackedBy = "Test Packer";
            vm.ShippedVia = "Test Shipper";
            vm.ShipDate = new DateTime(2016, 1, 1);
            vm.RequiredDate = new DateTime(2016, 1, 5);
            vm.TrackingNumber = "Z123ABC000";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"DeliveryStepViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<DeliveryStepViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Packer", vm.PackedBy);
            Assert.AreEqual("Test Shipper", vm.ShippedVia);
            Assert.AreEqual(2016, vm.ShipDate.Value.Year);
            Assert.AreEqual(1, vm.ShipDate.Value.Month);
            Assert.AreEqual(1, vm.ShipDate.Value.Day);
            Assert.AreEqual(2016, vm.RequiredDate.Value.Year);
            Assert.AreEqual(1, vm.RequiredDate.Value.Month);
            Assert.AreEqual(5, vm.RequiredDate.Value.Day);
            Assert.AreEqual("Z123ABC000", vm.TrackingNumber);
        }

        [TestMethod]
        public void BB1_IncomingInspectionViewModel()
        {
            var vm = new BB1_IncomingInspectionViewModel();
            PopulateStepViewModel(vm);
            vm.DisassembledStenciledBy = "Test Disassembled Stenciled By";
            vm.PicturesApprovedBy = "Test Pictures By";
            vm.IncomingFilesApprovedBy = "Test Incoming By";
            vm.ID1Measurements = new List<BoreMeasurementViewModel>();
            vm.OD1Measurements = new List<BoreMeasurementViewModel>();
            vm.ID2Measurements = new List<BoreMeasurementViewModel>();
            vm.OD2Measurements = new List<BoreMeasurementViewModel>();
            vm.SealMeasurements = new List<BoreMeasurementViewModel>();
            vm.HasARPin = true;
            vm.ARPinDiameter = 1.0m;
            vm.Quantity = 1;
            vm.OverallLength = 1.1m;
            vm.IsInsulated = true;
            vm.Insulation = "Test Rubber";
            vm.MeasuredIncomingSizesBy = "Test Measured By";
            vm.PartsAccessories = new List<IncomingInspectionAccessory>();
            vm.Material = "Test Material";
            vm.IsTC = true;
            vm.TCDepth = 2.0m;
            vm.TCDiameter = 3.0m;
            vm.PartNotes = "Test Part Notes";
            vm.FinalInspectionApprovedBy = "Test Final Approved By";

            vm.ID1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.ID1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.01m,
                B = 1.01m,
                C = 1.00m,
            });

            vm.ID2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.ID2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.011m,
                B = 1.011m,
                C = 1.001m,
            });

            vm.OD1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.012m,
                B = 1.012m,
                C = 1.002m,
            });

            vm.OD2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.013m,
                B = 1.013m,
                C = 1.003m,
            });

            vm.SealMeasurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.SealMeasurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.014m,
                B = 1.014m,
                C = 1.004m,
            });

            var part = new Part
            {
                PartId = 1,
                JobId = 2,
                PartTypeId = 3,
                PartContactId = 4,
                PartStatusId = 5,
                WorkScope = "Test Work Scope",
                RequiredDate = new DateTime(2016, 1, 1),
                ShipByDate = new DateTime(2016, 1, 2),
                ShippedDate = new DateTime(2016, 1, 2),
                NonConformanceReportNotes = "Test Non Conformance Notes",
                RequiresPT = true,
                RequiresUT = true,
                PartProcessId = 1,
                ItemCode = "IC123",
                IdentifyingInfo = "Test Info",
            };

            var iia = new IncomingInspectionAccessory
            {
                IncomingInspectionAccessoryId = 1,
                PartId = 2,
                Quantity = 3,
                Part = part,
                Name = "Test IIA",
            };

            vm.PartsAccessories.Add(iia);

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB1_IncomingInspectionViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Disassembled Stenciled By", vm.DisassembledStenciledBy);
            Assert.AreEqual("Test Pictures By", vm.PicturesApprovedBy);
            Assert.AreEqual("Test Incoming By", vm.IncomingFilesApprovedBy);
            Assert.AreEqual(1.01m, vm.ID1Measurements[0].A);
            Assert.AreEqual(1.01m, vm.ID1Measurements[0].B);
            Assert.AreEqual(1.00m, vm.ID1Measurements[0].C);
            Assert.AreEqual(1.011m, vm.ID2Measurements[0].A);
            Assert.AreEqual(1.011m, vm.ID2Measurements[0].B);
            Assert.AreEqual(1.001m, vm.ID2Measurements[0].C);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].A);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].B);
            Assert.AreEqual(1.002m, vm.OD1Measurements[0].C);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].A);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].B);
            Assert.AreEqual(1.003m, vm.OD2Measurements[0].C);
            Assert.AreEqual(1.014m, vm.SealMeasurements[0].A);
            Assert.AreEqual(1.014m, vm.SealMeasurements[0].B);
            Assert.AreEqual(1.004m, vm.SealMeasurements[0].C);
            Assert.AreEqual(true, vm.HasARPin);
            Assert.AreEqual(1.0m, vm.ARPinDiameter);
            Assert.AreEqual(1, vm.Quantity);
            Assert.AreEqual(1.1m, vm.OverallLength);
            Assert.AreEqual(true, vm.IsInsulated);
            Assert.AreEqual("Test Rubber", vm.Insulation);
            Assert.AreEqual("Test Measured By", vm.MeasuredIncomingSizesBy);
            Assert.AreEqual("Test Measured By", vm.MeasuredIncomingSizesBy);
            Assert.AreEqual("Test Material", vm.Material);
            Assert.AreEqual(true, vm.IsTC);
            Assert.AreEqual(2.0m, vm.TCDepth);
            Assert.AreEqual(3.0m, vm.TCDiameter);
            Assert.AreEqual("Test Part Notes", vm.PartNotes);
            Assert.AreEqual("Test Final Approved By", vm.FinalInspectionApprovedBy);
            Assert.AreEqual(1, vm.PartsAccessories[0].IncomingInspectionAccessoryId);
            Assert.AreEqual(2, vm.PartsAccessories[0].PartId);
            Assert.AreEqual("Test IIA", vm.PartsAccessories[0].Name);
            Assert.AreEqual(3, vm.PartsAccessories[0].Quantity);
            Assert.AreEqual(1, vm.PartsAccessories[0].Part.PartId);
            Assert.AreEqual(2, vm.PartsAccessories[0].Part.JobId);
            Assert.AreEqual(3, vm.PartsAccessories[0].Part.PartTypeId);
            Assert.AreEqual(4, vm.PartsAccessories[0].Part.PartContactId);
            Assert.AreEqual(5, vm.PartsAccessories[0].Part.PartStatusId);
            Assert.AreEqual("Test Work Scope", vm.PartsAccessories[0].Part.WorkScope);
            Assert.AreEqual(new DateTime(2016, 1, 1), vm.PartsAccessories[0].Part.RequiredDate);
            Assert.AreEqual(new DateTime(2016, 1, 2), vm.PartsAccessories[0].Part.ShipByDate);
            Assert.AreEqual(new DateTime(2016, 1, 2), vm.PartsAccessories[0].Part.ShippedDate);
            Assert.AreEqual("Test Non Conformance Notes", vm.PartsAccessories[0].Part.NonConformanceReportNotes);
            Assert.AreEqual(true, vm.PartsAccessories[0].Part.RequiresPT);
            Assert.AreEqual(true, vm.PartsAccessories[0].Part.RequiresUT);
            Assert.AreEqual(1, vm.PartsAccessories[0].Part.PartProcessId);
            Assert.AreEqual("IC123", vm.PartsAccessories[0].Part.ItemCode);
            Assert.AreEqual("Test Info", vm.PartsAccessories[0].Part.IdentifyingInfo);
            Assert.AreEqual(null, vm.PartsAccessories[0].Part.Contact);
            Assert.AreEqual(null, vm.PartsAccessories[0].Part.Job);
        }

        [TestMethod]
        public void BB2_PrecastRoughoutViewModel()
        {
            var vm = new BB2_PrecastRoughoutViewModel();
            PopulateStepViewModel(vm);
            vm.RoughOutBy = "Test Rougher";
            vm.BaseMaterial = "Test Base Material";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB2_PrecastRoughoutViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB2_PrecastRoughoutViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Rougher", vm.RoughOutBy);
            Assert.AreEqual("Test Base Material", vm.BaseMaterial);
        }

        [TestMethod]
        public void BB3_SpincastProcessViewModel()
        {
            var vm = new BB3_SpincastProcessViewModel();
            PopulateStepViewModel(vm);
            vm.DeburredBy = "Test Deburrer";
            vm.TinnedBy = "Test Tinner";
            vm.PlasteredBy = "Test Plasterer";
            vm.RPM = 1234;
            vm.RPMBy = "Test RPMer";
            vm.BabbitTemp = 100.0m;
            vm.BabbitTempBy = "Test Babbitt Temp Measurer";
            vm.PlateTemp = 120.0m;
            vm.PlateTempBy = "Test Plate Temp Measurer";
            vm.ScrubbedBy = "Test Scrubber";
            vm.SpuncastBy = "Test Spuncast";
            vm.CutApartBy = "Test Cutter";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB3_SpincastProcessViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB3_SpincastProcessViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Deburrer", vm.DeburredBy);
            Assert.AreEqual("Test Tinner", vm.TinnedBy);
            Assert.AreEqual("Test Plasterer", vm.PlasteredBy);
            Assert.AreEqual(1234, vm.RPM);
            Assert.AreEqual("Test RPMer", vm.RPMBy);
            Assert.AreEqual(100.0m, vm.BabbitTemp);
            Assert.AreEqual("Test Babbitt Temp Measurer", vm.BabbitTempBy);
            Assert.AreEqual(120.0m, vm.PlateTemp);
            Assert.AreEqual("Test Plate Temp Measurer", vm.PlateTempBy);
            Assert.AreEqual("Test Scrubber", vm.ScrubbedBy);
            Assert.AreEqual("Test Spuncast", vm.SpuncastBy);
            Assert.AreEqual("Test Cutter", vm.CutApartBy);
        }

        [TestMethod]
        public void BB4_PostcastCleanupViewModel()
        {
            var vm = new BB4_PostcastCleanupViewModel();
            PopulateStepViewModel(vm);
            vm.PlasterRemovedBy = "Test Plaster Remover";
            vm.WashedBy = "Test Washer";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB4_PostcastCleanupViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB4_PostcastCleanupViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Plaster Remover", vm.PlasterRemovedBy);
            Assert.AreEqual("Test Washer", vm.WashedBy);
        }

        [TestMethod]
        public void BB5_PostcastRoughoutViewModel()
        {
            var vm = new BB5_PostcastRoughoutViewModel();
            PopulateStepViewModel(vm);
            vm.ODInfo = "Test OD Info";
            vm.IsUT = true;
            vm.IsPT = true;
            vm.RoughedOutBy = "Test Rougher";

            vm.IID1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.IID1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.01m,
                B = 1.01m,
                C = 1.00m,
            });

            vm.IID2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.IID2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.011m,
                B = 1.011m,
                C = 1.001m,
            });

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB5_PostcastRoughoutViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB5_PostcastRoughoutViewModel>(json);
            AssertStepViewModel(vm);

            Assert.AreEqual("Test OD Info", vm.ODInfo);
            Assert.AreEqual(true, vm.IsUT);
            Assert.AreEqual(true, vm.IsPT);
            Assert.AreEqual("Test Rougher", vm.RoughedOutBy);
            Assert.AreEqual(1.01m, vm.IID1Measurements[0].A);
            Assert.AreEqual(1.01m, vm.IID1Measurements[0].B);
            Assert.AreEqual(1.00m, vm.IID1Measurements[0].C);
            Assert.AreEqual(1.011m, vm.IID2Measurements[0].A);
            Assert.AreEqual(1.011m, vm.IID2Measurements[0].B);
            Assert.AreEqual(1.001m, vm.IID2Measurements[0].C);
        }

        [TestMethod]
        public void BB6_InsulationProcessViewModel()
        {
            var vm = new BB6_InsulationProcessViewModel();
            PopulateStepViewModel(vm);
            vm.InsulationMadeBy = "Test Insulator";
            vm.GritBlastedBy = "Test Gritter";
            vm.SlingerRingCutOutBy = "Test Ringer";
            vm.InsulationInstalledBy = "Test Installer";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB6_InsulationProcessViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB6_InsulationProcessViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Insulator", vm.InsulationMadeBy);
            Assert.AreEqual("Test Gritter", vm.GritBlastedBy);
            Assert.AreEqual("Test Ringer", vm.SlingerRingCutOutBy);
            Assert.AreEqual("Test Installer", vm.InsulationInstalledBy);
        }

        [TestMethod]
        public void BB7_CleanupProcessViewModel()
        {
            var vm = new BB7_CleanupProcessViewModel();
            PopulateStepViewModel(vm);
            vm.CleanUpBy = "Test Cleaner";
            vm.SlingerRingCutOutBy = "Test Ringer";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB7_CleanupProcessViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB7_CleanupProcessViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Cleaner", vm.CleanUpBy);
            Assert.AreEqual("Test Ringer", vm.SlingerRingCutOutBy);
        }

        [TestMethod]
        public void BB8_FinalMachineInspectionViewModel()
        {
            var vm = new BB8_FinalMachineInspectionViewModel();
            PopulateStepViewModel(vm);
            vm.IsSplitLinesVerified = true;
            vm.IsDowelChecksGood = true;
            vm.IsBondVerified = true;
            vm.IsClean = false;
            vm.ReadyForFinalMachineBy = "Test Final";
            vm.IsFlaggedForCustomerApproval = true;
            vm.ProblemResolution = "Test Resolution";
            vm.SizedApprovedBy = "Test Size";
                       vm.OD1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.012m,
                B = 1.012m,
                C = 1.002m,
            });

            vm.OD2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.013m,
                B = 1.013m,
                C = 1.003m,
            });

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB8_FinalMachineInspectionViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB8_FinalMachineInspectionViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual(true, vm.IsSplitLinesVerified);
            Assert.AreEqual(true, vm.IsDowelChecksGood);
            Assert.AreEqual(true, vm.IsBondVerified);
            Assert.AreEqual(false, vm.IsClean);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].A);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].B);
            Assert.AreEqual(1.002m, vm.OD1Measurements[0].C);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].A);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].B);
            Assert.AreEqual(1.003m, vm.OD2Measurements[0].C);
            Assert.AreEqual("Test Final", vm.ReadyForFinalMachineBy);
            Assert.AreEqual(true, vm.IsFlaggedForCustomerApproval);
            Assert.AreEqual("Test Resolution", vm.ProblemResolution);
            Assert.AreEqual("Test Size", vm.SizedApprovedBy);
        }

        [TestMethod]
        public void BB9_FinishBoreProcessViewModel()
        {
            var vm = new BB9_FinishBoreProcessViewModel();
            PopulateStepViewModel(vm);
            vm.ShaftSize = "1.00";
            vm.ClearanceSize = "0.01";
            vm.ShimSize = "0.02";
            vm.Tolerance = "0.005";
            vm.Notes = "Test Notes";
            vm.CustomerBoreSize = "0.10";
            vm.CustomerBoreSizeHorizontal = "0.05";
            vm.CustomerODSize.Add(new PartDiameterMeasurement
            {
                ODComment = "0.01"
            });
            vm.Runout1FrontSize = 0.01m;
            vm.Runout1BackSize = 0.002m;
            vm.Runout1MiddleSize = 0.03m;
            vm.Runout2FaceSize = 0.004m;
            vm.Runout2BoreSize = 0.005m;
            vm.FinishedBoreBy = "Test Finisher";
            vm.IsVerifiedSizesOk = true;

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB9_FinishBoreProcessViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB9_FinishBoreProcessViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual(1.00m, vm.ShaftSize);
            Assert.AreEqual(0.01m, vm.ClearanceSize);
            Assert.AreEqual(0.02m, vm.ShimSize);
            Assert.AreEqual(0.005m, vm.Tolerance);
            Assert.AreEqual("Test Notes", vm.Notes);
            Assert.AreEqual(0.10m, vm.CustomerBoreSize);
            Assert.AreEqual(0.05m, vm.CustomerBoreSizeHorizontal);
            Assert.AreEqual(0.01m, vm.CustomerODSize);
            Assert.AreEqual(0.01m, vm.Runout1FrontSize);
            Assert.AreEqual(0.002m, vm.Runout1BackSize);
            Assert.AreEqual(0.03m, vm.Runout1MiddleSize);
            Assert.AreEqual(0.004m, vm.Runout2FaceSize);
            Assert.AreEqual(0.005m, vm.Runout2BoreSize);
            Assert.AreEqual("Test Finisher", vm.FinishedBoreBy);
            Assert.AreEqual(true, vm.IsVerifiedSizesOk);
        }

        [TestMethod]
        public void BB10_FinalAssemblyViewModel()
        {
            var vm = new BB10_FinalAssemblyViewModel();
            PopulateStepViewModel(vm);
            vm.MillWorkBy = "Test Miller";
            vm.DeburredBy = "Test Deburrer";
            vm.PartsInstalledBy = "Test Part Installer";
            vm.VerifiedBy = "Test Verifier";

            var part = new Part
            {
                PartId = 1,
                JobId = 2,
                PartTypeId = 3,
                PartContactId = 4,
                PartStatusId = 5,
                WorkScope = "Test Work Scope",
                RequiredDate = new DateTime(2016, 1, 1),
                ShipByDate = new DateTime(2016, 1, 2),
                ShippedDate = new DateTime(2016, 1, 2),
                NonConformanceReportNotes = "Test Non Conformance Notes",
                RequiresPT = true,
                RequiresUT = true,
                PartProcessId = 1,
                ItemCode = "IC123",
                IdentifyingInfo = "Test Info",
            };

            var iia = new IncomingInspectionAccessory
            {
                IncomingInspectionAccessoryId = 1,
                PartId = 2,
                Quantity = 3,
                Part = part,
                Name = "Test IIA",
            };

            vm.PartsAccessories.Add(iia);

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB10_FinalAssemblyViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB10_FinalAssemblyViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Miller", vm.MillWorkBy);
            Assert.AreEqual("Test Deburrer", vm.DeburredBy);
            Assert.AreEqual("Test Part Installer", vm.PartsInstalledBy);
            Assert.AreEqual("Test Verifier", vm.VerifiedBy);
            Assert.AreEqual(1, vm.PartsAccessories[0].IncomingInspectionAccessoryId);
            Assert.AreEqual(2, vm.PartsAccessories[0].PartId);
            Assert.AreEqual("Test IIA", vm.PartsAccessories[0].Name);
            Assert.AreEqual(3, vm.PartsAccessories[0].Quantity);
            Assert.AreEqual(1, vm.PartsAccessories[0].Part.PartId);
            Assert.AreEqual(2, vm.PartsAccessories[0].Part.JobId);
            Assert.AreEqual(3, vm.PartsAccessories[0].Part.PartTypeId);
            Assert.AreEqual(4, vm.PartsAccessories[0].Part.PartContactId);
            Assert.AreEqual(5, vm.PartsAccessories[0].Part.PartStatusId);
            Assert.AreEqual("Test Work Scope", vm.PartsAccessories[0].Part.WorkScope);
            Assert.AreEqual(new DateTime(2016, 1, 1), vm.PartsAccessories[0].Part.RequiredDate);
            Assert.AreEqual(new DateTime(2016, 1, 2), vm.PartsAccessories[0].Part.ShipByDate);
            Assert.AreEqual(new DateTime(2016, 1, 2), vm.PartsAccessories[0].Part.ShippedDate);
            Assert.AreEqual("Test Non Conformance Notes", vm.PartsAccessories[0].Part.NonConformanceReportNotes);
            Assert.AreEqual(true, vm.PartsAccessories[0].Part.RequiresPT);
            Assert.AreEqual(true, vm.PartsAccessories[0].Part.RequiresUT);
            Assert.AreEqual(1, vm.PartsAccessories[0].Part.PartProcessId);
            Assert.AreEqual("IC123", vm.PartsAccessories[0].Part.ItemCode);
            Assert.AreEqual("Test Info", vm.PartsAccessories[0].Part.IdentifyingInfo);
            Assert.AreEqual(null, vm.PartsAccessories[0].Part.Contact);
            Assert.AreEqual(null, vm.PartsAccessories[0].Part.Job);
        }

        [TestMethod]
        public void BB11_FinalInspectionViewModel()
        {
            var vm = new BB11_FinalInspectionViewModel();
            PopulateStepViewModel(vm);
            vm.OverallLength = 1.1m;
            vm.TCInstalledQuantity = 2;
            vm.IsAcceptable = true;
            vm.IsCustomerCalled = true;
            vm.FinalInspectionBy = "Test Inspector";
            vm.ReturnStepId = 0;
            vm.IsJobFailed = false;
            vm.ID1Measurements = new List<BoreMeasurementViewModel>();
            vm.OD1Measurements = new List<BoreMeasurementViewModel>();
            vm.ID2Measurements = new List<BoreMeasurementViewModel>();
            vm.OD2Measurements = new List<BoreMeasurementViewModel>();
            vm.SealMeasurements = new List<BoreMeasurementViewModel>();

            vm.ID1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.ID1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.01m,
                B = 1.01m,
                C = 1.00m,
            });

            vm.ID2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.ID2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.011m,
                B = 1.011m,
                C = 1.001m,
            });

            vm.OD1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.012m,
                B = 1.012m,
                C = 1.002m,
            });

            vm.OD2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.013m,
                B = 1.013m,
                C = 1.003m,
            });

            vm.SealMeasurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.SealMeasurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.014m,
                B = 1.014m,
                C = 1.004m,
            });

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB11_FinalInspectionViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB11_FinalInspectionViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual(1.01m, vm.ID1Measurements[0].A);
            Assert.AreEqual(1.01m, vm.ID1Measurements[0].B);
            Assert.AreEqual(1.00m, vm.ID1Measurements[0].C);
            Assert.AreEqual(1.011m, vm.ID2Measurements[0].A);
            Assert.AreEqual(1.011m, vm.ID2Measurements[0].B);
            Assert.AreEqual(1.001m, vm.ID2Measurements[0].C);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].A);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].B);
            Assert.AreEqual(1.002m, vm.OD1Measurements[0].C);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].A);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].B);
            Assert.AreEqual(1.003m, vm.OD2Measurements[0].C);
            Assert.AreEqual(1.014m, vm.SealMeasurements[0].A);
            Assert.AreEqual(1.014m, vm.SealMeasurements[0].B);
            Assert.AreEqual(1.004m, vm.SealMeasurements[0].C);
            Assert.AreEqual(1.1m, vm.OverallLength);
            Assert.AreEqual(2, vm.TCInstalledQuantity);
            Assert.AreEqual(true, vm.IsAcceptable);
            Assert.AreEqual(true, vm.IsCustomerCalled);
            Assert.AreEqual("Test Inspector", vm.FinalInspectionBy);
            Assert.AreEqual(0, vm.ReturnStepId);
            Assert.AreEqual(false, vm.IsJobFailed);
        }

        [TestMethod]
        public void BB12_DeliveryViewModel()
        {
            var vm = new BB12_DeliveryViewModel();
            PopulateStepViewModel(vm);
            vm.PackedBy = "Test Packer";
            vm.ShippedVia = "Test Shipper";
            vm.ShipDate = new DateTime(2016, 1, 1);
            vm.RequiredDate = new DateTime(2016, 1, 5);
            vm.TrackingNumber = "Z123ABC000";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB12_DeliveryViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB12_DeliveryViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Packer", vm.PackedBy);
            Assert.AreEqual("Test Shipper", vm.ShippedVia);
            Assert.AreEqual(2016, vm.ShipDate.Value.Year);
            Assert.AreEqual(1, vm.ShipDate.Value.Month);
            Assert.AreEqual(1, vm.ShipDate.Value.Day);
            Assert.AreEqual(2016, vm.RequiredDate.Value.Year);
            Assert.AreEqual(1, vm.RequiredDate.Value.Month);
            Assert.AreEqual(5, vm.RequiredDate.Value.Day);
            Assert.AreEqual("Z123ABC000", vm.TrackingNumber);
        }

        [TestMethod]
        public void BB13_FinalSignOffViewModel()
        {
            var vm = new BB13_FinalSignOffViewModel();
            PopulateStepViewModel(vm);

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB13_FinalSignOffViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(filepath, json);
            Assert.IsTrue(File.Exists(filepath));

            // Deserialize
            json = File.ReadAllText(filepath);
            vm = JsonConvert.DeserializeObject<BB13_FinalSignOffViewModel>(json);
            AssertStepViewModel(vm);
        }

        private void PopulateStepViewModel(StepViewModel vm)
        {
            vm.CustomerLabel = "Test Customer";
            vm.ContactLabel = "Test Contact";
            vm.JobSummary = "Test Job Summary";
            vm.JobId = -1;
            vm.ProcessId = -2;
            vm.StepId = -3;
            vm.IsPTRequired = true;
            vm.IsUTRequired = false;
            vm.IsDisplayPT = true;
            vm.IsDisplayUT = true;
            vm.IsCompleted = false;
        }

        private void AssertStepViewModel(StepViewModel vm)
        {
            Assert.AreEqual("Test Customer", vm.CustomerLabel);
            Assert.AreEqual("Test Contact", vm.ContactLabel);
            Assert.AreEqual("Test Job Summary", vm.JobSummary);
            Assert.AreEqual(-1, vm.JobId);
            Assert.AreEqual(-2, vm.ProcessId);
            Assert.AreEqual(-3, vm.StepId);
            Assert.AreEqual(true, vm.IsPTRequired);
            Assert.AreEqual(false, vm.IsUTRequired);
            Assert.AreEqual(true, vm.IsDisplayPT);
            Assert.AreEqual(true, vm.IsDisplayUT);
            Assert.AreEqual(false, vm.IsCompleted);
        }

        [TestMethod]
        public void BB1_IncomingInspectionViewModel_To_DB()
        {
            var vm = new BB1_IncomingInspectionViewModel();
            PopulateStepViewModel(vm);
            vm.DisassembledStenciledBy = "Test Disassembled Stenciled By";
            vm.PicturesApprovedBy = "Test Pictures By";
            vm.IncomingFilesApprovedBy = "Test Incoming By";
            vm.ID1Measurements = new List<BoreMeasurementViewModel>();
            vm.OD1Measurements = new List<BoreMeasurementViewModel>();
            vm.ID2Measurements = new List<BoreMeasurementViewModel>();
            vm.OD2Measurements = new List<BoreMeasurementViewModel>();
            vm.SealMeasurements = new List<BoreMeasurementViewModel>();
            vm.HasARPin = true;
            vm.ARPinDiameter = 1.0m;
            vm.Quantity = 1;
            vm.OverallLength = 1.1m;
            vm.IsInsulated = true;
            vm.Insulation = "Test Rubber";
            vm.MeasuredIncomingSizesBy = "Test Measured By";
            vm.PartsAccessories = new List<IncomingInspectionAccessory>();
            vm.Material = "Test Material";
            vm.IsTC = true;
            vm.TCDepth = 2.0m;
            vm.TCDiameter = 3.0m;
            vm.PartNotes = "Test Part Notes";
            vm.FinalInspectionApprovedBy = "Test Final Approved By";

            vm.ID1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.ID1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.01m,
                B = 1.01m,
                C = 1.00m,
            });

            vm.ID2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.ID2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.011m,
                B = 1.011m,
                C = 1.001m,
            });

            vm.OD1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.012m,
                B = 1.012m,
                C = 1.002m,
            });

            vm.OD2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.013m,
                B = 1.013m,
                C = 1.003m,
            });

            vm.SealMeasurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.SealMeasurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.014m,
                B = 1.014m,
                C = 1.004m,
            });

            var part = new Part
            {
                PartId = 1,
                JobId = 2,
                PartTypeId = 3,
                PartContactId = 4,
                PartStatusId = 5,
                WorkScope = "Test Work Scope",
                RequiredDate = new DateTime(2016, 1, 1),
                ShipByDate = new DateTime(2016, 1, 2),
                ShippedDate = new DateTime(2016, 1, 2),
                NonConformanceReportNotes = "Test Non Conformance Notes",
                RequiresPT = true,
                RequiresUT = true,
                PartProcessId = 1,
                ItemCode = "IC123",
                IdentifyingInfo = "Test Info",
            };

            var iia = new IncomingInspectionAccessory
            {
                IncomingInspectionAccessoryId = 1,
                PartId = 2,
                Quantity = 3,
                Part = part,
                Name = "Test IIA",
            };

            vm.PartsAccessories.Add(iia);

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);

            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 1);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 1,
                    ProcessId = 1,
                    Title = "Test1: IncomingInspectionViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }


            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 1);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB1_IncomingInspectionViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Disassembled Stenciled By", vm.DisassembledStenciledBy);
            Assert.AreEqual("Test Pictures By", vm.PicturesApprovedBy);
            Assert.AreEqual("Test Incoming By", vm.IncomingFilesApprovedBy);
            Assert.AreEqual(1.01m, vm.ID1Measurements[0].A);
            Assert.AreEqual(1.01m, vm.ID1Measurements[0].B);
            Assert.AreEqual(1.00m, vm.ID1Measurements[0].C);
            Assert.AreEqual(1.011m, vm.ID2Measurements[0].A);
            Assert.AreEqual(1.011m, vm.ID2Measurements[0].B);
            Assert.AreEqual(1.001m, vm.ID2Measurements[0].C);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].A);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].B);
            Assert.AreEqual(1.002m, vm.OD1Measurements[0].C);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].A);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].B);
            Assert.AreEqual(1.003m, vm.OD2Measurements[0].C);
            Assert.AreEqual(1.014m, vm.SealMeasurements[0].A);
            Assert.AreEqual(1.014m, vm.SealMeasurements[0].B);
            Assert.AreEqual(1.004m, vm.SealMeasurements[0].C);
            Assert.AreEqual(true, vm.HasARPin);
            Assert.AreEqual(1.0m, vm.ARPinDiameter);
            Assert.AreEqual(1, vm.Quantity);
            Assert.AreEqual(1.1m, vm.OverallLength);
            Assert.AreEqual(true, vm.IsInsulated);
            Assert.AreEqual("Test Rubber", vm.Insulation);
            Assert.AreEqual("Test Measured By", vm.MeasuredIncomingSizesBy);
            Assert.AreEqual("Test Measured By", vm.MeasuredIncomingSizesBy);
            Assert.AreEqual("Test Material", vm.Material);
            Assert.AreEqual(true, vm.IsTC);
            Assert.AreEqual(2.0m, vm.TCDepth);
            Assert.AreEqual(3.0m, vm.TCDiameter);
            Assert.AreEqual("Test Part Notes", vm.PartNotes);
            Assert.AreEqual("Test Final Approved By", vm.FinalInspectionApprovedBy);
            Assert.AreEqual(1, vm.PartsAccessories[0].IncomingInspectionAccessoryId);
            Assert.AreEqual(2, vm.PartsAccessories[0].PartId);
            Assert.AreEqual("Test IIA", vm.PartsAccessories[0].Name);
            Assert.AreEqual(3, vm.PartsAccessories[0].Quantity);
            Assert.AreEqual(1, vm.PartsAccessories[0].Part.PartId);
            Assert.AreEqual(2, vm.PartsAccessories[0].Part.JobId);
            Assert.AreEqual(3, vm.PartsAccessories[0].Part.PartTypeId);
            Assert.AreEqual(4, vm.PartsAccessories[0].Part.PartContactId);
            Assert.AreEqual(5, vm.PartsAccessories[0].Part.PartStatusId);
            Assert.AreEqual("Test Work Scope", vm.PartsAccessories[0].Part.WorkScope);
            Assert.AreEqual(new DateTime(2016, 1, 1), vm.PartsAccessories[0].Part.RequiredDate);
            Assert.AreEqual(new DateTime(2016, 1, 2), vm.PartsAccessories[0].Part.ShipByDate);
            Assert.AreEqual(new DateTime(2016, 1, 2), vm.PartsAccessories[0].Part.ShippedDate);
            Assert.AreEqual("Test Non Conformance Notes", vm.PartsAccessories[0].Part.NonConformanceReportNotes);
            Assert.AreEqual(true, vm.PartsAccessories[0].Part.RequiresPT);
            Assert.AreEqual(true, vm.PartsAccessories[0].Part.RequiresUT);
            Assert.AreEqual(1, vm.PartsAccessories[0].Part.PartProcessId);
            Assert.AreEqual("IC123", vm.PartsAccessories[0].Part.ItemCode);
            Assert.AreEqual("Test Info", vm.PartsAccessories[0].Part.IdentifyingInfo);
            Assert.AreEqual(null, vm.PartsAccessories[0].Part.Contact);
            Assert.AreEqual(null, vm.PartsAccessories[0].Part.Job);

        }

        [TestMethod]
        public void BB2_PrecastRoughoutViewModel_To_DB()
        {
            var vm = new BB2_PrecastRoughoutViewModel();
            PopulateStepViewModel(vm);
            vm.RoughOutBy = "Test Rougher";
            vm.BaseMaterial = "Test Base Material";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);

            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 2);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 2,
                    ProcessId = 1,
                    Title = "Test1: PrecastRoughoutViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 2);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB2_PrecastRoughoutViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Rougher", vm.RoughOutBy);
            Assert.AreEqual("Test Base Material", vm.BaseMaterial);
        }
        [TestMethod]
        public void BB3_SpincastProcessViewModel_To_DB()
        {
            var vm = new BB3_SpincastProcessViewModel();
            PopulateStepViewModel(vm);
            vm.DeburredBy = "Test Deburrer";
            vm.TinnedBy = "Test Tinner";
            vm.PlasteredBy = "Test Plasterer";
            vm.RPM = 1234;
            vm.RPMBy = "Test RPMer";
            vm.BabbitTemp = 100.0m;
            vm.BabbitTempBy = "Test Babbitt Temp Measurer";
            vm.PlateTemp = 120.0m;
            vm.PlateTempBy = "Test Plate Temp Measurer";
            vm.ScrubbedBy = "Test Scrubber";
            vm.SpuncastBy = "Test Spuncast";
            vm.CutApartBy = "Test Cutter";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 3);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 3,
                    ProcessId = 1,
                    Title = "Test1: SpincastProcessViewModel",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 3);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB3_SpincastProcessViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Deburrer", vm.DeburredBy);
            Assert.AreEqual("Test Tinner", vm.TinnedBy);
            Assert.AreEqual("Test Plasterer", vm.PlasteredBy);
            Assert.AreEqual(1234, vm.RPM);
            Assert.AreEqual("Test RPMer", vm.RPMBy);
            Assert.AreEqual(100.0m, vm.BabbitTemp);
            Assert.AreEqual("Test Babbitt Temp Measurer", vm.BabbitTempBy);
            Assert.AreEqual(120.0m, vm.PlateTemp);
            Assert.AreEqual("Test Plate Temp Measurer", vm.PlateTempBy);
            Assert.AreEqual("Test Scrubber", vm.ScrubbedBy);
            Assert.AreEqual("Test Spuncast", vm.SpuncastBy);
            Assert.AreEqual("Test Cutter", vm.CutApartBy);
        }

        [TestMethod]
        public void BB4_PostcastCleanupViewModel_To_DB()
        {
            var vm = new BB4_PostcastCleanupViewModel();
            PopulateStepViewModel(vm);
            vm.PlasterRemovedBy = "Test Plaster Remover";
            vm.WashedBy = "Test Washer";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 4);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 4,
                    ProcessId = 1,
                    Title = "Test1: PostcastCleanupViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 4);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB4_PostcastCleanupViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Plaster Remover", vm.PlasterRemovedBy);
            Assert.AreEqual("Test Washer", vm.WashedBy);
        }

        [TestMethod]
        public void BB5_PostcastRoughoutViewModel_To_DB()
        {
            var vm = new BB5_PostcastRoughoutViewModel();
            PopulateStepViewModel(vm);
            vm.ODInfo = "Test OD Info";
            vm.IsUT = true;
            vm.IsPT = true;
            vm.RoughedOutBy = "Test Rougher";

            vm.IID1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.IID1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.01m,
                B = 1.01m,
                C = 1.00m,
            });

            vm.IID2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.IID2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.011m,
                B = 1.011m,
                C = 1.001m,
            });
            

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB5_PostcastRoughoutViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 5);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 5,
                    ProcessId = 1,
                    Title = "Test1: PostcastRoughoutViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 5);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB5_PostcastRoughoutViewModel>(json);
            AssertStepViewModel(vm);

            Assert.AreEqual("Test OD Info", vm.ODInfo);
            Assert.AreEqual(true, vm.IsUT);
            Assert.AreEqual(true, vm.IsPT);
            Assert.AreEqual("Test Rougher", vm.RoughedOutBy);
            Assert.AreEqual(1.01m, vm.IID1Measurements[0].A);
            Assert.AreEqual(1.01m, vm.IID1Measurements[0].B);
            Assert.AreEqual(1.00m, vm.IID1Measurements[0].C);
            Assert.AreEqual(1.011m, vm.IID2Measurements[0].A);
            Assert.AreEqual(1.011m, vm.IID2Measurements[0].B);
            Assert.AreEqual(1.001m, vm.IID2Measurements[0].C);
        }

        [TestMethod]
        public void BB6_InsulationProcessViewModel_To_DB()
        {
            var vm = new BB6_InsulationProcessViewModel();
            PopulateStepViewModel(vm);
            vm.InsulationMadeBy = "Test Insulator";
            vm.GritBlastedBy = "Test Gritter";
            vm.SlingerRingCutOutBy = "Test Ringer";
            vm.InsulationInstalledBy = "Test Installer";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 6);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 6,
                    ProcessId = 1,
                    Title = "Test1: InsulationProcessViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 6);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB6_InsulationProcessViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Insulator", vm.InsulationMadeBy);
            Assert.AreEqual("Test Gritter", vm.GritBlastedBy);
            Assert.AreEqual("Test Ringer", vm.SlingerRingCutOutBy);
            Assert.AreEqual("Test Installer", vm.InsulationInstalledBy);
        }

        [TestMethod]
        public void BB7_CleanupProcessViewModel_To_DB()
        {
            var vm = new BB7_CleanupProcessViewModel();
            PopulateStepViewModel(vm);
            vm.CleanUpBy = "Test Cleaner";
            vm.SlingerRingCutOutBy = "Test Ringer";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 7);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 7,
                    ProcessId = 1,
                    Title = "Test1: CleanupProcessViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 7);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB7_CleanupProcessViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Cleaner", vm.CleanUpBy);
            Assert.AreEqual("Test Ringer", vm.SlingerRingCutOutBy);
        }

        [TestMethod]
        public void BB8_FinalMachineInspectionViewModel_To_DB()
        {
            var vm = new BB8_FinalMachineInspectionViewModel();
            PopulateStepViewModel(vm);
            vm.IsSplitLinesVerified = true;
            vm.IsDowelChecksGood = true;
            vm.IsBondVerified = true;
            vm.IsClean = false;
            vm.ReadyForFinalMachineBy = "Test Final";
            vm.IsFlaggedForCustomerApproval = true;
            vm.ProblemResolution = "Test Resolution";
            vm.SizedApprovedBy = "Test Size";
            
            vm.OD1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.012m,
                B = 1.012m,
                C = 1.002m,
            });

            vm.OD2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.013m,
                B = 1.013m,
                C = 1.003m,
            });

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();
            string filename = @"BB8_FinalMachineInspectionViewModel.json";
            string filepath = Path.Combine(Properties.Settings.Default.BaseTestPath, filename);

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 8);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 8,
                    ProcessId = 1,
                    Title = "Test1: FinalMachineInspectionViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 8);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB8_FinalMachineInspectionViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual(true, vm.IsSplitLinesVerified);
            Assert.AreEqual(true, vm.IsDowelChecksGood);
            Assert.AreEqual(true, vm.IsBondVerified);
            Assert.AreEqual(false, vm.IsClean);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].A);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].B);
            Assert.AreEqual(1.002m, vm.OD1Measurements[0].C);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].A);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].B);
            Assert.AreEqual(1.003m, vm.OD2Measurements[0].C);
            Assert.AreEqual("Test Final", vm.ReadyForFinalMachineBy);
            Assert.AreEqual(true, vm.IsFlaggedForCustomerApproval);
            Assert.AreEqual("Test Resolution", vm.ProblemResolution);
            Assert.AreEqual("Test Size", vm.SizedApprovedBy);
        }

        [TestMethod]
        public void BB9_FinishBoreProcessViewModel_To_DB()
        {
            var vm = new BB9_FinishBoreProcessViewModel();
            PopulateStepViewModel(vm);
            vm.ShaftSize = "1.00";
            vm.ClearanceSize = "0.01";
            vm.ShimSize = "0.02";
            vm.Tolerance = "0.005";
            vm.Notes = "Test Notes";
            vm.CustomerBoreSize = "0.10";
            vm.CustomerBoreSizeHorizontal = "0.05";
            vm.CustomerODSize.Add(new PartDiameterMeasurement
            {
                ODComment = "0.01"
            });
            vm.Runout1FrontSize = 0.01m;
            vm.Runout1BackSize = 0.002m;
            vm.Runout1MiddleSize = 0.03m;
            vm.Runout2FaceSize = 0.004m;
            vm.Runout2BoreSize = 0.005m;
            vm.FinishedBoreBy = "Test Finisher";
            vm.IsVerifiedSizesOk = true;

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 9);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 9,
                    ProcessId = 1,
                    Title = "Test1: FinishBoreProcessViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 9);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB9_FinishBoreProcessViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual(1.00m, vm.ShaftSize);
            Assert.AreEqual(0.01m, vm.ClearanceSize);
            Assert.AreEqual(0.02m, vm.ShimSize);
            Assert.AreEqual(0.005m, vm.Tolerance);
            Assert.AreEqual("Test Notes", vm.Notes);
            Assert.AreEqual(0.10m, vm.CustomerBoreSize);
            Assert.AreEqual(0.05m, vm.CustomerBoreSizeHorizontal);
            Assert.AreEqual(0.01m, vm.CustomerODSize);
            Assert.AreEqual(0.01m, vm.Runout1FrontSize);
            Assert.AreEqual(0.002m, vm.Runout1BackSize);
            Assert.AreEqual(0.03m, vm.Runout1MiddleSize);
            Assert.AreEqual(0.004m, vm.Runout2FaceSize);
            Assert.AreEqual(0.005m, vm.Runout2BoreSize);
            Assert.AreEqual("Test Finisher", vm.FinishedBoreBy);
            Assert.AreEqual(true, vm.IsVerifiedSizesOk);
        }

        [TestMethod]
        public void BB10_FinalAssemblyViewModel_To_DB()
        {
            var vm = new BB10_FinalAssemblyViewModel();
            PopulateStepViewModel(vm);
            vm.MillWorkBy = "Test Miller";
            vm.DeburredBy = "Test Deburrer";
            vm.PartsInstalledBy = "Test Part Installer";
            vm.VerifiedBy = "Test Verifier";

            var part = new Part
            {
                PartId = 1,
                JobId = 2,
                PartTypeId = 3,
                PartContactId = 4,
                PartStatusId = 5,
                WorkScope = "Test Work Scope",
                RequiredDate = new DateTime(2016, 1, 1),
                ShipByDate = new DateTime(2016, 1, 2),
                ShippedDate = new DateTime(2016, 1, 2),
                NonConformanceReportNotes = "Test Non Conformance Notes",
                RequiresPT = true,
                RequiresUT = true,
                PartProcessId = 1,
                ItemCode = "IC123",
                IdentifyingInfo = "Test Info",
            };

            var iia = new IncomingInspectionAccessory
            {
                IncomingInspectionAccessoryId = 1,
                PartId = 2,
                Quantity = 3,
                Part = part,
                Name = "Test IIA",
            };

            vm.PartsAccessories.Add(iia);

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 10);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 10,
                    ProcessId = 1,
                    Title = "Test1: FinalAssemblyViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 10);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB10_FinalAssemblyViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Miller", vm.MillWorkBy);
            Assert.AreEqual("Test Deburrer", vm.DeburredBy);
            Assert.AreEqual("Test Part Installer", vm.PartsInstalledBy);
            Assert.AreEqual("Test Verifier", vm.VerifiedBy);
            Assert.AreEqual(1, vm.PartsAccessories[0].IncomingInspectionAccessoryId);
            Assert.AreEqual(2, vm.PartsAccessories[0].PartId);
            Assert.AreEqual("Test IIA", vm.PartsAccessories[0].Name);
            Assert.AreEqual(3, vm.PartsAccessories[0].Quantity);
            Assert.AreEqual(1, vm.PartsAccessories[0].Part.PartId);
            Assert.AreEqual(2, vm.PartsAccessories[0].Part.JobId);
            Assert.AreEqual(3, vm.PartsAccessories[0].Part.PartTypeId);
            Assert.AreEqual(4, vm.PartsAccessories[0].Part.PartContactId);
            Assert.AreEqual(5, vm.PartsAccessories[0].Part.PartStatusId);
            Assert.AreEqual("Test Work Scope", vm.PartsAccessories[0].Part.WorkScope);
            Assert.AreEqual(new DateTime(2016, 1, 1), vm.PartsAccessories[0].Part.RequiredDate);
            Assert.AreEqual(new DateTime(2016, 1, 2), vm.PartsAccessories[0].Part.ShipByDate);
            Assert.AreEqual(new DateTime(2016, 1, 2), vm.PartsAccessories[0].Part.ShippedDate);
            Assert.AreEqual("Test Non Conformance Notes", vm.PartsAccessories[0].Part.NonConformanceReportNotes);
            Assert.AreEqual(true, vm.PartsAccessories[0].Part.RequiresPT);
            Assert.AreEqual(true, vm.PartsAccessories[0].Part.RequiresUT);
            Assert.AreEqual(1, vm.PartsAccessories[0].Part.PartProcessId);
            Assert.AreEqual("IC123", vm.PartsAccessories[0].Part.ItemCode);
            Assert.AreEqual("Test Info", vm.PartsAccessories[0].Part.IdentifyingInfo);
            Assert.AreEqual(null, vm.PartsAccessories[0].Part.Contact);
            Assert.AreEqual(null, vm.PartsAccessories[0].Part.Job);
        }
        
        [TestMethod]
        public void BB11_FinalInspectionViewModel_To_DB()
        {
            var vm = new BB11_FinalInspectionViewModel();
            PopulateStepViewModel(vm);
            vm.OverallLength = 1.1m;
            vm.TCInstalledQuantity = 2;
            vm.IsAcceptable = true;
            vm.IsCustomerCalled = true;
            vm.FinalInspectionBy = "Test Inspector";
            vm.ReturnStepId = 0;
            vm.IsJobFailed = false;
            vm.ID1Measurements = new List<BoreMeasurementViewModel>();
            vm.OD1Measurements = new List<BoreMeasurementViewModel>();
            vm.ID2Measurements = new List<BoreMeasurementViewModel>();
            vm.OD2Measurements = new List<BoreMeasurementViewModel>();
            vm.SealMeasurements = new List<BoreMeasurementViewModel>();

            vm.ID1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.ID1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.01m,
                B = 1.01m,
                C = 1.00m,
            });

            vm.ID2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.ID2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.011m,
                B = 1.011m,
                C = 1.001m,
            });

            vm.OD1Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD1Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.012m,
                B = 1.012m,
                C = 1.002m,
            });

            vm.OD2Measurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.OD2Measurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.013m,
                B = 1.013m,
                C = 1.003m,
            });

            vm.SealMeasurements.Add(new BoreMeasurementViewModel
            {
                Index = vm.SealMeasurements.Count + 1,
                MeasurementId = 0,
                MeasurementGroupId = null,
                A = 1.014m,
                B = 1.014m,
                C = 1.004m,
            });

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 11);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 11,
                    ProcessId = 1,
                    Title = "Test1: FinalInspectionViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 11);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB11_FinalInspectionViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual(1.01m, vm.ID1Measurements[0].A);
            Assert.AreEqual(1.01m, vm.ID1Measurements[0].B);
            Assert.AreEqual(1.00m, vm.ID1Measurements[0].C);
            Assert.AreEqual(1.011m, vm.ID2Measurements[0].A);
            Assert.AreEqual(1.011m, vm.ID2Measurements[0].B);
            Assert.AreEqual(1.001m, vm.ID2Measurements[0].C);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].A);
            Assert.AreEqual(1.012m, vm.OD1Measurements[0].B);
            Assert.AreEqual(1.002m, vm.OD1Measurements[0].C);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].A);
            Assert.AreEqual(1.013m, vm.OD2Measurements[0].B);
            Assert.AreEqual(1.003m, vm.OD2Measurements[0].C);
            Assert.AreEqual(1.014m, vm.SealMeasurements[0].A);
            Assert.AreEqual(1.014m, vm.SealMeasurements[0].B);
            Assert.AreEqual(1.004m, vm.SealMeasurements[0].C);
            Assert.AreEqual(1.1m, vm.OverallLength);
            Assert.AreEqual(2, vm.TCInstalledQuantity);
            Assert.AreEqual(true, vm.IsAcceptable);
            Assert.AreEqual(true, vm.IsCustomerCalled);
            Assert.AreEqual("Test Inspector", vm.FinalInspectionBy);
            Assert.AreEqual(0, vm.ReturnStepId);
            Assert.AreEqual(false, vm.IsJobFailed);
        }

        [TestMethod]
        public void BB12_DeliveryViewModel_To_DB()
        {
            var vm = new BB12_DeliveryViewModel();
            PopulateStepViewModel(vm);
            vm.PackedBy = "Test Packer";
            vm.ShippedVia = "Test Shipper";
            vm.ShipDate = new DateTime(2016, 1, 1);
            vm.RequiredDate = new DateTime(2016, 1, 5);
            vm.TrackingNumber = "Z123ABC000";

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 12);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 12,
                    ProcessId = 1,
                    Title = "Test1: DeliveryViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 12);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB12_DeliveryViewModel>(json);
            AssertStepViewModel(vm);
            Assert.AreEqual("Test Packer", vm.PackedBy);
            Assert.AreEqual("Test Shipper", vm.ShippedVia);
            Assert.AreEqual(2016, vm.ShipDate.Value.Year);
            Assert.AreEqual(1, vm.ShipDate.Value.Month);
            Assert.AreEqual(1, vm.ShipDate.Value.Day);
            Assert.AreEqual(2016, vm.RequiredDate.Value.Year);
            Assert.AreEqual(1, vm.RequiredDate.Value.Month);
            Assert.AreEqual(5, vm.RequiredDate.Value.Day);
            Assert.AreEqual("Z123ABC000", vm.TrackingNumber);
        }
        
        [TestMethod]
        public void BB13_FinalSignOffViewModel_To_DB()
        {
            var vm = new BB13_FinalSignOffViewModel();
            PopulateStepViewModel(vm);

            // Serialize
            var ms = new MemoryStream();
            JsonSerializer serializer = new JsonSerializer();

            var jsonTextWriter = new JsonTextWriter(new StreamWriter(ms));
            serializer.Serialize(jsonTextWriter, vm);
            jsonTextWriter.Flush();
            ms.Position = 0;

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            var step = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 13);

            if (step == null)
            {
                // Serialize a record to the database
                step = new Step
                {
                    StepNumber = 13,
                    ProcessId = 1,
                    Title = "Test1: FinalSignOffViewModel_To_DB",
                    Version = "1.0",
                    DataType = "viewmodel",
                    StringValue = json,
                    Notes = "",
                    LastRequestedBy = "",
                    Requested = null,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };

                _db.Steps.Add(step);
                _db.SaveChanges();
            }

            // Deserialize
            var stepagain = _db.Steps.FirstOrDefault(x => x.ProcessId == 1 && x.StepNumber == 13);
            json = stepagain.StringValue;
            vm = JsonConvert.DeserializeObject<BB13_FinalSignOffViewModel>(json);
            AssertStepViewModel(vm);
        }

    }
}