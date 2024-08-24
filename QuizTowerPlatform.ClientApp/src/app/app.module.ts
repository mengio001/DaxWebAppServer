import { NgModule, APP_ID } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { routes } from './app.routes';
import { HomeComponent } from './home/home.component';
import { QuizComponent } from './components/quiz/quiz.component';
import { QuizzesListComponent } from './components/quizzes-list/quizzes-list.component';
import { UserSessionComponent } from './user-session/user-session.component';
import { CsrfHeaderInterceptor } from './csrf-header.interceptor';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        QuizComponent,
        QuizzesListComponent,
        UserSessionComponent,        
    ],
    imports: [
        BrowserModule,
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,  
        RouterModule.forRoot(routes),
        CommonModule
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
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }