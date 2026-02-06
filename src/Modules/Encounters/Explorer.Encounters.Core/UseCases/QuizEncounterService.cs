using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Internal;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;

namespace Explorer.Encounters.Core.UseCases
{
    public class QuizEncounterService : IQuizEncounterService
    {
        private readonly IQuizEncounterRepository _quizEncounterRepository;
        private readonly IQuizCompletionRepository _quizCompletionRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly ITouristXpProfileRepository _xpProfileRepository;
        private readonly IEncounterCompletionRepository _encounterCompletionRepository;
        private readonly IInternalLeaderboardService _leaderboardService;
        private readonly IMapper _mapper;

        public QuizEncounterService(
            IQuizEncounterRepository quizEncounterRepository,
            IQuizCompletionRepository quizCompletionRepository,
            IChallengeRepository challengeRepository,
            ITouristXpProfileRepository xpProfileRepository,
            IEncounterCompletionRepository encounterCompletionRepository,
            IInternalLeaderboardService leaderboardService,
            IMapper mapper)
        {
            _quizEncounterRepository = quizEncounterRepository;
            _quizCompletionRepository = quizCompletionRepository;
            _challengeRepository = challengeRepository;
            _xpProfileRepository = xpProfileRepository;
            _encounterCompletionRepository = encounterCompletionRepository;
            _leaderboardService = leaderboardService;
            _mapper = mapper;
        }

        public QuizEncounterDto CreateForKeyPoint(CreateQuizEncounterDto dto, long authorId)
        {
            var challenge = _challengeRepository.Get(dto.ChallengeId);
            if (challenge == null)
                throw new NotFoundException("Challenge not found.");

            // Allow quiz creation if CreatorId matches OR if challenge has no creator (admin-created)
            if (challenge.CreatorId.HasValue && challenge.CreatorId.Value != authorId)
                throw new ArgumentException("You can only create quiz encounters for your own challenges.");

            if (challenge.Type != ChallengeType.Quiz)
                throw new ArgumentException("Challenge must be of type Quiz.");

            if (challenge.KeyPointId == null)
                throw new ArgumentException("Quiz encounters can only be created for KeyPoint challenges.");

            var existingQuiz = _quizEncounterRepository.GetByChallengeId(dto.ChallengeId);
            if (existingQuiz != null)
                throw new ArgumentException("Quiz encounter already exists for this challenge.");

            if (dto.Questions == null || dto.Questions.Count < 1 || dto.Questions.Count > 3)
                throw new ArgumentException("Quiz must have between 1 and 3 questions.");

            if (dto.MinimumCorrectAnswers <= 0 || dto.MinimumCorrectAnswers > dto.Questions.Count)
                throw new ArgumentException("MinimumCorrectAnswers must be between 1 and total questions count.");

            var quizEncounter = new QuizEncounter(dto.ChallengeId, dto.MinimumCorrectAnswers, dto.AudioStoryPath);
            
            var createdQuiz = _quizEncounterRepository.Create(quizEncounter);

            foreach (var questionDto in dto.Questions)
            {
                var answerOptions = questionDto.AnswerOptions
                    .Select(opt => new QuizAnswerOption(opt.Text, opt.IsCorrect))
                    .ToList();

                var question = new QuizQuestion(
                    questionDto.Text,
                    answerOptions,
                    createdQuiz.Id
                );
                createdQuiz.AddQuestion(question);
            }

            var updatedQuiz = _quizEncounterRepository.Update(createdQuiz);
            var result = _quizEncounterRepository.GetByIdWithQuestions(updatedQuiz.Id);

            return _mapper.Map<QuizEncounterDto>(result);
        }

        public QuizEncounterDto Update(UpdateQuizEncounterDto dto, long authorId)
        {
            var quizEncounter = _quizEncounterRepository.GetByIdWithQuestions(dto.Id);
            if (quizEncounter == null)
                throw new NotFoundException("Quiz encounter not found.");

            var challenge = _challengeRepository.Get(quizEncounter.ChallengeId);
            if (challenge == null)
                throw new NotFoundException("Challenge not found.");

            if (challenge.CreatorId.HasValue && challenge.CreatorId.Value != authorId)
                throw new ArgumentException("You can only update your own quiz encounters.");

            if (dto.Questions == null || dto.Questions.Count < 1 || dto.Questions.Count > 3)
                throw new ArgumentException("Quiz must have between 1 and 3 questions.");

            if (dto.MinimumCorrectAnswers <= 0 || dto.MinimumCorrectAnswers > dto.Questions.Count)
                throw new ArgumentException("MinimumCorrectAnswers must be between 1 and total questions count.");

            quizEncounter.UpdateAudioStoryPath(dto.AudioStoryPath);

            quizEncounter.Questions.Clear();
            foreach (var questionDto in dto.Questions)
            {
                var answerOptions = questionDto.AnswerOptions
                    .Select(opt => new QuizAnswerOption(opt.Text, opt.IsCorrect))
                    .ToList();

                var question = new QuizQuestion(
                    questionDto.Text,
                    answerOptions,
                    quizEncounter.Id
                );
                quizEncounter.AddQuestion(question);
            }

            quizEncounter.UpdateMinimumCorrectAnswers(dto.MinimumCorrectAnswers);

            var updatedQuiz = _quizEncounterRepository.Update(quizEncounter);
            var result = _quizEncounterRepository.GetByIdWithQuestions(updatedQuiz.Id);

            return _mapper.Map<QuizEncounterDto>(result);
        }

        public void Delete(long id, long authorId)
        {
            var quizEncounter = _quizEncounterRepository.GetById(id);
            if (quizEncounter == null)
                throw new NotFoundException("Quiz encounter not found.");

            var challenge = _challengeRepository.Get(quizEncounter.ChallengeId);
            if (challenge == null)
                throw new NotFoundException("Challenge not found.");

            if (challenge.CreatorId.HasValue && challenge.CreatorId.Value != authorId)
                throw new ArgumentException("You can only delete your own quiz encounters.");

            _quizEncounterRepository.Delete(id);
        }

        public QuizEncounterDto GetById(long id)
        {
            var quizEncounter = _quizEncounterRepository.GetByIdWithQuestions(id);
            if (quizEncounter == null)
                throw new NotFoundException("Quiz encounter not found.");

            return _mapper.Map<QuizEncounterDto>(quizEncounter);
        }

        public QuizEncounterDto GetByChallengeId(long challengeId)
        {
            var quizEncounter = _quizEncounterRepository.GetByChallengeIdWithQuestions(challengeId);
            if (quizEncounter == null)
                throw new NotFoundException("Quiz encounter not found for this challenge.");

            return _mapper.Map<QuizEncounterDto>(quizEncounter);
        }

        public List<QuizEncounterDto> GetByAuthor(long authorId)
        {
            var quizEncounters = _quizEncounterRepository.GetAllByAuthorId(authorId);
            return _mapper.Map<List<QuizEncounterDto>>(quizEncounters);
        }

        public QuizEncounterDto GetForTourist(long challengeId)
        {
            var quizEncounter = _quizEncounterRepository.GetByChallengeIdWithQuestions(challengeId);
            if (quizEncounter == null)
                throw new NotFoundException("Quiz encounter not found for this challenge.");

            var quizDto = _mapper.Map<QuizEncounterDto>(quizEncounter);
            
            // Hide correct answers for tourists
            foreach (var question in quizDto.Questions)
            {
                foreach (var option in question.AnswerOptions)
                {
                    option.IsCorrect = false;
                }
            }

            return quizDto;
        }

        public QuizResultDto SubmitQuiz(QuizSubmissionDto dto, long userId)
        {
            var quizEncounter = _quizEncounterRepository.GetByChallengeIdWithQuestions(dto.ChallengeId);
            if (quizEncounter == null)
                throw new NotFoundException("Quiz encounter not found.");

            var challenge = _challengeRepository.Get(dto.ChallengeId);
            if (challenge == null)
                throw new NotFoundException("Challenge not found.");

            var existingCompletion = _quizCompletionRepository.GetByUserAndChallenge(userId, dto.ChallengeId);
            
            // Allow retry only if previous attempt was unsuccessful
            if (existingCompletion != null && existingCompletion.IsSuccessful)
                throw new ArgumentException("You have already completed this quiz successfully.");

            var questions = quizEncounter.Questions.ToList();
            if (dto.Answers.Count != questions.Count)
                throw new ArgumentException("Answer count does not match question count.");

            int correctAnswersCount = 0;
            var questionResults = new List<QuestionResultDto>();

            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var answer = dto.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                
                if (answer == null)
                    throw new ArgumentException($"Missing answer for question {question.Id}");

                bool isCorrect = question.IsCorrectAnswer(answer.SelectedOptionIndexes);
                if (isCorrect) correctAnswersCount++;

                var optionResults = question.AnswerOptions.Select((opt, idx) => new OptionResultDto
                {
                    Text = opt.Text,
                    IsCorrect = opt.IsCorrect,
                    WasSelected = answer.SelectedOptionIndexes.Contains(idx)
                }).ToList();

                questionResults.Add(new QuestionResultDto
                {
                    QuestionId = question.Id,
                    Text = question.Text,
                    IsCompletelyCorrect = isCorrect,
                    Options = optionResults
                });
            }

            bool isSuccessful = quizEncounter.IsPassingScore(correctAnswersCount);
            int xpAwarded = isSuccessful ? challenge.XP : 0;

            // If there's an existing failed attempt, delete it before creating new one
            if (existingCompletion != null && !existingCompletion.IsSuccessful)
            {
                _quizCompletionRepository.Delete(existingCompletion.Id);
            }

            var quizCompletion = new QuizCompletion(
                userId,
                quizEncounter.Id,
                dto.ChallengeId,
                correctAnswersCount,
                questions.Count,
                isSuccessful,
                xpAwarded
            );

            _quizCompletionRepository.Create(quizCompletion);

            if (isSuccessful)
            {
                // Record encounter completion
                var encounterCompletion = new EncounterCompletion(userId, dto.ChallengeId, xpAwarded);
                _encounterCompletionRepository.Create(encounterCompletion);

                // Update XP profile
                var xpProfile = _xpProfileRepository.GetByUserId(userId);
                if (xpProfile != null)
                {
                    xpProfile.AddXP(xpAwarded);
                    _xpProfileRepository.Update(xpProfile);
                }

                // UPDATE LEADERBOARD STATS
                var coinsEarned = xpAwarded / 2;
                try
                {
                    Console.WriteLine($"[QUIZ ENCOUNTER] Updating leaderboard for user {userId}: XP={xpAwarded}, Challenges=1, Coins={coinsEarned}");
                    _leaderboardService.UpdateUserStatsAsync(
                        userId,
                        xpGained: xpAwarded,
                        challengesCompleted: 1,
                        toursCompleted: 0,
                        coinsEarned).GetAwaiter().GetResult();
                    Console.WriteLine($"[QUIZ ENCOUNTER] Leaderboard updated successfully for user {userId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[QUIZ ENCOUNTER] Error updating leaderboard for user {userId}: {ex.Message}");
                    // Don't fail the completion if leaderboard update fails
                }
            }

            return new QuizResultDto
            {
                CorrectAnswersCount = correctAnswersCount,
                TotalQuestionsCount = questions.Count,
                IsSuccessful = isSuccessful,
                XpAwarded = xpAwarded,
                QuestionResults = questionResults
            };
        }

        public bool HasTouristCompletedQuiz(long userId, long challengeId)
        {
            var completion = _quizCompletionRepository.GetByUserAndChallenge(userId, challengeId);
            return completion != null;
        }
    }
}
