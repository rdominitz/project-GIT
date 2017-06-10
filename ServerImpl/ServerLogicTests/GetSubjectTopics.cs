using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using System.Collections.Generic;
using DB;

namespace ServerLogicTests
{
    [TestClass]
    public class GetSubjectTopics
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext(0));
            _server.login("defaultadmin@gmail.com", "password");
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
            _server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic");
        }

        [TestMethod]
        public void getSubjectTopicsGetAQuestionSuccessfully()
        {
            List<string> l = _server.getSubjectTopicsGetAQuestion("subject");
            Assert.IsTrue(l.Count == 1);
            Assert.IsTrue(l[0].Equals("topic"));
        }

        [TestMethod]
        public void getSubjectTopicsGetAQuestionSubjectIsNull()
        {
            ;
            Assert.IsTrue(_server.getSubjectTopicsGetAQuestion(null) == null);
        }

        [TestMethod]
        public void getSubjectTopicsGetAQuestionSubjectIsNullString()
        {
            Assert.IsTrue(_server.getSubjectTopicsGetAQuestion("null") == null);
        }

        [TestMethod]
        public void getSubjectTopicsGetAQuestionSubjectIsEmptyString()
        {
            Assert.IsTrue(_server.getSubjectTopicsGetAQuestion("") == null);
        }
    }
}
