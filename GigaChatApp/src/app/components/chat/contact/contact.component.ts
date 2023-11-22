import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IsFriend } from 'src/app/enums/is-friend';
import { IChat } from 'src/app/interfaces/ichat';
import { IFriendList } from 'src/app/interfaces/ifriend-list';
import { IUser } from 'src/app/interfaces/iuser';
import { IUserProfile } from 'src/app/interfaces/iuser-profile';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {
  constructor(private _userService: UserService, private _router: Router) { }
  sessionUserId: number;
  users: IUser[];
  userProfiles: IUserProfile[];
  friendList: IFriendList[];
  searchUsers: string;
  ngOnInit(): void {
    this.initSessionUserId();
    if (this.sessionUserId != undefined) {
      this.getAllUsers();
    }
  }

  filterUsersNotBlocked(): IUser[]{
    let status: IUser[];
    status = this.users;
    
    var friendIds: number[] = []
    this.friendList.forEach(eachFriend =>{
      if(eachFriend.isBlocked == false){
      friendIds.push(eachFriend.friendId);
      }
    });
    status = this.users.filter(user => friendIds.includes(user.userId))

    if((this.searchUsers != null || this.searchUsers != undefined) && this.searchUsers.length > 0){
      status = this.users.filter(user => user.displayName.includes(this.searchUsers) || user.emailId.includes(this.searchUsers));
    } 
    return status;
  }

  getDynamicUserProfile(userId: number): IUserProfile{
    var status: IUserProfile = this.userProfiles.find(userProfile => userProfile.userId == userId);
    return status;
  }

  filterBlockedUsers(): IUser[]{
    var status: IUser[] = [];
    var friendIds: number[] = []
    this.friendList.forEach(eachFriend =>{
      if(eachFriend.isBlocked == true){
      friendIds.push(eachFriend.friendId);
      }
    });
    status = this.users.filter(user => friendIds.includes(user.userId))

    if((this.searchUsers != null || this.searchUsers != undefined) && this.searchUsers.length > 0){
      status = [];
    }
    // console.log(status);
    return status
  }

  private initSessionUserId() {
    let temp: string = sessionStorage.getItem("UserId");
    this.sessionUserId = (temp != null || temp != undefined) ? parseInt(temp) : undefined;
  }

  getAllUsers() {
    this._userService.getAllUsers().subscribe(
      (responseData) => {
        this.users = responseData.filter(user => user.userId != this.sessionUserId);
        this.getAllUserProfiles();
      },
      () => { },
      () => { }
    );
  }
  getAllUserProfiles() {
    this._userService.getAllUserProfiles().subscribe(
      (responseData) => {
        this.userProfiles = responseData.filter(user => user.userId != this.sessionUserId);
        this.getFriendList();
      },
      () => { },
      () => { }
    );
  }

  getFriendList() {
    this._userService.getFriendList(this.sessionUserId).subscribe(
      (responseData) => {
        this.friendList = responseData;
      },
      () => { },
      () => { }
    );
  }

  isFriend(friendId: number): IsFriend {
    var friend: IFriendList;
    var temp: IFriendList[] = this.friendList.filter(
      friend => friend.userId == this.sessionUserId && friend.friendId == friendId
    )
    if ((temp != null || temp != undefined) && temp.length > 0) {
      friend = temp[0];
      if (friend.isBlocked == true) {
        return IsFriend.Blocked
      } else {
        return IsFriend.Yes
      }
    } else {
      return IsFriend.No;
    }
  }

  public get IsFriend(): typeof IsFriend {
    return IsFriend
  }

  removeContact(user: IUser) {
    let userId: number = this.sessionUserId;
    var friend: IFriendList = {
      friendId: user.userId,
      userId: userId,
      isBlocked: this.isFriend(user.userId) == IsFriend.Blocked
    }
    this._userService.removeContact(friend).subscribe(
      (responseData) => {
        if (responseData == true) {
          this.friendList = this.friendList.filter(otherFriend => otherFriend.friendId != friend.friendId);
        }
      },
      () => { },
      () => { }
    );
  }
  addContact(user: IUser) {
    let userId: number = this.sessionUserId;
    var friend: IFriendList = {
      friendId: user.userId,
      userId: userId,
      isBlocked: this.isFriend(user.userId) == IsFriend.Blocked
    }
    this._userService.addContact(friend).subscribe(
      (responseData) => {
        console.log(responseData)
        if (responseData) {
          
          this.friendList.push(friend);
        }
      },
      () => { },
      () => { }
    );
  }
  unblockContact(user: IUser) {
    let userId: number = this.sessionUserId;
    var friend: IFriendList = {
      friendId: user.userId,
      userId: userId,
      isBlocked: this.isFriend(user.userId) == IsFriend.Blocked
    }
    this._userService.unblockContact(friend).subscribe(
      (responseData) => {
        if (responseData == true) {
          this.friendList.find(
            matchingFriend => matchingFriend.friendId == friend.friendId && matchingFriend.userId == friend.userId
          ).isBlocked = false;
        }
      },
      () => { },
      () => { }
    );
  }
  blockContact(user: IUser) {
    let userId: number = this.sessionUserId;
    var friend: IFriendList = {
      friendId: user.userId,
      userId: userId,
      isBlocked: this.isFriend(user.userId) == IsFriend.Blocked
    }
    this._userService.blockContact(friend).subscribe(
      (responseData) => {
        if (responseData == true) {
          this.friendList.find(
            matchingFriend => matchingFriend.friendId == friend.friendId && matchingFriend.userId == friend.userId
          ).isBlocked = true;
        }
      },
      () => { },
      () => { }
    );
  }

  addChat(userId: number){
    var chat: IChat = {
      chatId: undefined,
      initiatorId: this.sessionUserId,
      recipientId: userId
    }
    this._userService.addChat(chat).subscribe(
      (responseData) => {
        if(responseData > 0){
          chat.chatId = responseData;
          this._router.navigate(['/chat/messages'])
        }
      },
      () => {},
      () => {}
    );
  }

}
