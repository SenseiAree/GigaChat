import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/home/login/login.component';
import { SignupComponent } from './components/home/signup/signup.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ChatComponent } from './components/chat/chat.component';
import { ProfileComponent } from './components/chat/profile/profile.component';
import { FeedbackComponent } from './components/chat/feedback/feedback.component';
import { ContactComponent } from './components/chat/contact/contact.component';
import { MessageComponent } from './components/chat/message/message.component';
import { HttpClientModule } from '@angular/common/http'
import { UserService } from './services/user.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgPipesModule, TimeAgoPipe } from 'ngx-pipes';
import { DatePipe } from '@angular/common';
import { AdminComponent } from './components/admin/admin.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    SignupComponent,
    ChatComponent,
    ProfileComponent,
    FeedbackComponent,
    ContactComponent,
    MessageComponent,
    AdminComponent
    ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgPipesModule
  ],
  providers: [UserService, TimeAgoPipe, DatePipe],
  bootstrap: [AppComponent]
})
export class AppModule { }
