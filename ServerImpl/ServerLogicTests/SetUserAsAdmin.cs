using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using DB;

namespace ServerLogicTests
{
    [TestClass]
    public class SetUserAsAdmin
    {
        private IServer _server;
        private int _i;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new DB.FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
            _i = _server.register("newuser@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name").Item2;
        }

        [TestMethod]
        public void setUserAsAdminSuccessfully()
        {
            Assert.IsTrue(_server.setUserAsAdmin(Users.USER_UNIQUE_INT, "newuser@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void setUserAsAdminUserNotLoggedIn()
        {
            _server.logout(Users.USER_UNIQUE_INT);
            Assert.IsFalse(_server.setUserAsAdmin(Users.USER_UNIQUE_INT, "newuser@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void setUserAsAdminWrongUserId()
        {
            Assert.IsFalse(_server.setUserAsAdmin(Users.USER_UNIQUE_INT - 1, "newuser@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void setUserAsAdminUserIsNotAnAdmin()
        {
            _server.register("newuser2@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t = _server.login("newuser2@gmail.com", "password");
            Assert.IsFalse(_server.setUserAsAdmin(t.Item2, "newuser@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void setUserAsAdminUnregisteredUser()
        {
            Assert.IsFalse(_server.setUserAsAdmin(Users.USER_UNIQUE_INT, "newuser2@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void setUserAsAdminUsernameIsNull()
        {
            Assert.IsFalse(_server.setUserAsAdmin(Users.USER_UNIQUE_INT, null).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void setUserAsAdminUsernameISNullString()
        {
            Assert.IsFalse(_server.setUserAsAdmin(Users.USER_UNIQUE_INT, "null").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void setUserAsAdminUsernameIsEmptyString()
        {
            Assert.IsFalse(_server.setUserAsAdmin(Users.USER_UNIQUE_INT, "").Equals(Replies.SUCCESS));
        }
    }
}
