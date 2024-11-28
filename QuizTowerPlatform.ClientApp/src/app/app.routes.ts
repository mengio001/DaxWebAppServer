import { ActivatedRouteSnapshot, RouterStateSnapshot, Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthorizationService } from './services/authorization.service';

import { HomeComponent } from './home/home.component';
import { UserSessionComponent } from './user-session/user-session.component';
import { QuizComponent } from './components/quiz/quiz.component';
import { CreateQuizComponent } from './components/create-quiz/create-quiz.component';
import { AddQuestionsComponent } from './components/add-questions/add-questions.component';
import { StartQuizComponent } from './components/start-quiz/start-quiz.component';
import { SuperUserQuizComponent } from './components/super-user-quiz/super-user-quiz.component';
import { MyResultsComponent } from './components/my-results/my-results.component';

export const routes: Routes = [
  { 
      path: '',
      redirectTo: '/home',
      pathMatch: 'full'
  },
  {
      path: 'home', 
      component: HomeComponent 
  },
  {
    path: 'user-session', 
    component: UserSessionComponent 
  },
  {
    path: 'my-results', 
    component: MyResultsComponent,
    canActivate: [
      (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
        inject(AuthorizationService).canReadQuizDetails(),
    ],    
  },
  {
    path: 'create-quiz',
    component: CreateQuizComponent,
    canActivate: [
      (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
        inject(AuthorizationService).canCreateQuiz(),
    ],
  },
  {
    path: 'add-questions/:id',
    component: AddQuestionsComponent,
    canActivate: [
      (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
        inject(AuthorizationService).canCreateQuiz(),
    ],
  },  
  {
    path: 'quiz/:id',
    component: QuizComponent,
    canActivate: [
      (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
        inject(AuthorizationService).canReadQuizDetails(),
    ],
    data: {
      questions: "true"
    },
  },
  {
    path: 'super-user/q/:id',
    component: SuperUserQuizComponent,
    canActivate: [
      (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
        inject(AuthorizationService).canReadQuizAnswers(),
    ],
  },
  {
    path: 'start-quiz/:id',
    component: StartQuizComponent,
    canActivate: [
      (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
        inject(AuthorizationService).canReadQuizDetails(),
    ],
    data: {
      start: "true"
    },
  }, 
  {
    path: '**',
    redirectTo: 'home'
  }
];