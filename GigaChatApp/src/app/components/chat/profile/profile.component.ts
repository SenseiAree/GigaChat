import { Component, OnInit } from '@angular/core';
import { IUser } from 'src/app/interfaces/iuser';
import { IUserProfile } from 'src/app/interfaces/iuser-profile';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit{
  constructor(private _userService: UserService){}
  userId: number;
  availabilityStatus: string;
  displayName: string;
  user: IUser;
  userProfile: IUserProfile;
  selectedAvatar: string;
  updateMode: boolean;
  updateName:boolean;
  pictures: string[];
  ngOnInit(): void {
    this.updateMode = false;
    this.updateName = false;
    this.pictures = this._userService.loadProfilePictures();
    this.userId = sessionStorage.getItem("UserId") != null? parseInt(sessionStorage.getItem("UserId")) : undefined;
    if(this.userId != undefined){
      this.getUser(this.userId);
    }
  }
  getUser(userId: number) {
    this._userService.getUser(userId).subscribe(
      (responseData) => {
        this.user = responseData;
        this.displayName = this.user.displayName;        
        this.getUserProfile(userId);
      },
      () => {},
      () => {}
    );
  }
  getUserProfile(userId: number){
    this._userService.getUserProfile(userId).subscribe(
      (responseData) => {
        this.userProfile = responseData;
        this.availabilityStatus = responseData.availabilityStatus;
        this.selectedAvatar = this.userProfile.avatar;        
      },
      () => {},
      () => {}
    );
  }
  updateUserProfilePicture(avatar:string){
    console.log(this.selectedAvatar,this.displayName,this.availabilityStatus);
    //To Do Implementation
    this._userService.updateAvatar(this.userId, avatar).subscribe(
      result=>{
        if(result==true){
          this.selectedAvatar = avatar;
          console.log("Profile Picture updated");
        }
        else
          console.log("update failed");
      },
      error=>{
        console.log(error);
      }
    )

  }

  updateDisplayName(){
    this._userService.updateDisplayName(this.userId, this.displayName).subscribe(
      result=>{
        if(result==true)
          console.log("Display Name updated");
        else
          console.log("update failed");
      },
      error=>{
        console.log(error);
      }
    )
  }

  updateAvailibilityStatus(){
    this._userService.updateAvailabilityStatus(this.userId, this.availabilityStatus).subscribe(
      result=>{
        if(result==true)
          console.log("status updated");
        else
          console.log("update failed");
      },
      error=>{
        console.log(error);
      }
    )
  }
  cancelUpdate(){
    this.selectedAvatar = this.userProfile.avatar; 
    this.availabilityStatus = this.userProfile.availabilityStatus
    this.displayName = this.user.displayName; 
    this.updateMode = false;

  }
}
