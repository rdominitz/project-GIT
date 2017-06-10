using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Constants;
using Entities;

namespace ServerLogicTests
{
    [TestClass]
    public class InviteToGroup
    {
        private IServer _server;
        private static string _group = "group";
        private static string _email = "defaultadmin@gmail.com";
        private string _groupCreatedBy = _group + GroupsMembers.CREATED_BY + _email + ")";
        private string _nonExistingGroupCreatedBy = _group + "1" + GroupsMembers.CREATED_BY + _email + ")";
        private string _inviteeEmail = "invitee@gmail.com";

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new DB.FakeMedTrainDBContext());
            _server.login(_email, "password");
            _server.createGroup(Users.USER_UNIQUE_INT, _group, "", "");
        }

        [TestMethod]
        public void inviteToGRoupSuccessfullyInviteSingleRegisteredUser()
        {
            _server.register(_inviteeEmail, "password", Users.medicalTrainingLevels[0], "fn", "ln");
            Assert.IsTrue(_server.inviteToGroup(Users.USER_UNIQUE_INT, _groupCreatedBy, _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupSuccessfullyInviteSingleUnregisteredUser()
        {
            Assert.IsTrue(_server.inviteToGroup(Users.USER_UNIQUE_INT, _groupCreatedBy, _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupSuccessfullyInviteTwoRegisteredUsers()
        {
            string email1 = "invitee1@gmail.com";
            _server.register(email1, "password", Users.medicalTrainingLevels[0], "fn", "ln");
            string email2 = "invitee2@gmail.com";
            _server.register(email2, "password", Users.medicalTrainingLevels[0], "fn", "ln");
            string emails = email1 + ", " + email2;
            Assert.IsTrue(_server.inviteToGroup(Users.USER_UNIQUE_INT, _groupCreatedBy, emails, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupSuccessfullyInviteTwoUnegisteredUsers()
        {
            string email1 = "invitee1@gmail.com";
            string email2 = "invitee2@gmail.com";
            string emails = email1 + ", " + email2;
            Assert.IsTrue(_server.inviteToGroup(Users.USER_UNIQUE_INT, _groupCreatedBy, emails, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupUserNotLoggedIn()
        {
            _server.logout(Users.USER_UNIQUE_INT);
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT, _group, _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupUserIsNotAnAdmin()
        {
            _server.register("newuser@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t = _server.login("newuser@gmail.com", "password");
            Assert.IsFalse(_server.inviteToGroup(t.Item2, _group, _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupWrongUserId()
        {
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT - 1, _group, _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupNonExistingGroup()
        {
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT, _nonExistingGroupCreatedBy, _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupGroupNameIsNull()
        {
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT, null, _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupGroupNameIsNullString()
        {
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT, "null", _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupGroupNameIsEmptyString()
        {
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT, "", _inviteeEmail, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupInviteEmailsIsNull()
        {
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT, _group, null, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupInviteEmailsIsEmptyString()
        {
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT, _group, "", "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void inviteToGRoupInviteEmailsContainsInvalidEmail()
        {
            string emails = "goodemail1@gmail.com, goodemail2@gmail.com, bad@email@gmail.com, goodemail4@gmail.com";
            Assert.IsFalse(_server.inviteToGroup(Users.USER_UNIQUE_INT, _group, emails, "").Equals(Replies.SUCCESS));
        }
    }
}
