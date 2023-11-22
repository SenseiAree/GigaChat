import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import AOS from "aos";
import { IFeedback } from 'src/app/interfaces/ifeedback';
import { IUser } from 'src/app/interfaces/iuser';
import { IUserProfile } from 'src/app/interfaces/iuser-profile';
import { UserService } from 'src/app/services/user.service';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  rating: number;
  latestFeedback: IFeedback[];
  isLighttheme: boolean;
  users: IUser[];
  userProfiles: IUserProfile[];
  constructor(private _router: Router, private _userService: UserService) { }
  ngOnInit(): void {
    AOS.init();

    if ((sessionStorage.getItem("UserId") != null || sessionStorage.getItem("UserId") != undefined) && (sessionStorage.getItem("RoleId") != null || sessionStorage.getItem("RoleId") != undefined)) {
      if (sessionStorage.getItem("RoleId") == "1") {
        this._router.navigate(['/chat/messages'])
      } else if (sessionStorage.getItem("RoleId") == "2"){
        this._router.navigate(['/admin'])
      }
    }

    this._router.events.subscribe(
      (event) => {
        if (event instanceof NavigationEnd) {
          window.scroll({ top: 0, behavior: 'smooth' });
        }
      }
    );



    this.rating = 2;
    this.isLighttheme = true;
    this.getTopFeedbacks();
  }
  scrollTo() {
    window.scroll({ top: 0, behavior: 'smooth' });
  }
  getTopFeedbacks() {
    this._userService.getTopFeedbacks().subscribe(
      (responseData) => {
        this.latestFeedback = responseData;
        this.getAllUsers();
      },
      (responseError) => {
        this.latestFeedback = [
          { userId: 1001, rating: 4, postedAt: new Date(), feedbackId: 101, userFeedback: "I really love this application.", adminReply: "", adminReplyTime: undefined },
          { userId: 1002, rating: 5, postedAt: new Date(), feedbackId: 101, userFeedback: "Its a fun place to pass time and connect.", adminReply: "", adminReplyTime: undefined },
        ];
        console.log("Unable to Fetch Feedbacks. Reason: "+ responseError)
      }
    );
    

  }

  getAllUsers() {
    this._userService.getAllUsers().subscribe(
      (responseData) => {
        this.users = responseData;
        this.getAllUserProfiles();
      },
      () => { },
      () => { }
    );
  }
  getAllUserProfiles() {
    this._userService.getAllUserProfiles().subscribe(
      (responseData) => {
        this.userProfiles = responseData;
      },
      () => { },
      () => { }
    );
  }

  getNameFromUserId(userId: number): string {
    var name: string;
    if(this.users != null || this.users != undefined){
      name = this.users.find(user => user.userId === userId).displayName;
    }else{
      name = "Anonymous User";
    }
    return name;
  }
  getPictureFromUserId(userId: number): string {
    var avatar: string;
    if(this.userProfiles != null || this.userProfiles != undefined){
      avatar = this.userProfiles.find(userProfile => userProfile.userId === userId).avatar;
    }else{
      avatar = "Anonymous/spyware.png";
    }
    return avatar;    
  }
}
