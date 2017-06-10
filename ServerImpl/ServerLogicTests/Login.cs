using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;

namespace ServerLogicTests
{
    [TestClass]
    public class Login
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
        }

        [TestMethod]
        public void loginSuccessful()
        {
            Tuple<string, int> t = _server.login("user@gmail.com", "password");
            Assert.IsTrue(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 != -1);
        }

        [TestMethod]
        public void loginUserNotRegistered()
        {
            Tuple<string, int> t = _server.login("otheruser@gmail.com", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameWithMoreThanOneAt()
        {
            Tuple<string, int> t = _server.login("user@gm@ail.com", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameStartsWithAt()
        {
            Tuple<string, int> t = _server.login("@gmail.com", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameEndsWithAt()
        {
            Tuple<string, int> t = _server.login("usergmail.com@", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameEndsWithDot()
        {
            Tuple<string, int> t = _server.login("user@gmail.com.", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameWithoutDotAfterAt()
        {
            Tuple<string, int> t = _server.login("user@gmailcom", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameWithDotJustAfterAt()
        {
            Tuple<string, int> t = _server.login("user@.gmail.com", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginWrongPassword()
        {
            Tuple<string, int> t = _server.login("user@gmail.com", "notpassword");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameIsNull()
        {
            Tuple<string, int> t = _server.login(null, "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameIsNullString()
        {
            Tuple<string, int> t = _server.login("null", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginUsernameIsEmpty()
        {
            Tuple<string, int> t = _server.login("", "password");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginPasswordIsNull()
        {
            Tuple<string, int> t = _server.login("user@gmail.com", null);
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginPasswordIsNullString()
        {
            Tuple<string, int> t = _server.login("user@gmail.com", "null");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void loginPasswordIsEmpty()
        {
            Tuple<string, int> t = _server.login("user@gmail.com", "");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }
    }
}
