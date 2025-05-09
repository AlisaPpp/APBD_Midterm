using Microsoft.Data.SqlClient;
using PTQ.Models;

namespace PTQ.Repositories;

public class QuizRepository : IQuizRepository
{
    private string _connectionString;

    public QuizRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Quiz> GetAllQuizzes()
    {
        List<Quiz> quizzes = new List<Quiz>();
        
        string selectQuizzes = "select * from Quiz";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(selectQuizzes, conn);
            conn.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var quiz = new Quiz
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            PotatoTeacherId = reader.GetInt32(2),
                            PathFile = reader.GetString(3)
                        };
                        quizzes.Add(quiz);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        return quizzes;
    }

    public SpecificQuizDTO GetSpecificQuiz(int Id)
    {
        string specificQuiz = @"select * from Quiz q 
                                join PotatoTeacher pt on q.PotatoTeacherId = pt.Id
                                where Id = @Id";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(specificQuiz, conn);
            command.Parameters.AddWithValue("@Id", Id);
            conn.Open();
            
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return new SpecificQuizDTO
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        PotatoTeacherName = reader.GetString(5),
                        PathFile = reader.GetString(3)
                    };

                }
            }
            finally
            {
                reader.Close();
            }
        }
        return null;
    }

    public bool AddQuiz(AddANewQuizDTO quiz)
    {
        var countRowsAdd = 0;
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                int teacherId = 0;
                string selectTeacherQuery = "select Id from PotatoTeacher where Name = @Name";
                using (SqlCommand command = new SqlCommand(selectTeacherQuery, conn, transaction))
                {
                    command.Parameters.AddWithValue("@Name", quiz.PotatoTeacherName);
                    object? result = command.ExecuteScalar();
                    if (result != null)
                    {
                        teacherId = Convert.ToInt32(result);
                    }
                }

                if (teacherId == 0)
                {
                    string insertTeacherQuery = "INSERT INTO PotatoTeacher (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                    using (SqlCommand command = new SqlCommand(insertTeacherQuery, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@Name", quiz.PotatoTeacherName);
                        teacherId = (int)command.ExecuteScalar();
                    }
                }

                string insertQuizQuery =
                    "INSERT INTO Quiz (Name, PotatoTeacherId, PathFile) VALUES (@Name, @PotatoTeacherId, @PathFile)";
                using (SqlCommand command = new SqlCommand(insertQuizQuery, conn, transaction))
                {
                    command.Parameters.AddWithValue("@Name", quiz.PotatoTeacherName);
                    command.Parameters.AddWithValue("@PotatoTeacherId", teacherId);
                    command.Parameters.AddWithValue("@PathFile", quiz.PathFile);
                    countRowsAdd = command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        return countRowsAdd > 0;
    }
}