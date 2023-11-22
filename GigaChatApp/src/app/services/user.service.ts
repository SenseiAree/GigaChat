import { Observable, catchError, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParamsOptions } from '@angular/common/http';
import { IUser } from '../interfaces/iuser';
import { IChat } from '../interfaces/ichat';
import { Injectable } from '@angular/core';
import { IUserProfile } from '../interfaces/iuser-profile';
import { IMessage } from '../interfaces/imessage';
import { IFeedback } from '../interfaces/ifeedback';
import { IFriendList } from '../interfaces/ifriend-list';
import { IStarredMessage } from '../interfaces/istarred-message';
import { IMessageCount } from '../interfaces/imessageCount';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private _http: HttpClient) { }

  loadProfilePictures(): string[] {
    return [
      'Anonymous/anonymous.png',
      'Anonymous/spyware.png',
      'Animals/cat.png',
      'Animals/dog.png',
      'Animals/fox.png',
      'Animals/chicken.png',
      'Animals/buddy.png',
      'Animals/buddy (1).png',
      'Men/gamer.png',
      'Men/man.png',
      'Men/man (1).png',
      'Men/man (2).png',
      'Men/man (3).png',
      'Men/man (4).png',
      'Women/woman.png',
      'Women/woman (1).png',
      'Women/woman (2).png',
      'Women/woman (3).png',
      'Women/user.png',
      'Women/girl.png',

    ];
  }

  loadUnknownPicture(): string {
    return 'Anonymous/anonymous.png'
  }

  toggleTheme(user: IUser): Observable<string> {
    let status: Observable<string> = this._http.put<string>(`https://localhost:57932/api/User/ToggleTheme`, user)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  login(emailId: string, password: string): Observable<IUser> {
    let status: Observable<IUser> = this._http.get<IUser>(`https://localhost:57932/api/User/ValidateCredentials?emailId=${emailId}&password=${password}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  signUp(user: IUser): Observable<IUser> {
    let status: Observable<IUser> = this._http.post<IUser>(`https://localhost:57932/api/User/RegisterUser`, user)
      .pipe(catchError(this.errorHandler));
    return status;

  }

  errorHandler(error: HttpErrorResponse) {
    console.error(error);
    return throwError(error.message || "Server Error");
  }

  getChats(userId: number): Observable<IChat[]> {
    let status: Observable<IChat[]> = this._http.get<IChat[]>(`https://localhost:57932/api/User/GetChats?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getUnreadChats(userId: number): Observable<IMessageCount[]> {
    return this._http.get<IMessageCount[]>(`https://localhost:57932/api/User/GetUnreadMessages?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
  }

  getChatUsersFromUserId(userId: number): Observable<IUser[]> {
    let status: Observable<IUser[]> = this._http.get<IUser[]>(`https://localhost:57932/api/User/GetChatUsersFromUserId?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getChatUserProfilesFromUserId(userId: number): Observable<IUserProfile[]> {
    let status: Observable<IUserProfile[]> = this._http.get<IUserProfile[]>(`https://localhost:57932/api/User/GetChatUserProfilesFromUserId?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getMessagesFromChatId(chatId: number): Observable<IMessage[]> {
    let status: Observable<IMessage[]> = this._http.get<IMessage[]>(`https://localhost:57932/api/User/GetMessagesFromChatId?chatId=${chatId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  deleteMessageFromChatId(chatId: number): Observable<boolean> {
    let status: Observable<boolean> = this._http.delete<boolean>(`https://localhost:57932/api/User/DeleteChat?chatId=${chatId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  logout(userId: number): Observable<boolean> {
    let status: Observable<boolean> = this._http.put<boolean>(`https://localhost:57932/api/User/Logout?userId=${userId}`, null)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getUser(userId: number): Observable<IUser> {
    let status: Observable<IUser> = this._http.get<IUser>(`https://localhost:57932/api/User/GetUser?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getUserProfile(userId: number): Observable<IUserProfile> {
    let status: Observable<IUserProfile> = this._http.get<IUserProfile>(`https://localhost:57932/api/User/GetUserProfile?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  sendMessage(message: IMessage): Observable<number> {
    let status: Observable<number> = this._http.post<number>(`https://localhost:57932/api/User/SendMessage`, message)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  updateAvailabilityStatus(userId: number, availabilityStatus: string): Observable<boolean> {
    let status: Observable<boolean> = this._http
      .put<boolean>(
        `https://localhost:57932/api/User/UpdateAvailabilityStatus?userId=${userId}&newAvailabilityStatus=${availabilityStatus}`
        , null)
      .pipe(catchError(this.errorHandler));
    return status;
  }
  getAllUsers(): Observable<IUser[]> {
    let status: Observable<IUser[]> = this._http.get<IUser[]>(`https://localhost:57932/api/User/GetAllUsers`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getAllUserProfiles(): Observable<IUserProfile[]> {
    let status: Observable<IUserProfile[]> = this._http.get<IUserProfile[]>(`https://localhost:57932/api/User/GetAllUserProfiles`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getFriendList(userId: number): Observable<IFriendList[]> {
    let status: Observable<IFriendList[]> = this._http.get<IFriendList[]>(`https://localhost:57932/api/User/GetFriendList?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  removeContact(friend: IFriendList): Observable<boolean> {
    //To do Implementation
    let httpOptions = {
      headers: new HttpHeaders({ 'content-type': 'application/json' }),
      body: friend
    }
    let status: Observable<boolean> = this._http.delete<boolean>(`https://localhost:57932/api/User/DeleteFriend`, httpOptions)
      .pipe(catchError(this.errorHandler));
    return status;
  }
  addContact(friend: IFriendList): Observable<boolean> {
    //To do Implementation
    let status: Observable<boolean> = this._http.post<boolean>(`https://localhost:57932/api/User/AddFriend`, friend)
      .pipe(catchError(this.errorHandler));
    return status;
  }
  unblockContact(friend: IFriendList): Observable<boolean> {
    //To do Implementation
    let status: Observable<boolean> = this._http.put<boolean>(`https://localhost:57932/api/User/UnblockUser`, friend)
      .pipe(catchError(this.errorHandler));
    return status;
  }
  blockContact(friend: IFriendList): Observable<boolean> {
    //To do Implementation
    let status: Observable<boolean> = this._http.put<boolean>(`https://localhost:57932/api/User/BlockUser`, friend)
      .pipe(catchError(this.errorHandler));
    return status;
  }
  deleteMessage(messageId: number, userId: number): Observable<number> {
    let status: Observable<number> = this._http.delete<number>(`https://localhost:57932/api/User/DeleteMessage?messageId=${messageId}&userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }
  toggleStarMessage(messageId: number, userId: number): Observable<number> {
    let status: Observable<number> = this._http.get<number>(`https://localhost:57932/api/User/ToggleStarMessage?messageId=${messageId}&userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }
  getStarredMessages(userId: number): Observable<IStarredMessage[]> {
    let status: Observable<IStarredMessage[]> = this._http.get<IStarredMessage[]>(`https://localhost:57932/api/User/GetStarredMessages?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  addFeedback(userId: number, rating: number, feedbackContetn: string): Observable<boolean> {
    let feedbackObj = {
      "UserId": userId,
      "UserFeedback": feedbackContetn,
      "Rating": rating
    }
    return this._http.post<boolean>('https://localhost:57932/api/User/AddFeedback', feedbackObj);
  }
  updateDisplayName(userId: number, displayName: string): Observable<boolean> {
    var status: Observable<boolean> = this._http.put<boolean>(`https://localhost:57932/api/User/UpdateDisplayName?userId=${userId}&displayName=${displayName}`, null)
      .pipe(catchError(this.errorHandler));
    return status
  }

  updateAvatar(userId: number, newAvatar: string): Observable<boolean> {
    var status: Observable<boolean> = this._http.put<boolean>(`https://localhost:57932/api/User/UpdateAvatar?userId=${userId}&newAvatar=${newAvatar}`, null)
      .pipe(catchError(this.errorHandler));
    return status
  }

  addChat(chat: IChat): Observable<number> {
    var status: Observable<number> = this._http.post<number>(`https://localhost:57932/api/User/AddChat`, chat)
      .pipe(catchError(this.errorHandler));
    return status
  }

  getFeedback(userId: number): Observable<IFeedback[]> {
    var status: Observable<IFeedback[]> = this._http.get<IFeedback[]>(`https://localhost:57932/api/User/GetFeedback?userId=${userId}`)
      .pipe(catchError(this.errorHandler));
    return status
  }

  editMessage(message: IMessage): Observable<number> {
    var status: Observable<number> = this._http.put<number>(`https://localhost:57932/api/User/EditMessage`, message)
      .pipe(catchError(this.errorHandler));
    return status
  }
  // getAllFeedback(): Observable<IFeedback[]> {
  //   var feedbackList: Observable<IFeedback[]> = this._http.get<IFeedback[]>(`https://localhost:57932/api/User/GetAllFeedbacks`)
  //     .pipe(catchError(this.errorHandler));
  //   return feedbackList
  // }
  getTopFeedbacks(): Observable<IFeedback[]> {
    var status: Observable<IFeedback[]> = this._http.get<IFeedback[]>(`https://localhost:57932/api/User/GetTopFeedbacks`)
      .pipe(catchError(this.errorHandler));
    return status
  }
}
