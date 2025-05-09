using PTQ.Repositories;
using PTQ.Models;
namespace PTQ.Application;

public class QuizService : IQuizService
{
    private IQuizRepository _quizRepository;

    public QuizService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public List<GetAllQuizzesDTO> GetAllQuizzes()
    {
        var quizzes = _quizRepository.GetAllQuizzes();
        var result = new List<GetAllQuizzesDTO>();

        foreach (var quiz in quizzes)
        {
            result.Add(new GetAllQuizzesDTO
            {
                Id = quiz.Id,
                Name = quiz.Name,
            });
        }
        return result;
    }

    public SpecificQuizDTO GetSpecificQuiz(int quizId)
    {
        var quiz = _quizRepository.GetSpecificQuiz(quizId);
        return quiz;
    }

    public bool AddQuiz(AddANewQuizDTO quiz)
    {
        return _quizRepository.AddQuiz(quiz);
    }
}