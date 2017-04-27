﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Entities;
using System.Collections.Generic;
using Constants;

namespace ServerLogicTests
{
    [TestClass]
    public class AnswerAQuestion
    {
        private IServer _server;
        private Question _q;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new ServerImpl(new FakeMedTrainDBContext());
            _server.login("defaultadmin@gmail.com", "password");
            _server.addSubject(Users.USER_UNIQUE_INT, "subject");
            _server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic");
            _server.createQuestion(Users.USER_UNIQUE_INT, "subject", new List<string>() { "topic" }, new List<byte[]>(), "");
        }

        private void setup()
        {
            _server.getAutoGeneratedQuesstion(Users.USER_UNIQUE_INT, "subject", "topic");
            _q = _server.getNextQuestion(Users.USER_UNIQUE_INT).Item2;
        }

        [TestMethod]
        public void answerAQuestionUserNotLoggedIn()
        {
            setup();
            _server.logout(Users.USER_UNIQUE_INT);
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, true, 5, new List<string>(), new List<int>()).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionWrongUserId()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT - 1, _q.QuestionId, true, 5, new List<string>(), new List<int>()).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionWrongQuestionId()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, 0, true, 5, new List<string>(), new List<int>()).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionNormalityCertaintyIsZero()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, true, 0, new List<string>(), new List<int>()).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionNormalityCertaintyIsNegative()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, true, -1, new List<string>(), new List<int>()).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionNormalityCertaintyIsOverTen()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, true, 11, new List<string>(), new List<int>()).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionDiagnosesIsNull()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, true, 5, null, new List<int>()).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionDiagnosesContainsNull()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, true, 5, new List<string>() { null }, new List<int>() { 5 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionDiagnosesContainsNullString()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "null" }, new List<int>() { 5 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionDiagnosesContainsEmptyString()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "" }, new List<int>() { 5 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionDiagnosesContainsNonExistingTopic()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "no_topic" }, new List<int>() { 5 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestionCertaintiesIsNull()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, true, 5, new List<string>(), null).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioCertaintiesContainsZero()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "topic" }, new List<int>() { 0 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioCertaintiesContainsNegativeNumber()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "topic" }, new List<int>() { -1 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioCertaintiesContainsNumberOverTen()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "topic" }, new List<int>() { 11 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioMoreDiagnosesThanCertainties()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "topic" }, new List<int>()).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioMoreCertaintiesThanDiagnoses()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>(), new List<int>() { 5 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioNoQuestionRequested()
        {
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, 1, false, 5, new List<string>() { "topic" }, new List<int>() { 5 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioNormalWithDiagnoses()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, true, 5, new List<string>() { "topic" }, new List<int>() { 5 }).Item1;
            Assert.IsFalse(s.Equals(Replies.NEXT));
            Assert.IsFalse(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioSingleQuestion()
        {
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "topic" }, new List<int>() { 5 }).Item1;
            Assert.IsTrue(s.Equals(Replies.SHOW_ANSWER));
        }

        [TestMethod]
        public void answerAQuestioFromTestAnswerAfterEveryQuestion()
        {
            _server.createQuestion(Users.USER_UNIQUE_INT, "subject", new List<string>() { "topic" }, new List<byte[]>(), "");
            _server.getAutoGeneratedTest(Users.USER_UNIQUE_INT, "subject", "topic", 2, true);
            _q = _server.getNextQuestion(Users.USER_UNIQUE_INT).Item2;
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "topic" }, new List<int>() { 5 }).Item1;
            Assert.IsTrue(s.Equals(Replies.SHOW_ANSWER));
        }
        
        [TestMethod]
        public void answerAQuestioFromTestAnswersAtTheEnd()
        {
            _server.createQuestion(Users.USER_UNIQUE_INT, "subject", new List<string>() { "topic" }, new List<byte[]>(), "");
            _server.getAutoGeneratedTest(Users.USER_UNIQUE_INT, "subject", "topic", 2, false);
            _q = _server.getNextQuestion(Users.USER_UNIQUE_INT).Item2;
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "topic" }, new List<int>() { 5 }).Item1;
            Assert.IsTrue(s.Equals(Replies.NEXT));
        }

        [TestMethod]
        public void answerAQuestioWrongAnswer()
        {
            _server.addTopic(Users.USER_UNIQUE_INT, "subject", "new topic");
            setup();
            string s = _server.answerAQuestion(Users.USER_UNIQUE_INT, _q.QuestionId, false, 5, new List<string>() { "new topic" }, new List<int>() { 5 }).Item1;
            Assert.IsTrue(s.Equals(Replies.SHOW_ANSWER));
        }
    }
}
