-- =============================================
-- Author:		John Cunningham
-- Create date: 10/2/2015
-- Description:	Updates the EmployeeSkill table 
--              when a checkbox is clicked in the
--				Skills grid.
-- =============================================
CREATE PROCEDURE UpdateEmployeeSkill
	-- Add the parameters for the stored procedure here
	@empID int,
	@skillID int,
	@hasSkill bit

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	-- Insert statements for procedure here
	if EXISTS(SELECT EmployeeId,SkillId FROM EmployeeSkill WHERE EmployeeId = @empID AND SkillId = @skillID)
		BEGIN
			DELETE FROM EmployeeSkill WHERE EmployeeId = @empID AND SkillId = @skillID
		END
	else
		BEGIN
			INSERT INTO EmployeeSkill (EmployeeId,SkillId) VALUES (@empID,@skillID)
		END
END