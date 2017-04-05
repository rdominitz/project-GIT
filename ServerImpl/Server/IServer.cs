﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Server
{
    public interface IServer
    {
        // has tests
        Tuple<string, int> register(string eMail, string password, string medicalTraining, string firstName, string lastName);

        // has tests
        Tuple<string, int> login(string eMail, string password);

        // has tests
        string restorePassword(string eMail);

        // has tests
        string AnswerAQuestion(int userUniqueInt, int questionID, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties);

        // has tests
        string getAutoGeneratedQuesstion(int userUniqueInt, string subject, string topic);

        // has tests
        string getAutoGeneratedTest(int userUniqueInt, string subject, string topic, int numOfQuestions, bool answerEveryTime);

        // has tests
        Tuple<string, Question> getNextQuestion(int userUniqueInt);

        // has tests
        Tuple<string, List<Question>> getAnsweres(int userUniqueInt);

        // need to implement addQuestion vith images first
        // correct and incorrect question IDs
        List<string> getQuestionImages(int questionId);

        // has tests
        List<string> getQuestionDiagnoses(int questionId);

        // has tests
        List<string> getAllSubjects();

        // has tests
        List<string> getSubjectTopics(string subject);

        // has tests
        string addSubject(int userUniqueInt, string subject);

        // has tests
        string addTopic(int userUniqueInt, string subject, string topic);

        string addQuestion(int userUniqueInt, string subject, bool isNormal, string text, List<string> qDiagnoses);

        // has tests
        string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin);

        // must be logged in
        bool hasMoreQuestions(int userUniqueInt);

        void logout(int userUniqueInt);

        bool isLoggedIn(int userUniqueInt);

        // must be logged in
        string getUserName(int userUniqueInt);

        string createGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent);

        string inviteToGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent);

        Tuple<string, List<string>> getAllAdminsGroups(int userUniqueInt);

        string removeGroup(int userUniqueInt, string groupName);

        Tuple<string, List<Question>> createTest(int userUniqueInt, string subject, List<string> topics);

        string createTest(int userUniqueInt, List<int> questionsIds, string name);

        bool isAdmin(int userUniqueInt);

        Tuple<string, List<Test>> getAllTests(int userUniqueInt);

        string addTestToGroup(int userUniqueInt, string groupName, int testId);
        
		List<String> getUsersGroups(int userUniqueInt);

        List<String> getUsersGroupsInvitations(int userUniqueInt);

        void acceptUsersGroupsInvitations(int userUniqueInt, List<String> groups);

        string saveSelectedGroup(int userUniqueInt, string groupName);

        Tuple<string, string> getSavedGroup(int userUniqueInt);
    }
}
