/*
	Adds mock employees to the database - Alan Nguyen
*/

USE [JDsBabbittBearingDB]
GO

SET IDENTITY_INSERT [dbo].[Skill] ON
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (1,'Management')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (2,'Incoming Inspection')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (3,'Centrifugal Cast')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (4,'Final Inspection')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (5,'Packaging')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (6,'Fill Propane')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (7,'Fork Lift')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (8,'Billing')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (9,'Start/Edit Jobs')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (10,'Tinning')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (11,'Roughout Bearing')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (12,'Slinger Ring Cutout')
INSERT INTO [dbo].[Skill] ([SkillId],[Name]) VALUES (13,'Inspection for Final Machining')
SET IDENTITY_INSERT [dbo].[Skill] OFF


SET IDENTITY_INSERT [dbo].[Address] ON
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (1,
			'3001 Hall Office Park',
            '',
            'Frisco',
            'Texas',
            '75034',
            'US')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (2,
			'5600 Nebraska Furniture Mart Dr',
            '',
            'The Colony',
            'Texas',
            '75056',
            'US')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (3,
			'8801 Ohio Dr',
            '',
            'Plano',
            'Texas',
            '75024',
            'US')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (4,
			'4620 TX-121',
            '',
            'Lewisville',
            'Texas',
            '75056',
            'US')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (5,
			'2525 E Abram St',
            '',
            'Arlington',
            'Texas',
            '76010',
            'US')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (6,
			'1 Microsoft Way',
            '',
            'Redmond',
            'Washington',
            '98052',
            'US')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (7,
			'38303 Michigan Ave',
            '',
            'Wayne',
            'Michigan',
            '48184',
            'US')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (8,
			'New Court',
            'St Swithin Ln',
            'London',
            'Middlesex',
            'EC4N 8AL',
            'UK')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (9,
			'VW-Straße 103',
            '',
            'Wolfsburg',
            '',
            '38440 ',
            'Germany')
GO
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (10,
			'〒198-0024',
            '',
            'Tokyo',
            'Oume',
            '',
            'Japan')
GO
SET IDENTITY_INSERT [dbo].[Address] OFF


SET IDENTITY_INSERT [dbo].[Employee] ON
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (1, 1,
            'Joe Dev',
            'joe.dev@codeauthority.com',
            '972-987-4321',
            '2015-09-15',
            '1111',
            'Mary Dev',
			'972-987-4322',
            0,
            0,
            '',1)
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (2, 2,
            'Jay Wooden',
            'jay.wooden@nfmart.com',
            '817-987-4321',
            '2000-09-15',
            '2222',
            'Jill Wooden',
			'817-987-4322',
            500,
            400,
            '',1)		
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (3, 3,
            'Wall World',
            'wall.worldn@walmart.com',
            '682-987-4321',
            '2010-09-15',
            '3333',
            'Irene World',
			'682-987-4322',
            300,
            250,
            '',1)
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (4, 4,
            'Rod McMullen',
            'rod.mcmullen@krg.com',
            '682-123-1234',
            '2005-01-01',
            '4444',
            'Kristine McMullen',
			'682-123-1235',
            300,
            270,
            '',1)
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (5, 5,
            'Juan Jimenez',
            'jj@gmarlington.com',
            '817-456-8253',
            '2007-01-01',
            '5555',
            'Sofia Jiminez',
			'817-456-8254',
            280,
            200,
            '',1)
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (6, 6,
            'Bill Gates',
            'bill.gates@ms.com',
            '800-642-7676',
            '2008-02-02',
            '6666',
            'Melinda Gates',
			'800-642-7677',
            200,
            100,
            '',1)
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (7, 7,
            'Henley Ford',
            'henley.ford@frd.com',
            '800-392-3673',
            '2009-04-13',
            '7777',
            'Helsig Ford',
			'800-392-3674',
            400,
            320,
            '',1)
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (8, 8,
            'Amschel Rothschild',
            'amschel.rothschild@roths.com',
            '800-392-1234',
            '2001-05-15',
            '8888',
            'Amy Rothschild',
			'800-392-1235',
            450,
            400,
            '',1)
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (9, 9,
            'Volks Wagen',
            'v.w@vw.com',
            '800-822-8987',
            '2004-05-15',
            '9999',
            'Weelie Wagen',
			'800-822-8988',
            525,
            470,
            '',1)
INSERT INTO [dbo].[Employee]
           ([EmployeeId],[AddressId],[Name],[Email],[Phone],[HireDate],[PIN],[EmergencyContact],[EmergencyPhone],[VacationHoursEarned],[VacationHoursUsed],[Notes],[IsActive])
     VALUES
           (10, 10,
            'Atsutoshi Nishida',
            'atsutoshi.nishida@tsb.com',
            '800-618-4444',
            '2007-05-15',
            '0000',
            'Akira Nishida',
			'800-618-4445',
            600,
            575,
            '',1)
GO
SET IDENTITY_INSERT [dbo].[Employee] OFF


INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES (1,1)
INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES (1,2)
INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES (2,1)
INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES (2,4)
INSERT INTO [dbo].[EmployeeSkill] ([EmployeeId],[SkillId]) VALUES (2,6)
GO


SET IDENTITY_INSERT [dbo].[Certificate] ON
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (1,'2005-05-10', '2017-01-03', 'Note 1', 'Bearing Company', 'Certificate 1', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (2,'2006-06-11', '2017-02-04', 'Note 2', 'Bearing Company', 'Certificate 2', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (3,'2007-07-12', '2017-03-05', 'Note 3', 'Bearing Company', 'Certificate 3', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (4,'2008-08-13', '2017-04-06', 'Note 4', 'Bearing Company', 'Certificate 4', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (5,'2009-09-14', '2017-05-07', 'Note 5', 'Bearing Company', 'Certificate 5', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (6,'2010-10-15', '2017-06-08', 'Note 6', 'Bearing Company', 'Certificate 6', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (7,'2011-11-16', '2017-07-09', 'Note 7', 'Bearing Company', 'Certificate 7', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (8,'2012-12-17', '2017-08-10', 'Note 8', 'Bearing Company', 'Certificate 8', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (9,'2013-01-18', '2017-09-11', 'Note 9', 'Bearing Company', 'Certificate 9', null)
INSERT INTO [dbo].[Certificate] ([CertificateId],[CertificateDate],[CertificateExpires],[Notes],[CompanyName],[Name],[CertificateFileId])
     VALUES (10,'2014-02-19', '2017-10-12', 'Note 10', 'Bearing Company', 'Certificate 10', null)
GO
SET IDENTITY_INSERT [dbo].[Certificate] OFF


SET IDENTITY_INSERT [dbo].[Address] ON
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (101,
			'1 Address Lane 1','',
            'San Francisco',
            'CA','94101','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (102,
			'2 Address Street 2','',
            'Lansing',
            'MI','48917','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (103,
			'3 Address Street 3','',
            'New Orleans',
            'LA','70112','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (104,
			'4 Address Street 4','',
            'Fort Worth',
            'TX','76102','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (105,
			'5 Address Street 5','',
            'Anna',
            'OH','45302','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (106,
			'6 Address Street 6','',
            'ALCOA',
            'TN','37701','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (107,
			'7 Address Street 7','',
            'Dallas',
            'TX','75237','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (108,
			'8 Address Street 8','',
            'Mesa',
            'AZ','85206','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (109,
			'9 Address Street 9','',
            'Lisbon',
            'NH','03585','US')
INSERT INTO [dbo].[Address] ([AddressId],[AddressLine1],[AddressLine2],[City],[State],[Zip],[Country])
     VALUES
           (110,
			'9 Address Street 9','',
            'Chicago',
            'IL','60602','US')
GO
SET IDENTITY_INSERT [dbo].[Address] OFF


SET IDENTITY_INSERT [dbo].[Customer] ON
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (1,null,101,101,'01 Company','01CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (2,null,102,102,'02 Company','02CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (3,null,103,103,'03 Company','03CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (4,null,104,104,'04 Company','04CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (5,null,105,105,'05 Company','05CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (6,null,106,106,'06 Company','06CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (7,null,107,107,'07 Company','07CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (8,null,108,108,'08 Company','08CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (9,null,109,109,'09 Company','09CODE','',1)
INSERT INTO [dbo].[Customer] ([CustomerId],[PrimaryContactId],[BillingAddressId],[ShippingAddressId],[CompanyName],[Code],[Notes],[IsActive]) VALUES (10,null,110,110,'10 Company','10CODE','',1)
GO
SET IDENTITY_INSERT [dbo].[Customer] OFF


SET IDENTITY_INSERT [dbo].[Contact] ON
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (1,1,'01 First','01 Contact','01.01@test.com', '111-111-1111','111-111-1112','111-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (2,2,'02 Second','02 Contact','02.02@test.com', '211-111-1111','211-111-1112','211-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (3,3,'03 Third','03 Contact','03.03@test.com', '311-111-1111','311-111-1112','311-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (4,4,'04 Forth','04 Contact','04.04@test.com', '411-111-1111','411-111-1112','411-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (5,5,'05 Fifth','05 Contact','05.05@test.com', '511-111-1111','511-111-1112','511-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (6,6,'06 Sixth','06 Contact','06.06@test.com', '611-111-1111','611-111-1112','611-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (7,7,'07 Seventh','07 Contact','07.07@test.com', '711-111-1111','711-111-1112','711-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (8,8,'08 Eighth','08 Contact','08.08@test.com', '811-111-1111','811-111-1112','811-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (9,9,'09 Ninth','09 Contact','09.09@test.com', '911-111-1111','911-111-1112','911-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (10,10,'10 Tenth','10 Contact','10.10@test.com', '101-111-1111','101-111-1112','101-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (11,1,'11 Eleventh','11 Contact','11.11@test.com', '110-111-1111','110-111-1112','110-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (12,2,'12 Twelveth','12 Contact','12.12@test.com', '121-111-1111','121-111-1112','121-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (13,3,'13 Thirteenth','13 Contact','13.13@test.com', '131-111-1111','131-111-1112','131-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (14,4,'14 Forthteenth','14 Contact','14.14@test.com', '141-111-1111','141-111-1112','141-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (15,5,'15 Fifthteenth','15 Contact','15.15@test.com', '151-111-1111','151-111-1112','151-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (16,6,'16 Sixtheenth','16 Contact','16.16@test.com', '161-111-1111','161-111-1112','161-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (17,7,'17 Seventheenth','17 Contact','17.17@test.com', '171-111-1111','171-111-1112','171-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (18,8,'18 Eighteenth','18 Contact','18.18@test.com', '181-111-1111','181-111-1112','181-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (19,9,'19 Nineteenth','19 Contact','19.19@test.com', '191-111-1111','191-111-1112','191-111-1113','',1)
INSERT INTO [dbo].[Contact] ([ContactId],[CustomerId],[FirstName],[LastName],[Email],[WorkPhone],[CellPhone],[Fax],[Notes],[IsActive])
     VALUES (20,10,'20 Twentieth','20 Contact','20.20@test.com', '201-111-1111','201-111-1112','201-111-1113','',1)
GO
SET IDENTITY_INSERT [dbo].[Contact] OFF

UPDATE [dbo].[Customer]
   SET [PrimaryContactId] = [CustomerId]
GO

SET IDENTITY_INSERT [dbo].[JobStatus] ON
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (1,'New')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (2,'In Progress')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (3,'On Hold')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (4,'Blocked')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (5,'Finished')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (6,'Not Billed')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (7,'Billed - Unpaid')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (8,'Billed - Paid Partial')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (9,'Billed - Paid Full')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (10,'Shipped')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (11,'Cancelled')
INSERT INTO [dbo].[JobStatus] ([JobStatusId],[Name]) VALUES (12,'Closed')
GO
SET IDENTITY_INSERT [dbo].[JobStatus] OFF



SET IDENTITY_INSERT [dbo].[Job] ON
INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (1,null,1,1
		   ,'C 0001'
		   ,'PO 0001'
		   ,'2014-05-10'
		   ,'2014-07-10'
		   ,'2014-07-04'
		   ,null
		   ,null
		   ,null,null
           ,'Job 01 Notes',null,1)

INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (2,null,2,2
		   ,'C 0002'
		   ,'PO 0002'
		   ,'2015-10-20'
		   ,'2015-12-20'
		   ,'2015-12-14'
		   ,null
		   ,null
		   ,null,null
           ,'Job 02 Notes',null,1)

INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (3,null,3,3
		   ,'C 0003'
		   ,'PO 0003'
		   ,'2015-03-01'
		   ,'2015-05-10'
		   ,'2015-05-03'
		   ,null
		   ,null
		   ,null,null
           ,'Job 03 Notes',null,1)
		   
INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (4,null,4,4
		   ,'C 0004'
		   ,'PO 0004'
		   ,'2015-03-01'
		   ,'2015-05-10'
		   ,'2015-05-03'
		   ,null
		   ,null
		   ,null,null
           ,'Job 04 Notes',null,1)
		   
INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (5,null,5,5
		   ,'C 0005'
		   ,'PO 0005'
		   ,'2015-01-20'
		   ,'2015-03-28'
		   ,'2015-03-24'
		   ,null
		   ,null
		   ,null,null
           ,'Job 05 Notes',null,1)
		   
INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (6,null,6,6
		   ,'C 0006'
		   ,'PO 0006'
		   ,'2015-09-10'
		   ,'2015-11-10'
		   ,'2015-11-04'
		   ,null
		   ,null
		   ,0,0
           ,'Job 06 Notes',null,1)

INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (7,null,7,7
		   ,'C 0007'
		   ,'PO 0007'
		   ,'2015-05-10'
		   ,'2015-07-10'
		   ,'2015-07-04'
		   ,null
		   ,'2015-06-25'
		   ,0,0
           ,'Job 07 Notes',null,1)

INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (8,null,8,8
		   ,'C 0008'
		   ,'PO 0008'
		   ,'2014-05-10'
		   ,'2014-07-10'
		   ,'2014-07-04'
		   ,'2014-07-01'
		   ,'2014-06-25'
		   ,0,0
           ,'Job 08 Notes',null,1)

INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (9,null,9,9
		   ,'C 0009'
		   ,'PO 0009'
		   ,'2014-05-10'
		   ,'2014-07-10'
		   ,'2014-07-04'
		   ,'2014-07-01'
		   ,'2014-06-25'
		   ,0,0
           ,'Job 09 Notes',null,1)

INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (10,null,10,10
		   ,'C 0010'
		   ,'PO 0010'
		   ,'2014-05-10'
		   ,'2014-07-10'
		   ,'2014-07-04'
		   ,'2014-07-01'
		   ,'2014-06-25'
		   ,0,0
           ,'Job 10 Notes',null,1)

INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (11,null,11,11
		   ,'C 0010'
		   ,'PO 0001'
		   ,'2014-05-10'
		   ,'2014-07-10'
		   ,'2014-07-04'
		   ,null
		   ,'2014-06-25'
		   ,0,0
           ,'Job 11 Notes',null,1)

INSERT INTO [dbo].[Job] ([JobId],[CreatedByEmployeeId],[CustomerContactId],[JobStatusId],[CustomerJobNumber],[PurchaseOrderNumber],[ReceivedDate]
		   ,[RequiredDate],[ShipByDate],[ShippedDate],[BilledDate],[OvertimeRequired],[HoldForCustomerApproval],[JobNotes],[LastUpdated],[IsActive])
     VALUES (12,null,12,12
		   ,'C 0012'
		   ,'PO 0012'
		   ,'2015-05-10'
		   ,'2015-07-10'
		   ,'2015-07-04'
		   ,'2015-07-01'
		   ,'2015-06-25'
		   ,0,0
           ,'Job 12 Notes',null,1)
GO
SET IDENTITY_INSERT [dbo].[Job] OFF

SET IDENTITY_INSERT [dbo].[Activity] ON
INSERT INTO [dbo].[Activity] ([ActivityId],[Name],[IsActive]) VALUES (1,'Approved',1)
GO
SET IDENTITY_INSERT [dbo].[Activity] OFF

SET IDENTITY_INSERT [dbo].[PartType] ON
INSERT INTO [dbo].[PartType] ([PartTypeId],[Name]) VALUES (1,'Babbitt Bearing')
INSERT INTO [dbo].[PartType] ([PartTypeId],[Name]) VALUES (2,'Slinger Ring')
INSERT INTO [dbo].[PartType] ([PartTypeId],[Name]) VALUES (3,'Oil Shield')
GO
SET IDENTITY_INSERT [dbo].[PartType] OFF

SET IDENTITY_INSERT [dbo].[PartStatus] ON
INSERT INTO [dbo].[PartStatus] ([PartStatusId],[Name]) VALUES (1,'Not Started')
INSERT INTO [dbo].[PartStatus] ([PartStatusId],[Name]) VALUES (2,'In Progress')
INSERT INTO [dbo].[PartStatus] ([PartStatusId],[Name]) VALUES (3,'Finished')
INSERT INTO [dbo].[PartStatus] ([PartStatusId],[Name]) VALUES (4,'Blocked')
GO
SET IDENTITY_INSERT [dbo].[PartStatus] OFF

SET IDENTITY_INSERT [dbo].[ProcessType] ON
INSERT INTO [dbo].[ProcessType] ([ProcessTypeId],[Name],[Description],[IsActive]) VALUES (1,'Fan','Fan',1)
INSERT INTO [dbo].[ProcessType] ([ProcessTypeId],[Name],[Description],[IsActive]) VALUES (2,'Motor','Motor',1)
INSERT INTO [dbo].[ProcessType] ([ProcessTypeId],[Name],[Description],[IsActive]) VALUES (3,'Turbine','Turbine',1)
INSERT INTO [dbo].[ProcessType] ([ProcessTypeId],[Name],[Description],[IsActive]) VALUES (4,'Generator','Generator',1)
GO
SET IDENTITY_INSERT [dbo].[ProcessType] OFF


SET IDENTITY_INSERT [dbo].[Part] ON
INSERT INTO [dbo].[Part]
           ([PartId]
		   ,[JobId]
           ,[PartTypeId]
           ,[PartStatusId]
           ,[PartContactId]
           ,[WorkScope]
           ,[RequiredDate]
           ,[ShipByDate]
           ,[ShippedDate]
           ,[NonConformanceReportNotes]
           ,[RequiresPT]
           ,[RequiresUT]
           ,[ItemCode]
           ,[IdentifyingInfo]
		   ,[IsActive])
     VALUES
           (1, 1, 1, 1, 1
		   ,'T/C Required, UT, Insulate'
           ,'2015-11-10'
           ,'2015-11-07'
           ,null
           ,''
           ,0
           ,1
           ,'P123AD00000111'
           ,''
		   ,1)
GO
SET IDENTITY_INSERT [dbo].[Part] OFF


SET IDENTITY_INSERT [dbo].[Process] ON
INSERT INTO [dbo].[Process] ([ProcessId],[ProcessTypeId],[PartId],[Name],[Description],[Notes],[IsActive]) VALUES (1,1,1,'TEST Babbit Bearing for Fan','','',1)
GO
SET IDENTITY_INSERT [dbo].[Process] OFF