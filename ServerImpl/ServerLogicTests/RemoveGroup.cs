using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using DB;

namespace ServerLogicTests
{
    [TestClass]
    public class RemoveGroup
    {
        private IServer _server;
        private static string _group = "group";
        private static string _email = "defaultadmin@gmail.com";
        private string _groupCreatedBy = _group + GroupsMembers.CREATED_BY + _email + ")";
        private string _nonExistingGroupCreatedBy = _group + "1" + GroupsMembers.CREATED_BY + _email + ")";

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext(0));
            _server.login(_email, "password");
            _server.createGroup(Users.USER_UNIQUE_INT, _group, "invitee@gmail.com", "join my group :)");
        }

        [TestMethod]
        public void removeGroupSuccessfully()
        {
            Assert.IsTrue(_server.removeGroup(Users.USER_UNIQUE_INT, _groupCreatedBy).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void removeGroupUserNotLoggedIn()
        {
            _server.logout(Users.USER_UNIQUE_INT);
            Assert.IsFalse(_server.removeGroup(Users.USER_UNIQUE_INT, _group).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void removeGroupUserIsNotAnAdmin()
        {
            _server.register("newuser@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t = _server.login("newuser@gmail.com", "password");
            Assert.IsFalse(_server.removeGroup(t.Item2, _group).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void removeGroupWrongUserId()
        {
            Assert.IsFalse(_server.removeGroup(Users.USER_UNIQUE_INT - 1, _group).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void removeGroupGroupNameIsNull()
        {
            Assert.IsFalse(_server.removeGroup(Users.USER_UNIQUE_INT, null).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void removeGroupGroupNameIsNullString()
        {
            Assert.IsFalse(_server.removeGroup(Users.USER_UNIQUE_INT, "null").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void removeGroupGroupNameIsEmptyString()
        {
            Assert.IsFalse(_server.removeGroup(Users.USER_UNIQUE_INT, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void removeGroupGroupDoesNotExist()
        {
            Assert.IsFalse(_server.removeGroup(Users.USER_UNIQUE_INT, _nonExistingGroupCreatedBy).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void removeGroupUserDidNotCreateThatGroup()
        {
            string email = "newuser@gmail.com";
            _server.register(email, "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t = _server.login(email, "password");
            _server.setUserAsAdmin(Users.USER_UNIQUE_INT, email);
            Assert.IsFalse(_server.removeGroup(t.Item2, _groupCreatedBy).Equals(Replies.SUCCESS));
        }
    }
}
