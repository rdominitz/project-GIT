using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;

namespace ServerLogicTests
{
    [TestClass]
    public class RestorePasswords
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
        }

        [TestMethod]
        public void restorePasswordSuccessful()
        {
            Assert.IsTrue(_server.restorePassword("user@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUserNotRegistered()
        {
            Assert.IsFalse(_server.restorePassword("otheruser@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUserNotInCache()
        {
            for (int i = 0; i < 1500; i++)
            {
                _server.register("user" + i + "@gmail.com", "password1", Users.medicalTrainingLevels[0], "first name", "last name");
            }
            Assert.IsTrue(_server.restorePassword("user@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameStartsWithAt()
        {
            Assert.IsFalse(_server.restorePassword("@gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameEndsWithAt()
        {
            Assert.IsFalse(_server.restorePassword("usergmail.com@").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameEndsWithDot()
        {
            Assert.IsFalse(_server.restorePassword("user@gmail.com.").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameWithoutDotAfterAt()
        {
            Assert.IsFalse(_server.restorePassword("user@gmailcom").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameWithDotJustAfterAt()
        {
            Assert.IsFalse(_server.restorePassword("user@.gmail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameWithMoreThanOneAt()
        {
            Assert.IsFalse(_server.restorePassword("user@gm@ail.com").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameIsNull()
        {
            Assert.IsFalse(_server.restorePassword(null).Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameIsNullString()
        {
            Assert.IsFalse(_server.restorePassword("null").Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void restorePasswordUsernameIsEmpty()
        {
            Assert.IsFalse(_server.restorePassword("").Equals(Replies.SUCCESS));
        }
    }
}
