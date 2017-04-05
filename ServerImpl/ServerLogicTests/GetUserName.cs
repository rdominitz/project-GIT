using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;

namespace ServerLogicTests
{
    [TestClass]
    public class GetUserName
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
        }

        [TestMethod]
        public void getUserNameSuccessfully()
        {
            Assert.IsTrue(_server.getUserName(Users.USER_UNIQUE_INT).Equals("default admin"));
        }

        [TestMethod]
        public void getUserNameUserNotLoggedIn()
        {
            _server.logout(Users.USER_UNIQUE_INT);
            Assert.IsFalse(_server.getUserName(Users.USER_UNIQUE_INT).Equals("default admin"));
        }

        [TestMethod]
        public void getUserNameWrongUserId()
        {
            Assert.IsFalse(_server.getUserName(Users.USER_UNIQUE_INT - 1).Equals("default admin"));
        }
    }
}
