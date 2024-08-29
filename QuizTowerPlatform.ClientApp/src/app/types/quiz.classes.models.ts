import { IUserResultModel } from "./user.classes.models";

export interface IQuizModel {
  Id: number;
  Name: string;
  Category: number;
  QuizLogoUrl: string;
  QuizQuestions: IQuestionModel[];
}

export interface IQuestionModel
{
    Id: number;
    QuestionName: string;
    FirstOption: string;
    SecondOption: string;
    ThirdOption: string;
    FourthOption: string;
    CorrectAnswer: string;
    QuizId: number;
    CorrectAnswerPoints : number;
}

export interface IAnswerModel
{
    QuestionId: number;
    Answer: string;
}

export interface IStartQuizModel {
  QuizId: number;
  Answers: IAnswerModel[];
}

