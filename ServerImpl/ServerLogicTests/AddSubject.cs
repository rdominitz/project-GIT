using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;

namespace ServerLogicTests
{
    [TestClass]
    public class AddSubject
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
        }

        // string addSubject(int userUniqueInt, string subject)

        // success
        // not logged in
        // not an admin
        // wrong user id
        // subject is null
        // subject is "null"
        // subject is ""
        // subject exists

        [TestMethod]
        public void addSubjectSuccessfully()
        {
            Assert.IsTrue(_server.addSubject(Users.USER_UNIQUE_INT, "subject").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addSubjectUserNotLoggedIn()
        {
            _server.logout(Users.USER_UNIQUE_INT);
            Assert.IsFalse(_server.addSubject(Users.USER_UNIQUE_INT, "subject").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addSubjectUserIsNotAnAdmin()
        {
            _server.register("newuser@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t = _server.login("newuser@gmail.com", "password");
            Assert.IsFalse(_server.addSubject(t.Item2, "subject").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addSubjectWrongUserId()
        {
            Assert.IsFalse(_server.addSubject(Users.USER_UNIQUE_INT - 1, "subject").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addSubjectSubjectIsNull()
        {
            Assert.IsFalse(_server.addSubject(Users.USER_UNIQUE_INT, null).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addSubjectSubjectIsNullString()
        {
            Assert.IsFalse(_server.addSubject(Users.USER_UNIQUE_INT, "null").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addSubjectSubjectIsEmptyString()
        {
            Assert.IsFalse(_server.addSubject(Users.USER_UNIQUE_INT, "").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void addSubjectSubjectAlreadyExist()
        {
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
            Assert.IsFalse(_server.addSubject(Users.USER_UNIQUE_INT, "subject").Equals(Replies.SUCCESS));
        }
    }
}
