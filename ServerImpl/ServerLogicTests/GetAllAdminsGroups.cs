using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Constants;
using System.Collections.Generic;

namespace ServerLogicTests
{
    [TestClass]
    public class GetAllAdminsGroups
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new Entities.FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
            // _server.createGroup(Users.USER_UNIQUE_INT, "group", "", "");
        }

        // Tuple<string, List<string>> getAllAdminsGroups(int userUniqueInt)

        [TestMethod]
        public void getAllADminsGroupsNoGroups()
        {
            Tuple<string, List<string>> t = _server.getAllAdminsGroups(Users.USER_UNIQUE_INT);
            Assert.IsTrue(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2.Count == 0);
        }

        [TestMethod]
        public void getAllADminsGroupsOneGroup()
        {
            string groupName = "group";
            _server.createGroup(Users.USER_UNIQUE_INT, groupName, "", "");
            Tuple<string, List<string>> t = _server.getAllAdminsGroups(Users.USER_UNIQUE_INT);
            Assert.IsTrue(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2.Count == 1 && t.Item2.Contains(groupName));
        }

        [TestMethod]
        public void getAllADminsGroupsTwoGroups()
        {
            string groupName1 = "group1";
            _server.createGroup(Users.USER_UNIQUE_INT, groupName1, "", "");
            string groupName2 = "group2";
            _server.createGroup(Users.USER_UNIQUE_INT, groupName2, "", "");
            Tuple<string, List<string>> t = _server.getAllAdminsGroups(Users.USER_UNIQUE_INT);
            Assert.IsTrue(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2.Count == 2 && t.Item2.Contains(groupName1) && t.Item2.Contains(groupName2));
        }

        [TestMethod]
        public void getAllADminsGroupsUserNotLoggedIn()
        {
            _server.logout(Users.USER_UNIQUE_INT);
            Tuple<string, List<string>> t = _server.getAllAdminsGroups(Users.USER_UNIQUE_INT);
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == null);
        }

        [TestMethod]
        public void getAllADminsGroupsUserIsNotAnAdmin()
        {
            _server.register("newuser@gmail.com", "password", Users.medicalTrainingLevels[0], "first name", "last name");
            Tuple<string, int> t1 = _server.login("newuser@gmail.com", "password");
            Tuple<string, List<string>> t2 = _server.getAllAdminsGroups(t1.Item2);
            Assert.IsFalse(t2.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t2.Item2 == null);
        }

        [TestMethod]
        public void getAllADminsGroupsWrongUserId()
        {
            Tuple<string, List<string>> t = _server.getAllAdminsGroups(Users.USER_UNIQUE_INT - 1);
            Assert.IsFalse(t.Item1.Equals(Replies.SUCCESS));
            Assert.IsTrue(t.Item2 == null);
        }
    }
}
