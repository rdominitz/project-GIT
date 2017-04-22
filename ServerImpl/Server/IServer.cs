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
        // has tests - 100% coverage
        Tuple<string, int> register(string eMail, string password, string medicalTraining, string firstName, string lastName);

        // has tests - 100% coverage
        Tuple<string, int> login(string eMail, string password);

        // has tests - 100% coverage
        string restorePassword(string eMail);

        // has tests - 100% coverage
        string answerAQuestion(int userUniqueInt, int questionID, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties);

        // has tests - 100% coverage
        string getAutoGeneratedQuesstion(int userUniqueInt, string subject, string topic);

        // has tests - 100% coverage
        string getAutoGeneratedTest(int userUniqueInt, string subject, string topic, int numOfQuestions, bool answerEveryTime);

        // has tests - 100% coverage
        Tuple<string, Question> getNextQuestion(int userUniqueInt);

        // has tests - 100% coverage
        Tuple<string, List<Question>> getAnsweres(int userUniqueInt);

        // need to implement addQuestion vith images first
        // correct and incorrect question IDs
        List<string> getQuestionImages(int questionId);

        // has tests - 100% coverage
        List<string> getQuestionDiagnoses(int questionId);

        // has tests - 100% coverage
        List<string> getAllSubjects();

        // has tests - 100% coverage
        List<string> getSubjectTopics(string subject);

        // has tests - 100% coverage
        string addSubject(int userUniqueInt, string subject);

        // has tests - 100% coverage
        string addTopic(int userUniqueInt, string subject, string topic);

        // has tests - 100% coverage
        string setUserAsAdmin(int userUniqueInt, string usernameToTurnToAdmin);

        // has tests - 100% coverage
        bool hasMoreQuestions(int userUniqueInt);

        // 100% coverage
        void logout(int userUniqueInt);

        // has tests - 100% coverage
        bool isLoggedIn(int userUniqueInt);

        // has tests - 100% coverage
        string getUserName(int userUniqueInt);

        // has tests - 100% coverage
        string createGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent);

        // has tests - 100% coverage
        string inviteToGroup(int userUniqueInt, string groupName, string inviteEmails, string emailContent);

        // has tests - 100% coverage
        Tuple<string, List<string>> getAllAdminsGroups(int userUniqueInt);

        string removeGroup(int userUniqueInt, string groupName);

        string createTest(int userUniqueInt, string subject, List<string> topics);

        List<Question> getTestQuestions(int userUniqueInt);

        string createTest(int userUniqueInt, List<int> questionsIds, string name);

        bool isAdmin(int userUniqueInt);

        Tuple<string, List<Test>> getAllTests(int userUniqueInt);

        string addTestToGroup(int userUniqueInt, string groupName, int testId);
        
		Tuple<string, List<String>> getUsersGroups(int userUniqueInt);

        Tuple<string, List<String>> getUsersGroupsInvitations(int userUniqueInt);

        string acceptUsersGroupsInvitations(int userUniqueInt, List<String> groups);

        string saveSelectedGroup(int userUniqueInt, string groupName);

        Tuple<string, string> getSavedGroup(int userUniqueInt);

        // allows empty list of images for tests
        string createQuestion(int userUniqueInt, string subject, List<string> qDiagnoses, List<byte[]> allImgs, string freeText);

        string removeQuestions(int userUniqueInt, List<int> questionsIdsList);
    }
}
