using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using System.Collections.Generic;

namespace ServerLogicTests
{
    [TestClass]
    public class GetAllSubjects
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new DB.FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
        }

        [TestMethod]
        public void getAllSubjects()
        {
            List<string> l = _server.getAllSubjects();
            Assert.IsTrue(l.Count == 1);
            Assert.IsTrue(l[0].Equals("subject"));
        }
    }
}
