using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Constants;
using Entities;

namespace ServerLogicTests
{
    [TestClass]
    public class CreateGroup
    {
        private IServer _server;
        private string _emails = "invitee@gmail.com";

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
        }

        [TestMethod]
        public void createGroupSuccessfullyNoInvitees()
        {
            Assert.IsTrue(_server.createGroup(Users.USER_UNIQUE_INT, "group1", "", "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupSuccessfullyOneInvitee()
        {
            Assert.IsTrue(_server.createGroup(Users.USER_UNIQUE_INT, "group1", _emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupSuccessfullyTwoInvitees()
        {
            string emails = "invitee1@gmail.com, invitee2@gmail.com";
            Assert.IsTrue(_server.createGroup(Users.USER_UNIQUE_INT, "group1", emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupUserNotLoggedIn()
        {
            _server.logout(Users.USER_UNIQUE_INT);
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, "group1", _emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupUserIsNotAnAdmin()
        {
            _server.register("newuser@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t = _server.login("newuser@gmail.com", "password");
            Assert.IsFalse(_server.createGroup(t.Item2, "group1", _emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupWrongUserId()
        {
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT - 1, "group1", _emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupGroupNameIsNull()
        {
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, null, _emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupGroupNameIsNullString()
        {
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, "null", _emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupGroupNameIsEmptyString()
        {
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, "", _emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupEmailsAreNull()
        {
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, "group1", null, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupInvalidEmail()
        {
            string emails = "invitee1@gmailcom";
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, "group1", emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupValidAndInvalidEmails()
        {
            string emails = "invitee1@gmail.com, invitee2@.gmail.com";
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, "group1", emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupInvalidEmails()
        {
            string emails = "invitee1@gmail.com., invitee@2@gmail.com";
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, "group1", emails, "join my group :)").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void createGroupGroupAlreadyExist()
        {
            _server.createGroup(Users.USER_UNIQUE_INT, "group1", _emails, "join my group :)");
            Assert.IsFalse(_server.createGroup(Users.USER_UNIQUE_INT, "group1", _emails, "join my group :)").Equals(Replies.SUCCESS));
        }
    }
}
