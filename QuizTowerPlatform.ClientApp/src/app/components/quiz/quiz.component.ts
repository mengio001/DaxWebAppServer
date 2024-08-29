import { Component, Input } from '@angular/core';
import { IQuizModel, IQuestionModel } from "../../types/quiz.classes.models";

@Component({
  selector: 'app-quiz',
  templateUrl: './quiz.component.html',
})

export class QuizComponent {
  @Input() quiz: IQuizModel = <IQuizModel>{};
}