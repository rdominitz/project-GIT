using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using System.Collections.Generic;

namespace ServerLogicTests
{

    // public Tuple<string, List<Tuple<string, int, int, int, int>>> getPastGroupGrades(int userUniqueInt, string groupName)
    // not logged in
    // no statistics to show
    // test statistics

    [TestClass]
    public class GetPastGroupGrades
    {
        private IServer _server;
        private string _email = "defaultadmin@gmail.com";
        private int _inviteeId;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.login(_email, "password");
            _inviteeId = _server.register("invitee@gmail.com", "password", Users.medicalTrainingLevels[0], "fn", "ln").Item2;
            _server.login("invitee@gmail.com", "password");
            _server.createGroup(Users.USER_UNIQUE_INT, "group", "invitee@gmail.com", "join my group :)");
            _server.acceptUsersGroupsInvitations(_inviteeId, new List<string>() { "group" + GroupsMembers.CREATED_BY + _email + ")" });
        }

        [TestMethod]
        public void getPastGroupGradesGroupIsNull()
        {
            Assert.IsFalse(_server.getPastGroupGrades(_inviteeId, null).Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void getPastGroupGradesGroupIsNullString()
        {
            Assert.IsFalse(_server.getPastGroupGrades(_inviteeId, "null").Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void getPastGroupGradesGroupIsEmptyString()
        {
            Assert.IsFalse(_server.getPastGroupGrades(_inviteeId, "").Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void getPastGroupGradesGroupDoesNotExist()
        {
            Assert.IsFalse(_server.getPastGroupGrades(_inviteeId, "group1" + GroupsMembers.CREATED_BY + _email + ")").Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void getPastGroupGradesGroupWrongUserId()
        {
            Assert.IsFalse(_server.getPastGroupGrades(_inviteeId - 1, "group" + GroupsMembers.CREATED_BY + _email + ")").Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void getPastGroupGradesGroupNotLoggedIn()
        {
            _server.logout(_inviteeId);
            Assert.IsFalse(_server.getPastGroupGrades(_inviteeId, "group" + GroupsMembers.CREATED_BY + _email + ")").Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void getPastGroupGradesGroupNoStatisticsToShow()
        {
            Assert.IsFalse(_server.getPastGroupGrades(_inviteeId, "group" + GroupsMembers.CREATED_BY + _email + ")").Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void getPastGroupGradesGroupSuccessfully()
        {
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
            _server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic");
            _server.createQuestion(Users.USER_UNIQUE_INT, "subject", new List<string>() { "topic" }, new List<byte[]>(), "");
            _server.createTest(Users.USER_UNIQUE_INT, "subject", new List<string>() { "topic" });
            _server.createTest(Users.USER_UNIQUE_INT, new List<int>() { 1 }, "test");
            _server.addTestToGroup(Users.USER_UNIQUE_INT, "group" + GroupsMembers.CREATED_BY + _email + ")", 1);
            _server.answerAQuestionGroupTest(_inviteeId, "group" + GroupsMembers.CREATED_BY + _email + ")", 1, 1, false, 7, new List<string>() { "topic" }, new List<int>() { 7 });
            Tuple<string, List<Tuple<string, int, int, int, int>>> t = _server.getPastGroupGrades(_inviteeId, "group" + GroupsMembers.CREATED_BY + _email + ")");
            Assert.IsTrue(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2[0].Item1.Equals("test"));
            Assert.IsTrue(t.Item2[0].Item2 == 1);
            Assert.IsTrue(t.Item2[0].Item3 == 1);
            Assert.IsTrue(t.Item2[0].Item4 == 0);
        }
    }
}
