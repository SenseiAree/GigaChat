import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription, interval } from 'rxjs';
import { IFeedback } from 'src/app/interfaces/ifeedback';
import { IUser } from 'src/app/interfaces/iuser';
import { IUserProfile } from 'src/app/interfaces/iuser-profile';
import { AdminService } from 'src/app/services/admin.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent {
  isLightTheme: boolean;
  userId: number;
  user: IUser;
  userProfile: IUserProfile;
  feedbackList: IFeedback[];
  submitFeedbck: boolean = false;
  feedbackUsers: IUser[];
  feedbackUserProfiles: IUserProfile[];
  replies: string[];
  activeUserCount: number;
  userCount: number;
  averageRating: number;
  selectedFeedback: number;
  liveCountSubscription: Subscription;
  // adminReply:string;

  constructor(private _router: Router, private _userService: UserService, private _adminService: AdminService) { }
  ngOnInit(): void {
    let userId_str = sessionStorage.getItem("UserId");
    if (userId_str != undefined || userId_str != null) {
      if ((sessionStorage.getItem("UserId") != null || sessionStorage.getItem("UserId") != undefined) && (sessionStorage.getItem("RoleId") != null || sessionStorage.getItem("RoleId") != undefined)) {
        if (sessionStorage.getItem("RoleId") == "1") {
          this._router.navigate(['/chat/messages'])
        } else if (sessionStorage.getItem("RoleId") == "2"){
          //this._router.navigate(['/admin'])
        }
      }
      
      this.userId = parseInt(userId_str)

      this.getUser(this.userId);
    }
    else {
      this._router.navigate(['/'])
    }

  }

  getUser(userId: number) {
    console.log(userId)
    this._userService.getUser(userId).subscribe(
      (responseData) => {
        this.user = responseData;
        this.getUserProfile(userId);
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
        if (responseData != null) {
          this.userProfile = responseData;
          this.userProfile.theme == "Light" ? this.isLightTheme = true : this.isLightTheme = false;
          this.getAllFeedbacks();
        } else {
          console.log("UserProfile Cannot be Fetched")
        }
      },
      () => { },
      () => { }
    );
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
        else {
          console.log("Theme Update Failed. Reason: " + responseData);
        }
      },
      (responseError) => {
        console.log("Theme Update Failed. Reason: " + responseError);
      },
      () => { }
    )
  }

  logOut() {
    if (confirm("Are you sure you want to log out?")) {
      if (this.userId != null || this.userId != undefined) {
        this._userService.logout(this.userId).subscribe(
          (responseData) => {
            if (responseData) {
              sessionStorage.clear();
              this.liveCountSubscription.unsubscribe();
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

  getActiveUsersCount() {
    this._adminService.getActiveUsersCount().subscribe(
      (responseData) =>{
        console.log(responseData)
        this.activeUserCount = responseData;
        this.getUsersCount();
      },
      (responseError) => {
        this.activeUserCount = -1;
        console.log(`Failed to fetch Active User Count. Reason: ${responseError}`);
      }
      );
    }
  getUsersCount() {
    this._adminService.getUsersCount().subscribe(
      (responseData) => {
        this.userCount = responseData;
        this.getAverageRating();
      },
      (responseError) => {
        this.userCount = -1;
        console.log(`Failed to fetch User Count. Reason: ${responseError}`);        
      }
      )
    }
    getAverageRating() {
      this._adminService.getAverageRating().subscribe(
        (responseData) => {
        this.averageRating = responseData;
      },
      (responseError) => {
        this.averageRating = -1;
        console.log(`Failed to fetch Average Rating. Reason: ${responseError}`);                
      }
    );
  }
  getAllFeedbacks() {
    this._adminService.getAllFeedbacks().subscribe(
      results => {
        this.feedbackList = results;
        this.replies = [];
        this.feedbackList.forEach( eachfeedback => {
          this.replies.push(eachfeedback.adminReply)
        });
        console.log(results);
        this.getAllFeedbackUsers();
      },
      error => {
        console.log(error)
      }
    )
  }

  addAdminReply(editFeedbackId: number){
    this.submitFeedbck = !this.submitFeedbck;
    this.selectedFeedback = editFeedbackId;
  }
  sendAdminReply(feedbackId:number, adminReply:string){
    if(adminReply)
    this._adminService.addReplyToFeedback(feedbackId,adminReply).subscribe(
      result =>{
        if(result)
          this.getAllFeedbacks();
        else
          console.log("failed to add feedback reply...")
      }
    )
  }

  getAllFeedbackUsers() {
    this._adminService.getAllFeedbackUsers().subscribe(
      (responseData) => {
        if (responseData != null) {
          this.feedbackUsers = responseData;
          this.getAllFeedbackUserProfiles();
        }
      },
      (responseError) => {
        console.log("Fetching User Details From Feedback Failed. Reason: "+responseError)
      },
      () => { }
    );
  }
  
  getAllFeedbackUserProfiles() {
    this._adminService.getAllFeedbackUserProfiles().subscribe(
      (responseData) => {
        if (responseData != null) {
          this.feedbackUserProfiles = responseData;
          this.liveCountSubscription = interval(2000).subscribe(
            () => {
              this.getActiveUsersCount();
            }
          );
        }
      },
      (responseError) => {
        console.log("Fetching UserProfile Details From Feedback Failed. Reason: "+responseError)
      },
      () => { }
    );
  }

}
