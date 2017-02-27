using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using System.Text.RegularExpressions;

namespace ServerLogicTests
{
    [TestClass]
    public class Registration
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
        }

        [TestMethod]
        public void registrationSuccessful()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsTrue(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 != -1);
        }

        [TestMethod]
        public void registrationUsernameInUse()
        {
            _server.register("user@gmail.com", "password1", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t = _server.register("user@gmail.com", "password2", Users.medicalTrainingLevels[1], "other first name", "other last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUserNotInCache()
        {
            _server.register("user@gmail.com", "password1", Users.medicalTrainingLevels[0], "first name", "last name");
            for (int i = 0; i < 1500; i++)
            {
                _server.register("user" + i + "@gmail.com", "password1", Users.medicalTrainingLevels[0], "first name", "last name");
            }
            Tuple<string, int> t = _server.register("user@gmail.com", "password2", Users.medicalTrainingLevels[1], "other first name", "other last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationTooShortPassword()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "pass", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationTooLongPassword()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "veryLongPassword", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationPasswordWithSpecialChar()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "_password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameWithMoreThanOneAt()
        {
            Tuple<string, int> t = _server.register("user@gm@ail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameStartsWithAt()
        {
            Tuple<string, int> t = _server.register("@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameEndsWithAt()
        {
            Tuple<string, int> t = _server.register("usergmail.com@", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameEndsWithDot()
        {
            Tuple<string, int> t = _server.register("user@gmail.com.", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameWithoutDotAfterAt()
        {
            Tuple<string, int> t = _server.register("user@gmailcom", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameWithDotJustAfterAt()
        {
            Tuple<string, int> t = _server.register("user@.gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameIsNull()
        {
            Tuple<string, int> t = _server.register(null, "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameIsNullString()
        {
            Tuple<string, int> t = _server.register("null", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationUsernameIsEmpty()
        {
            Tuple<string, int> t = _server.register("", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationPasswordIsNull()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", null, Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationPasswordIsNullString()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "null", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationPasswordIsEmpty()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "", Users.medicalTrainingLevels[0], "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationMedicalTrainingIsNull()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", null, "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationMedicalTrainingIsNullString()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", "null", "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationMedicalTrainingIsEmpty()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", "", "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationMedicalTrainingIsInvalid()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", "invalid medical training", "first name", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationFirstNameIsNull()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], null, "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationFirstNameIsNullString()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "null", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationFirstNameIsEmpty()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "", "last name");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationLastNameIsNull()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", null);
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationLastNameIsNullString()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "null");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }

        [TestMethod]
        public void registrationLastNameIsEmpty()
        {
            Tuple<string, int> t = _server.register("user@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "");
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == -1);
        }
    }
}
