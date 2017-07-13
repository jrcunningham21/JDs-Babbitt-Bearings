--USE [JDsBabbittBearingDB]
--GO
--BEGIN TRANSACTION;
--BEGIN TRY
--	DELETE FROM [dbo].[Step];
--	DELETE FROM [dbo].[Process];
--	DELETE FROM [dbo].[PartPhoto];
--	DELETE FROM [dbo].[PartFile];
--	DELETE FROM [dbo].[PartTest];
--	DELETE FROM [dbo].[IncomingInspectionAccessory];
--	DELETE FROM [dbo].[Part];
--	DELETE FROM [dbo].[ChangeLogEntry];
--	DELETE FROM [dbo].[Job];
--	DBCC CHECKIDENT ('[Job]', RESEED, 10491);

--	DELETE FROM [dbo].[AutoComplete];	

--	INSERT INTO [dbo].[AutoComplete] ([ControlId],[Value]) VALUES(1,'');
--	INSERT INTO [dbo].[AutoComplete] ([ControlId],[Value]) VALUES(2,'');
--	INSERT INTO [dbo].[AutoComplete] ([ControlId],[Value]) VALUES(3,'');
--	INSERT INTO [dbo].[AutoComplete] ([ControlId],[Value]) VALUES(101,'');

--	DELETE FROM [dbo].[Vacation];
--	DELETE FROM [dbo].[SignOff];
--	DELETE FROM [dbo].[TimesheetEntry];
--	DELETE FROM [dbo].[EmployeeFile];
--	DELETE FROM [dbo].[EmployeeSkill];
--	DELETE FROM [dbo].[Employee];
--	DBCC CHECKIDENT ('[Employee]', RESEED, 0);

--	ALTER TABLE [dbo].[Contact] NOCHECK CONSTRAINT ALL
--	ALTER TABLE [dbo].[Customer] NOCHECK CONSTRAINT ALL

--	DELETE FROM [dbo].[Contact];
--	DELETE FROM [dbo].[Customer];

--	ALTER TABLE [dbo].[Contact] CHECK CONSTRAINT ALL
--	ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT ALL

--	DELETE FROM [dbo].[Address];
--	DELETE FROM [dbo].[Certificate];
--	DELETE FROM [dbo].[CertificateFile];

--	INSERT INTO [dbo].[Employee] ([Name],[Phone],[PIN],[IsActive]) VALUES('Jarad','903-816-2323','1224',1);
--	INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES(1,1);
--	INSERT INTO [dbo].[Employee] ([Name],[Phone],[PIN],[IsActive]) VALUES('JP','903-815-7123','2424',1);
--	INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES(2,1);
--	INSERT INTO [dbo].[Employee] ([Name],[Phone],[PIN],[IsActive]) VALUES('Justin','903-815-5776','1978',1);
--	INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES(3,1);
--	INSERT INTO [dbo].[Employee] ([Name],[Phone],[PIN],[IsActive]) VALUES('Greg','972-658-3312','0002',1);
--	INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES(4,1);
	
--END TRY
--BEGIN CATCH
--    SELECT 
--        ERROR_NUMBER() AS ErrorNumber
--        ,ERROR_SEVERITY() AS ErrorSeverity
--        ,ERROR_STATE() AS ErrorState
--        ,ERROR_PROCEDURE() AS ErrorProcedure
--        ,ERROR_LINE() AS ErrorLine
--        ,ERROR_MESSAGE() AS ErrorMessage;

--    IF @@TRANCOUNT > 0
--        ROLLBACK TRANSACTION;
--END CATCH;

--IF @@TRANCOUNT > 0
--    COMMIT TRANSACTION;
--GO