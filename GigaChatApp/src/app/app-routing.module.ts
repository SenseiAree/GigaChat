import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/home/login/login.component';
import { SignupComponent } from './components/home/signup/signup.component';
import { ChatComponent } from './components/chat/chat.component';
import { MessageComponent } from './components/chat/message/message.component';
import { ProfileComponent } from './components/chat/profile/profile.component';
import { ContactComponent } from './components/chat/contact/contact.component';
import { FeedbackComponent } from './components/chat/feedback/feedback.component';
import { AdminComponent } from './components/admin/admin.component';

const routes: Routes = [
  {
    path: "", component: HomeComponent, children: [
      { path: "login", component: LoginComponent },
      { path: "signup", component: SignupComponent }
    ]
  },
  {
    path: "chat", component: ChatComponent, children: [
      { path: "", component: MessageComponent },
      {
        path: "messages", component: MessageComponent
      },
      { path: "profile", component: ProfileComponent },
      { path: "contacts", component: ContactComponent },
      { path: "feedback", component: FeedbackComponent },
      { path: "**", component: MessageComponent }
    ]
  },
  {
    path: "admin", component: AdminComponent
  }

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
