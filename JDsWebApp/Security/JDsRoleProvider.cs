using System;
using System.Web.Security;
using JDsDataModel;
using NLog;

namespace JDsWebApp.Security
{
    public class JDsRoleProvider : RoleProvider
    {
        private static readonly JDsDBEntities _db = new JDsDBEntities();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override string ApplicationName
        {
            get { return "JDsWebApp"; }
            set { throw new NotImplementedException(); }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            // TODO
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            // TODO
            throw new NotImplementedException();
        }

        public void RemoveUserFromRoles(string username, string[] roleNames)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
