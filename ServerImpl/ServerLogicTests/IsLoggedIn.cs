using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;

namespace ServerLogicTests
{
    [TestClass]
    public class IsLoggedIn
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
        }

        [TestMethod]
        public void isLoggedInTrue()
        {
            _server.login("defaultadmin@gmail.com", "password");
            Assert.IsTrue(_server.isLoggedIn(Users.USER_UNIQUE_INT) != null);
        }

        [TestMethod]
        public void isLoggedInFalse()
        {
            Assert.IsFalse(_server.isLoggedIn(Users.USER_UNIQUE_INT) != null);
        }

        [TestMethod]
        public void isLoggedInWrongUserId()
        {
            Assert.IsFalse(_server.isLoggedIn(Users.USER_UNIQUE_INT - 1) != null);
        }
    }
}
