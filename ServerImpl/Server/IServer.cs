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
        /// <summary>
        /// registers a user to the system
        /// </summary>
        /// <param name="eMail"></param>
        /// <param name="password"></param>
        /// <param name="medicalTraining"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>
        /// if the input is illegal an error message
        /// if a user with the same eMail address is registered an error message
        /// else a success message and a unique integer
        /// </returns>
        Tuple<string, int> register(string eMail, string password, string medicalTraining, string firstName, string lastName);

        /// <summary>
        /// logs a user to the system
        /// </summary>
        /// <param name="eMail"></param>
        /// <param name="password"></param>
        /// <returns>
        /// if the input is illegal an error message
        /// if the provided eMail and password don't match or the user in not registered an error message
        /// if the medical training level is not one of the pre-defined levels an error message
        /// else a success message and the user unique id
        /// </returns>
        Tuple<string, int> login(string eMail, string password);

        /// <summary>
        /// send a user their password
        /// </summary>
        /// <param name="eMail"></param>
        /// <returns>
        /// if the input is illegal an error message
        /// if there is no user with that eMail address provided is registered an error message
        /// else success message
        /// </returns>
        string restorePassword(string eMail);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueInt"></param>
        /// <param name="questionID"></param>
        /// <param name="isNormal"></param>
        /// <param name="normalityCertainty"></param>
        /// <param name="diagnoses"></param>
        /// <param name="diagnosisCertainties"></param>
        /// <returns>
        /// if input is invalid an error message
        /// if should show the answer a "Show answer" message
        /// else a "Next" message
        /// </returns>
        string AnswerAQuestion(int userUniqueInt, int questionID, bool isNormal, int normalityCertainty, List<string> diagnoses, List<int> diagnosisCertainties);

        /// <summary>
        /// calls getAutoGeneratedTest with numOfQuestions = 1 and answerEveryTime irrelevant
        /// </summary>
        /// <param name="userUniqueInt"></param>
        /// <param name="subject"></param>
        /// <param name="topic"></param>
        /// <returns>
        /// if getAutoGeneratedTest fails returns a null question and the error message
        /// else returns a single question and a success message
        /// </returns>
        string getAutoGeneratedQuesstion(int userUniqueInt, string subject, string topic);

        /// <summary>
        /// generate a series of questions based on requested subject and topic
        /// for each question there is a pre-defined probability, NORMAL_PROBABILITY, that the diagnosis should be normal
        /// the mentioned probability is only relevant for a large enough DB
        /// </summary>
        /// <param name="userUniqueInt"></param>
        /// <param name="subject"></param>
        /// <param name="topic"></param>
        /// <param name="numOfQuestions"></param>
        /// <param name="answerEveryTime"> specifies if needs to show the user an answer after each question or the entire test </param>
        /// <returns>
        /// relevant error message if there are not enough questions (both normal and abnormal)
        /// success message and a list of numOfQuestions questions
        /// </returns>
        string getAutoGeneratedTest(int userUniqueInt, string subject, string topic, int numOfQuestions, bool answerEveryTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueInt"></param>
        /// <returns>
        /// if input is invalid an error message
        /// if has next question returns a success message and the question
        /// else returns a success message and null so should always show the answer
        /// </returns>
        Tuple<string, Question> getNextQuestion(int userUniqueInt);

        Tuple<string, List<Question>> getAnsweres(int userUniqueInt);

        List<string> getAllSubjects();

        List<string> getSubjectTopics(string subject);

        //string addQuestion(int userUniqueInt, string subject, List<string> diagnoses, HttpRequest.Form.Files files);
    }
}
