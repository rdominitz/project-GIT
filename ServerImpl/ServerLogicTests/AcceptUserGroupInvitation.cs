using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Constants;
using Entities;
using System.Collections.Generic;

namespace ServerLogicTests
{
    [TestClass]
    public class AcceptUserGroupInvitation
    {
        private IServer _server;
        private static string _group = "group";
        private static string _email = "defaultadmin@gmail.com";
        private string _inviteeEmail = "invitee@gmail.com";
        private string _groupCreatedBy = _group + GroupsMembers.CREATED_BY + _email + ")";
        private string _group1CreatedBy = _group + "1" + GroupsMembers.CREATED_BY + _email + ")";
        private int _inviteeId;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new DB.FakeMedTrainDBContext());
            _server.login(_email, "password");
            _inviteeId = _server.register(_inviteeEmail, "password", Users.medicalTrainingLevels[0], "fn", "ln").Item2;
            _server.login(_inviteeEmail, "password");
            _server.createGroup(Users.USER_UNIQUE_INT, _group, "invitee@gmail.com", "join my group :)");
        }

        [TestMethod]
        public void acceptUsersGroupsInvitationsGroupListIsNull()
        {
            Assert.IsFalse(_server.acceptUsersGroupsInvitations(_inviteeId, null).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void acceptUsersGroupsInvitationsWrongUserId()
        {
            Assert.IsFalse(_server.acceptUsersGroupsInvitations(_inviteeId - 1, new List<string>() { _groupCreatedBy }).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void acceptUsersGroupsInvitationsNotLoggedIn()
        {
            _server.logout(_inviteeId);
            Assert.IsFalse(_server.acceptUsersGroupsInvitations(_inviteeId, new List<string>() { _groupCreatedBy }).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void acceptUsersGroupsInvitationsWrongGroup()
        {
            Assert.IsFalse(_server.acceptUsersGroupsInvitations(_inviteeId, new List<string>() { _group1CreatedBy }).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void acceptUsersGroupsInvitationsWrongGroupOutOfTwo()
        {
            Assert.IsFalse(_server.acceptUsersGroupsInvitations(_inviteeId, new List<string>() { _groupCreatedBy, _group1CreatedBy }).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void acceptUsersGroupsInvitationsSingleInvite()
        {
            Assert.IsTrue(_server.acceptUsersGroupsInvitations(_inviteeId, new List<string>() { _groupCreatedBy }).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void acceptUsersGroupsInvitationsTwoInvites()
        {
            _server.createGroup(Users.USER_UNIQUE_INT, _group + "1", "invitee@gmail.com", "join my group :)");
            Assert.IsTrue(_server.acceptUsersGroupsInvitations(_inviteeId, new List<string>() { _groupCreatedBy, _group1CreatedBy }).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void acceptUsersGroupsInvitationsUnregisteredUser()
        {
            _server.inviteToGroup(Users.USER_UNIQUE_INT, _groupCreatedBy, "newuser@gmail.com", "join my group!");
            _server.register("newuser@gmail.com", "password", Users.medicalTrainingLevels[0], "fn", "ln");
            Assert.IsTrue(_server.getUsersGroupsInvitations(_server.login("newuser@gmail.com", "password").Item2).Item2.Count == 1);
        }
    }
}
