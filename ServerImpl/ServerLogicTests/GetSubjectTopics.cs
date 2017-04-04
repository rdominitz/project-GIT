using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using System.Collections.Generic;

namespace ServerLogicTests
{
    [TestClass]
    public class GetSubjectTopics
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
            _server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic");
        }

        [TestMethod]
        public void getSubjectTopicsSuccessfully()
        {
            List<string> l = _server.getSubjectTopics("subject");
            Assert.IsTrue(l.Count == 1);
            Assert.IsTrue(l[0].Equals("topic"));
        }

        [TestMethod]
        public void getSubjectTopicsSubjectIsNull()
        {
            ;
            Assert.IsTrue(_server.getSubjectTopics(null) == null);
        }

        [TestMethod]
        public void getSubjectTopicsSubjectIsNullString()
        {
            Assert.IsTrue(_server.getSubjectTopics("null") == null);
        }

        [TestMethod]
        public void getSubjectTopicsSubjectIsEmptyString()
        {
            Assert.IsTrue(_server.getSubjectTopics("") == null);
        }
    }
}
