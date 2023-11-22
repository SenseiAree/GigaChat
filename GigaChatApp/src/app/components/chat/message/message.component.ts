import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { TimeAgoPipe } from 'ngx-pipes';
import { IsFriend } from 'src/app/enums/is-friend';
import { IChat } from 'src/app/interfaces/ichat';
import { IFriendList } from 'src/app/interfaces/ifriend-list';
import { IMessage } from 'src/app/interfaces/imessage';
import { IMessageCount } from 'src/app/interfaces/imessageCount';
import { IStarredMessage } from 'src/app/interfaces/istarred-message';
import { IUser } from 'src/app/interfaces/iuser';
import { IUserProfile } from 'src/app/interfaces/iuser-profile';
import { UserService } from 'src/app/services/user.service';
import { Subscription, interval } from 'rxjs';


@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit, OnDestroy {
  user: IUser;
  userProfile: IUserProfile;
  userIdOfMessageToBeDisplayed: number;
  returnDate(date: Date) {
    let dateResult: Date = new Date(date);

    let timeAgo: string = this._timeAgoPipe.transform(dateResult)
    let datePipe: string = this._datePipe.transform(dateResult, 'dd-MM-yyyy, hh:MM')

    if (timeAgo.includes("month") || timeAgo.includes("year") || timeAgo.includes("week") || timeAgo.includes("days")) {
      return datePipe
    }
    else {
      return timeAgo
    }
  }
  friendList: IFriendList[];
  // editMode: boolean = false;
  constructor(private _router: Router, private _userService: UserService, private _timeAgoPipe: TimeAgoPipe, private _datePipe: DatePipe) { }
  content: string;
  showMessages: boolean;
  chatId: number;
  isEditing: boolean;
  chats: IChat[];
  users: IUser[];
  userProfiles: IUserProfile[];
  messages: IMessage[];
  unreadMessageCount: {};
  userId: number;
  
  friendId: number;
  starredMessages: IStarredMessage[];
  refreshMessageSubscription: Subscription;
  now = new Date();
  searchUsers: string;
  autoRefresh: boolean = false;
  messageSearched: string;
  roleId: number;
  cachefiles: { content: string, message: IMessage } = { content: undefined, message: undefined };

  getAvatar(): string{
    return this.userProfiles.find(p => p.userId == this.userIdOfMessageToBeDisplayed).avatar
  }
  getDisplayName(): string{
    return this.users.find(p => p.userId == this.userIdOfMessageToBeDisplayed).displayName
    
  }

  ngOnInit(): void {
    this.userId = sessionStorage.getItem("UserId") != null ? parseInt(sessionStorage.getItem("UserId")) : undefined;
    if (this.userId != undefined) {
      this.getChats(this.userId);
      
    }
    this.showMessages = false;
    this.isEditing = false;
    
    this.refreshMessageSubscription =  interval(1000) // 2000 milliseconds = 2 seconds
    .subscribe(() => {
      this.refreshMessageCount(undefined, false);
      });      
    }
    ngOnDestroy(): void {
      this.refreshMessageSubscription.unsubscribe();
    }
    
  filterMessages(): IMessage[]{
    let filteredMessage: IMessage[] = this.messages;
    if((this.messageSearched != undefined || this.messageSearched != null) && this.messageSearched != ""){
      filteredMessage = filteredMessage.filter(eachMessage => eachMessage.content.includes(this.messageSearched));
    }
    return filteredMessage;
  }

  findAvailability(userId: number): string{
    let status: string = this.userProfiles.find(p => p.userId == userId).availabilityStatus
    return status
  }

  pressedEnter(event: KeyboardEvent){
    if(event.key == 'Enter'){
      if(this.isEditing == true){
        let editButton: HTMLElement = document.getElementById("edit-button");        
        editButton.click();
      } else {
        let sendButton: HTMLElement = document.getElementById("send-button");        
        sendButton.click();        
      }
    }
  }

  refreshMessageCount(divElementToBeRotated: HTMLDivElement, manualRefresh: boolean) {
    if (divElementToBeRotated != undefined) {
      let transform: string = divElementToBeRotated.style.transform;
      divElementToBeRotated.style.transition = "transform 1s ease-in-out"
      divElementToBeRotated.style.transform = "rotateZ(180deg)";
      setTimeout(() => {
        divElementToBeRotated.style.transform = transform;
      }, 1000)
    }
    if (this.autoRefresh || manualRefresh) {
      console.log("refresh called")
      this.unreadMessageCount = this.getUnreadMessage();
    }
  }

  manualRefreshMessageCount() {
    this.unreadMessageCount = this.getUnreadMessage();
  }

  toggleAutoRefresh() {
    console.log("toggle called")
    console.log("toggle: ", this.autoRefresh);
    this.autoRefresh = !this.autoRefresh;
    console.log(this.autoRefresh);
  }

  deleteMessgeFromChatId(userId: number) {
    if (confirm("Are you sure you want to all your sent messages?")) {
      let chatId = this.getChatId(userId);
      console.log(chatId, userId)
      this._userService.deleteMessageFromChatId(chatId).subscribe(
        result => {
          if (result == true)
            console.log("delete successfull")
          else {
            console.log("delete failed")
          }
        },
        error => {
          console.log(error);
        }
      )
    }
  }

  //findPicture(user: IUser): string {
  //  let avatar: string;
  //  try {
  //    avatar = this.userProfiles.find(userProfile => userProfile.userId == user.userId).avatar
  //  } catch {
  //    avatar = null;
  //  }
  //  return avatar
  //}

  getUnreadMessage() {
    console.log("messageCountUpdateCalled")
    let umc = {}
    this._userService.getUnreadChats(this.userId).subscribe(
      result => {
        if (result) {
          result.forEach(element => {
            let sId = element.senderId.toString();
            umc[sId] = element.count;
            if(this.messages != null){
              this.getMessagesFromUserId(this.userIdOfMessageToBeDisplayed);
            }
          });
        }
      },
      error => {
        console.log(error);
      }
    )
    console.log(umc)
    return umc;


  }

  editMode(message: IMessage) {
    this.isEditing = true;
    this.cachefiles.content = this.content;
    this.cachefiles.message = message;
    this.content = message.content;
  }

  cancelEditMode() {
    this.isEditing = false;
    this.content = this.cachefiles.content
  }

  findPicture(user: IUser): string {

    let avatar: string;

    try {

      avatar = this.userProfiles.find(userProfile => userProfile.userId == user.userId).avatar

    } catch {

      avatar = null;

    }

    return avatar

  }
  filterUsers(): IUser[] {
    let status: IUser[];
    if ((this.searchUsers != null || this.searchUsers != undefined) && this.searchUsers.length > 0) {
      status = this.users.filter(user => user.displayName.includes(this.searchUsers) || user.emailId.includes(this.searchUsers));

    } else {
      status = this.users;
    }
    return status;
  }

  // getUnreadMessageCount(userId: number){
  //    let message = this.unreadMessageCount.find(user => user.senderId == userId)
  //    if(message){
  //     return message.count;
  //    }
  //    else{
  //     return 0;
  //    }
  // }

  isFriend(friendId: number): IsFriend {
    var friend: IFriendList;
    var temp: IFriendList[] = this.friendList.filter(
      friend => friend.userId == this.userId && friend.friendId == friendId
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

  isStarred(messageId: number): boolean {
    let status: boolean = this.starredMessages.find(starredMessage => starredMessage.messageId == messageId && starredMessage.userId == this.userId) != undefined;
    return status
  }


  getChatId(userId: number) {
    var tempChats = this.chats;
    if (tempChats != null) {
      let chat: IChat = tempChats.filter((c) => c.initiatorId == userId || c.recipientId == userId).pop();
      return chat.chatId;
    }
    return -96;
  }

  getChats(userId: number) {
    this._userService.getChats(userId).subscribe(
      (responseData) => {
        this.chats = responseData;
        this.getChatUsersFromUserId(userId);
      },
      (responseError) => {
        console.log("Chats fetch failed");
      },
      () => { }
    );
  }

  getChatUsersFromUserId(userId: number) {
    this._userService.getChatUsersFromUserId(userId).subscribe(
      (responseData) => {
        this.users = responseData;
        this.getChatUserProfilesFromUserId(userId);
      },
      (responseError) => {
        console.log("Users cannot be fetched")
      },
      () => { }
    );
  }

  getChatUserProfilesFromUserId(userId: number) {
    this._userService.getChatUserProfilesFromUserId(userId).subscribe(
      (responseData) => {
        this.userProfiles = responseData;
        this.getFriendList(userId);
      },
      (responseError) => {
        console.log("Profiles cannot be fetched")
      },
      () => { }
    )
  }

  getFriendList(userId: number) {
    this._userService.getFriendList(userId).subscribe(
      (responseData) => {
        this.friendList = responseData;
        this.getStarredMessages(userId);
      },
      () => { },
      () => { }
    );
  }

  getMessagesFromUserId(userId: number) {
    this.userIdOfMessageToBeDisplayed = userId
    this.friendId = userId;
    let chatId: number = this.getChatId(userId);

    this._userService.getMessagesFromChatId(chatId).subscribe(
      (responseData) => {
        this.messages = responseData;
        this.showMessages = true;
        setTimeout(
          () => {
            document.getElementsByClassName('load-conversation-view')[0].scroll({ top: document.getElementsByClassName('load-conversation-view')[0].clientHeight, behavior: 'smooth' })
            this.unreadMessageCount[userId] = 0;
          }
          , 100)
      },
      (responseError) => {
        console.log(`Fetch message failed. Reason: ${responseError}`)
      },
      () => { }
    );
  }

  sendMessage() {
    let message: IMessage = {
      senderId: this.userId,
      receiverId: this.users.find(u => u.userId == this.userIdOfMessageToBeDisplayed).userId,
      content: this.content,

      chatId: 0,
      messageId: 0,
      isRead: null,
      sentTime: new Date()
    }
    console.log(message.senderId, message.receiverId, message.content);
    this._userService.sendMessage(message).subscribe(
      (responseData) => {
        console.log(responseData);
        if (responseData > 0) {
          console.log("Message Sent Successfully"); this.content = "";
          message.messageId = responseData;
          this.messages.push(message);
          setTimeout(
            () => {
              // document.getElementsByClassName('load-conversation-view')[0].scroll({ top: document.getElementsByClassName('load-conversation-view')[0].clientHeight, behavior: 'smooth' })
              document.getElementsByClassName('load-conversation-view')[0].scroll({ top: document.getElementsByClassName('load-conversation-view')[0].scrollHeight, behavior: 'smooth' })
            }
            , 100);
        }
      },
      () => { },
      () => { }
    );
  }

  editMessage() {
    if (this.cachefiles.message != undefined) {
      let message: IMessage = this.cachefiles.message;
      message.content = this.content;
      this._userService.editMessage(message).subscribe(
        (responseData) => {
          if (responseData) {
            let indexToBeFound: number = this.messages.findIndex(messageToBeFound => messageToBeFound.messageId == message.messageId);
            this.messages[indexToBeFound] = message;
            this.cancelEditMode();
          } else {
            console.log(`Message Edition Failed. Output Returned: ${responseData}`);
          }
        },
        (responseError) => {
          console.log(`Message Edition Failed. Error Returned: ${responseError}`)
        },
        () => { }
      );




    }
  }

  deleteMessage(messageId: number) {
    this._userService.deleteMessage(messageId, this.userId).subscribe(
      (responseData) => {
        if (responseData == 1) {
          this.messages = this.messages.filter(message => message.messageId != messageId);
        } else {
          console.log(responseData)
        }
      },
      () => { },
      () => { }
    );
  }

  getStarredMessages(userId: number) {
    //To do Implementation
    this._userService.getStarredMessages(userId).subscribe(
      (responseData) => {
        this.starredMessages = responseData;
        this.getUser(this.userId);
      },
      () => { },
      () => { }
    );
  }

  toggleStarMessage(message: IMessage) {
    let starredMessage: IStarredMessage = {
      messageId: message.messageId,
      starredMessageId: undefined,
      userId: this.userId
    }
    this._userService.toggleStarMessage(message.messageId, this.userId).subscribe(
      (responseData) => {
        if (responseData == 1) {
          this.starredMessages.push(starredMessage);


        } else if (responseData == 0) {
          this.starredMessages = this.starredMessages.filter(eachStarredMessage => eachStarredMessage.messageId != starredMessage.messageId && eachStarredMessage.userId != starredMessage.userId)
        }
      },
      () => { },
      () => { }
    );
  }

  getUser(userId: number) {
    this._userService.getUser(userId).subscribe(
      (responseData) => {
        this.user = responseData;
        this.getUserProfile(userId);
      },
      () => { },
      () => { }
    );
  }

  getUserProfile(userId: number) {
    this._userService.getUserProfile(userId).subscribe(
      (responseData) => {
        this.userProfile = responseData;
        this.unreadMessageCount = this.getUnreadMessage();
      },
      () => { },
      () => { }
    );
  }

}
