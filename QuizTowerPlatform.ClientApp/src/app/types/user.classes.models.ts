import { IQuizModel } from "./quiz.classes.models";

export interface IUserResultModel
{
    User: IUserInfoModel,
    Quiz: IQuizModel,
    UsersCorrectAnswers: number,
    UsersWrongAnswers: number,
    PointsEarned: number
}

export interface IUserInfoModel
{
    Username: string,
    Initials: string | undefined,
    MiddleName: string | undefined,
    LastName:string,
    TeamId: number,
    TotalQuizPoints: number | 0,
    TotalAchievementPoints: number | 0,
}