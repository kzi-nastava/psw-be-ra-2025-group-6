-- Seed quizzes for integration tests
INSERT INTO tours."Quizzes"("Id", "AuthorId", "Title", "Description", "CreatedAt", "UpdatedAt")
VALUES
    (-1, 1, 'Belgrade Basics', 'Starter quiz', '2024-01-01T00:00:00Z', NULL),
    (-2, 2, 'Disposable Quiz', 'Quiz used for delete flow', '2024-01-02T00:00:00Z', NULL);

INSERT INTO tours."QuizQuestions"("Id", "QuizId", "Text", "AllowsMultipleAnswers")
VALUES
    (-11, -1, 'Which river flows through Belgrade?', FALSE),
    (-12, -1, 'Select Serbian rivers', TRUE),
    (-21, -2, 'Sample question to remove', FALSE);

INSERT INTO tours."QuizAnswerOptions"("Id", "QuestionId", "Text", "IsCorrect", "Feedback")
VALUES
    (-111, -11, 'Sava', FALSE, 'Not the target river.'),
    (-112, -11, 'Danube', TRUE, 'Correct river.'),
    (-121, -12, 'Danube', TRUE, 'Danube flows through Serbia.'),
    (-122, -12, 'Tisza', FALSE, 'Not correct for this question.'),
    (-123, -12, 'Sava', TRUE, 'Sava also flows through Serbia.'),
    (-211, -21, 'Wrong', FALSE, 'Incorrect option.'),
    (-212, -21, 'Right', TRUE, 'Correct option.');
