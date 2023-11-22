import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Observable, Subscription, interval } from 'rxjs';
import { IUser } from 'src/app/interfaces/iuser';
import { IUserProfile } from 'src/app/interfaces/iuser-profile';
import { UserService } from 'src/app/services/user.service';
@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  subscription: Subscription;
  constructor(private _router: Router, private _userService: UserService) { }
  isLightTheme: boolean;
  userId: number;
  user: IUser;
  userProfile: IUserProfile
  ngOnInit(): void {
    let userId_str = sessionStorage.getItem("UserId");
    if (userId_str != undefined || userId_str != null) {
      if ((sessionStorage.getItem("UserId") != null || sessionStorage.getItem("UserId") != undefined) && (sessionStorage.getItem("RoleId") != null || sessionStorage.getItem("RoleId") != undefined)) {
        if (sessionStorage.getItem("RoleId") == "1") {
          //this._router.navigate(['/chat/messages'])
        } else if (sessionStorage.getItem("RoleId") == "2"){
          this._router.navigate(['/admin'])
        }
      }
      this.userId = parseInt(userId_str)
      this.getUser(this.userId);
    }
    else {
      this._router.navigate(['/'])
    }

  }
  toggleTheme() {
    this._userService.toggleTheme(this.user).subscribe(
      (responseData) => {
        if (responseData == "Light") {
          this.isLightTheme = true;
          this.userProfile.theme = "Light";
          console.log("Theme Updated to Light");
        } else if (responseData == "Dark") {
          this.isLightTheme = false;
          this.userProfile.theme = "Dark";
          console.log("Theme Updated to Dark");
        }
        else{
          console.log("Theme Update Failed. Reason: " + responseData);          
        }
      },
      (responseError) => { 
        console.log("Theme Update Failed. Reason: " + responseError);
       },
      () => { }
    )
  }

  getUser(userId: number) {
    this._userService.getUser(userId).subscribe(
      (responseData) => {
        this.user = responseData;
        this.subscription =  interval(500).subscribe(() => {
          this.getUserProfile(userId);
        })        
        
      },
      (responseError) => { 
        console.log("Web Service is not working");
        sessionStorage.clear();
        this._router.navigate(['/login']);
      },
      () => { }
    );
  }

  getUserProfile(userId: number) {
    this._userService.getUserProfile(userId).subscribe(
      (responseData) => {
        this.userProfile = responseData;
        this.userProfile.theme == "Light" ? this.isLightTheme = true : this.isLightTheme = false;
      },
      () => { },
      () => { }
    );
  }

  logOut() {
    if (confirm("Are you sure you want to log out?")) {
      if (this.userId != null || this.userId != undefined) {
        this._userService.logout(this.userId).subscribe(
          (responseData) => {
            if (responseData) {
              sessionStorage.clear();
              this.subscription.unsubscribe();
              this._router.navigate(['']);
            } else {
              console.log("Logout Failed");
            }
          },
          (responseError) => {
          },
          () => { }
        );
      } else {
        this._router.navigate(['']);
      }
    }
  }
}
