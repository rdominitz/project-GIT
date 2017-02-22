using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;

namespace ServerLogicTests
{
    [TestClass]
    public class getAllSUbjects
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
        }

        [TestMethod]
        public void getAllSubjects()
        {
        }
    }
}
