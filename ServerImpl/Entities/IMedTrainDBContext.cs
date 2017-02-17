using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IMedTrainDBContext
    {
        int SaveChanges();
        void addUser(User u);
        User getUser(string eMail);
        User getUser(int uniqueInt);

        void addAdmin(string eMail);
        Admin getAdmin(string eMail);

        void addQuestion(Question q);
        Question getQuestion(int id);
        void updateQuestion(Question q);
        List<Question> getQuestions(string subject, string topic);
        List<Question> getNormalQuestions(string subject);

        void addUserLevel(UserLevel userLevel);
        UserLevel getUserLevel(string eMail, string subject, string topic);
        void updateUserLevel(UserLevel ul);

        void addAnswer(Answer a);

        void addTopic(string subject, string topic);
        void addTopic(Topic t);
        Topic getTopic(string subject, string topic);
        List<Topic> getTopics(string subject);

        void addSubject(string subject);
        void addSubject(Subject s);
        Subject getSubject(string subject);
        List<Subject> getSubjects();
        
    }
}
