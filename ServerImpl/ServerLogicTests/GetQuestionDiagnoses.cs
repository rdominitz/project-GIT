using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using System.Collections.Generic;

namespace ServerLogicTests
{
    [TestClass]
    public class GetQuestionDiagnoses
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
        public void getQuestionDiagnosesExistQuestionWithDiagnoses()
        {
            _server.createQuestion(Users.USER_UNIQUE_INT, "subject", new List<string>() { "topic" }, new List<byte[]>() { new byte[1] { 5 } }, "");
            List<string> l = _server.getQuestionDiagnoses(1);
            Assert.IsTrue(l.Count == 1);
            Assert.IsTrue(l[0].Equals("topic"));
        }

        [TestMethod]
        public void getQuestionDiagnosesExistQuestionWithoutDiagnoses()
        {
            _server.createQuestion(Users.USER_UNIQUE_INT, "subject", new List<string>(), new List<byte[]>() { new byte[1] { 5 } }, "");
            List<string> l = _server.getQuestionDiagnoses(1);
            Assert.IsTrue(l.Count == 1);
            Assert.IsTrue(l[0].Equals(Topics.NORMAL));
        }

        [TestMethod]
        public void getQuestionDiagnosesNonExistQuestion()
        {
            List<string> l = _server.getQuestionDiagnoses(1);
            Assert.IsTrue(l.Count == 0);
        }
    }
}
