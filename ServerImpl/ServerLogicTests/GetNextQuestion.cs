﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using Constants;
using System.Collections.Generic;

namespace ServerLogicTests
{
    [TestClass]
    public class GetNextQuestion
    {
        private IServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
            _server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic");
            _server.addQuestion(Users.USER_UNIQUE_INT, "subject", false, "", new List<string>() { "topic" });
        }

        [TestMethod]
        public void GetNextQuestionSingleQuestion()
        {
            _server.getAutoGeneratedQuesstion(Users.USER_UNIQUE_INT, "subject", "topic");
            Assert.IsTrue(_server.getNextQuestion(Users.USER_UNIQUE_INT).Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void GetNextQuestionTwoQuestions()
        {
            _server.addQuestion(Users.USER_UNIQUE_INT, "subject", false, "", new List<string>() { "topic" });
            _server.getAutoGeneratedTest(Users.USER_UNIQUE_INT, "subject", "topic", 2, true);
            Tuple<string, Question> t = _server.getNextQuestion(Users.USER_UNIQUE_INT);
            Assert.IsTrue(t.Item1.Equals(Replies.SUCCESS));
            _server.answerAQuestion(Users.USER_UNIQUE_INT, t.Item2.QuestionId, true, 5, new List<string>(), new List<int>());
            t = _server.getNextQuestion(Users.USER_UNIQUE_INT);
            Assert.IsTrue(t.Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void GetNextQuestionUserNotLoggedIn()
        {
            _server.getAutoGeneratedQuesstion(Users.USER_UNIQUE_INT, "subject", "topic");
            _server.logout(Users.USER_UNIQUE_INT);
            Assert.IsFalse(_server.getNextQuestion(Users.USER_UNIQUE_INT).Item1.Equals(Replies.SUCCESS));
        }

        [TestMethod]
        public void GetNextQuestionWrongUserId()
        {
            _server.getAutoGeneratedQuesstion(Users.USER_UNIQUE_INT, "subject", "topic");
            Assert.IsFalse(_server.getNextQuestion(Users.USER_UNIQUE_INT - 1).Item1.Equals(Replies.SUCCESS));
        }
    }
}
