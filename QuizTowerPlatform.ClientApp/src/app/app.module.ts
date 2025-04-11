import { NgModule, APP_ID } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogModule } from '@angular/material/dialog';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { routes } from './app.routes';
import { HomeComponent } from './home/home.component';
import { UserSessionComponent } from './user-session/user-session.component';
import { CsrfHeaderInterceptor } from './csrf-header.interceptor';
import { QuizComponent } from './components/quiz/quiz.component';
import { QuizzesListComponent } from './components/quizzes-list/quizzes-list.component';
import { CreateQuizComponent } from './components/create-quiz/create-quiz.component';
import { AddQuestionsComponent } from './components/add-questions/add-questions.component';
import { StartQuizComponent } from './components/start-quiz/start-quiz.component';
import { SuperUserQuizComponent } from './components/super-user-quiz/super-user-quiz.component';
import { MyResultsComponent } from './components/my-results/my-results.component';
import { ShareDialogComponent } from './components/share-dialog/share-dialog.component';

import { SharedModule } from './shared/shared.module';
import { CategoryDisplayNamePipe } from './pipes/category-display-name.pipe';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        UserSessionComponent,
        HomeComponent,
        QuizComponent,
        QuizzesListComponent,
        CreateQuizComponent,
        AddQuestionsComponent,
        CategoryDisplayNamePipe,
        StartQuizComponent,
        SuperUserQuizComponent,
        MyResultsComponent,
        ShareDialogComponent
    ],
    imports: [
        BrowserModule,
        MatMenuModule,
        MatIconModule,
        MatButtonModule,
        BrowserAnimationsModule,
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forRoot(routes),
        CommonModule,
        SharedModule,
        MatDialogModule
    ],
    providers: [
        {
            provide: APP_ID, useValue: 'ng-cli-universal'
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: CsrfHeaderInterceptor,
            multi: true
        },
        provideHttpClient(withInterceptorsFromDi()),
    ],
    exports: [
        CategoryDisplayNamePipe,
        MatMenuModule,
        MatIconModule,
        MatButtonModule,
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }