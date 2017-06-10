using Constants;
using DB;
using Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class QuestionsManager
    {
        private IMedTrainDBContext _db;
        private int _questionID;
        private readonly object _syncLockQuestionId;
        private Dictionary<User, List<Question>> _questionsToRemove;

        public QuestionsManager(IMedTrainDBContext db)
        {
            _db = db;
            _questionID = 1;
            _syncLockQuestionId = new object();
            _questionsToRemove = new Dictionary<User, List<Question>>();
        }

        public string createQuestion(string subject, List<string> qDiagnoses, List<byte[]> allImgs, string freeText)
        {
            // verify subject exist
            Subject sub = _db.getSubject(subject);
            if (sub == null)
            {
                return Replies.NOT_A_SUBJECT;
            }
            // verify all diagnoses are topics of the specified subject
            List<Diagnosis> diagnoses = new List<Diagnosis>();
            List<Topic> subjectTopics = _db.getTopics(subject);
            foreach (string diagnosis in qDiagnoses)
            {
                if (subjectTopics.Where(t => t.TopicId.Equals(diagnosis)).ToList().Count == 0)
                {
                    return "Error. " + diagnosis + " is not a topic of " + subject;
                }
            }
            if (qDiagnoses.Count == 0)
            {
                qDiagnoses.Add(Topics.NORMAL);
            }
            lock (_syncLockQuestionId)
            {
                // save images
                List<string> imagesPathes = new List<string>();
                for (int i = 0; i < allImgs.Count; i++)
                {
                    MemoryStream ms = new MemoryStream(allImgs[i], 0, allImgs[i].Length);
                    ms.Write(allImgs[i], 0, allImgs[i].Length);
                    Image image = Image.FromStream(ms, true);
                    string path = @"C:/Users/admin/Desktop/project-GIT/ServerImpl/communication/Images/q" + _questionID + "_img" + (i + 1) + ".jpg";
                    image.Save(path, ImageFormat.Jpeg);
                    imagesPathes.Add("../Images/q" + _questionID + "_img" + (i + 1) + ".jpg");
                }
                addQuestion(sub, subjectTopics.Where(st => qDiagnoses.Contains(st.TopicId)).ToList(), imagesPathes, freeText);
            }
            return Replies.SUCCESS;
        }

        public void addQuestion(Subject s, List<Topic> diagnoses, List<string> images, string qText)
        {
            Question q = new Question
            {
                QuestionId = _questionID,
                SubjectId = s.SubjectId,
                text = qText,
                level = Levels.DEFAULT_LVL,
                points = Questions.QUESTION_INITAL_POINTS,
                timeAdded = DateTime.Now,
                isDeleted = false
            };
            _db.addQuestion(q);
            foreach (Topic t in diagnoses)
            {
                Diagnosis d = new Diagnosis
                {
                    QuestionId = _questionID,
                    TopicId = t.TopicId,
                    SubjectId = s.SubjectId
                };
                _db.addDiagnosis(d);
            }
            foreach (string path in images)
            {
                QuestionImage i = new QuestionImage
                {
                    ImageId = path,
                    QuestionId = _questionID,
                };
                _db.addImage(i);
            }
            _questionID++;
        }

        public string saveSelectedSubjectTopic(User user, string subject, List<string> topicsList)
        {
            // verify subject exist
            Subject sub = _db.getSubject(subject);
            if (sub == null)
            {
                return Replies.NOT_A_SUBJECT;
            }
            // verify all diagnoses are topics of the specified subject
            List<Diagnosis> diagnoses = new List<Diagnosis>();
            List<Topic> subjectTopics = _db.getTopics(subject);
            foreach (string diagnosys in topicsList)
            {
                if (subjectTopics.Where(t => t.TopicId.Equals(diagnosys)).ToList().Count == 0)
                {
                    return "Error. " + diagnosys + " is not a topic of " + subject;
                }
            }
            List<Question> questions = new List<Question>();
            foreach (string topic in topicsList)
            {
                questions.AddRange(_db.getQuestions(subject, topic));
            }
            _questionsToRemove[user] = questions;
            return Replies.SUCCESS;
        }

        public Tuple<string, List<Question>> getAllReleventQuestions(User user)
        {
            if (!_questionsToRemove.Keys.Contains(user))
            {
                return new Tuple<string, List<Question>>("Error. No questions available.", null);
            }
            List<Question> l = _questionsToRemove[user];
            _questionsToRemove.Remove(user);
            return new Tuple<string, List<Question>>(Replies.SUCCESS, l);
        }

        public string removeQuestions(List<Tuple<int, string>> questionsIdsAndResonsList)
        {
            // verify all questions exists
            lock (_syncLockQuestionId)
            {
                foreach (Tuple<int, string> t in questionsIdsAndResonsList)
                {
                    if (t.Item1 >= _questionID)
                    {
                        return "Error. Some questions cannot be removed.";
                    }
                }
            }
            foreach (Tuple<int, string> t in questionsIdsAndResonsList)
            {
                Question q = _db.getQuestion(t.Item1);
                string removalReason = t.Item2;
                if (removalReason.Equals(""))
                {
                    removalReason = "This quesiton has been removed. Any answer will not be accounted for.";
                }
                q.isDeleted = true;
                if (q.text == "")
                {
                    q.text = removalReason;
                }
                else
                {
                    q.text += Environment.NewLine + Environment.NewLine + removalReason;
                }
                _db.updateQuestion(q);
            }
            return Replies.SUCCESS;
        }
    }
}
