import { IUserInfoModel } from "./user.classes.models";
import { IQuizModel } from "./quiz.classes.models";

export interface IMyResultsModel {
    UserId: number;
    User: IUserInfoModel;
    QuizId: number;
    Quiz: IQuizModel;
    UsersCorrectAnswers: number;
    UsersWrongAnswers: number;
    PointsEarned: number;
    Id: number;
}