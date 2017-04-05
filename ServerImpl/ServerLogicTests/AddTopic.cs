using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;

namespace ServerLogicTests
{
    [TestClass]
    public class AddTopic
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
        }

        [TestMethod]
        public void addTopicSuccessfully()
        {
            Assert.IsTrue(_server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicUserNotLoggedIn()
        {
            _server.logout(Users.USER_UNIQUE_INT);
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicWrongUserId()
        {
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT - 1, "subject", "topic").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicUserIsNotAnAdmin()
        {
            _server.register("newuser@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t = _server.login("newuser@gmail.com", "password");
            Assert.IsFalse(_server.addTopic(t.Item2, "subject", "topic").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicSubjectDoesNotExist()
        {
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, "subject_1", "topic").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicSubjectIsNull()
        {
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, null, "topic").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicSubjectIsNullString()
        {
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, "null", "topic").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicSubjectIsEmptyString()
        {
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, "", "topic").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicTopicIsNull()
        {
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, "subject", null).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicTopicIsNullString()
        {
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, "subject", "null").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicTopicIsEmptyString()
        {
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, "subject", "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addTopicTopicAlreadyExist()
        {
            _server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic");
            Assert.IsFalse(_server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic").Equals(Replies.SUCCESS));
        }
    }
}
