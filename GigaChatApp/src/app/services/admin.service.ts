import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { IUser } from '../interfaces/iuser';
import { IFeedback } from '../interfaces/ifeedback';
import { IUserProfile } from '../interfaces/iuser-profile';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private _http: HttpClient) { }
  errorHandler(error: HttpErrorResponse) {
    console.error(error);
    return throwError(error.message || "Server Error");
  }

  getAllFeedbackUsers(): Observable<IUser[]> {
    let status: Observable<IUser[]> = this._http.get<IUser[]>(`https://localhost:57932/api/Admin/GetAllFeedbackUsers`)
      .pipe(catchError(this.errorHandler));
    return status;
  }
  
  getAllFeedbackUserProfiles(): Observable<IUserProfile[]> {
    let status: Observable<IUserProfile[]> = this._http.get<IUserProfile[]>(`https://localhost:57932/api/Admin/GetAllFeedbackUserProfiles`)
    .pipe(catchError(this.errorHandler));
  return status;
  }

  getAverageRating(): Observable<number> {
    let status: Observable<number> = this._http.get<number>(`https://localhost:57932/api/Admin/GetAverageRating`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getActiveUsersCount(): Observable<number> {
    let status: Observable<number> = this._http.get<number>(`https://localhost:57932/api/Admin/GetActiveUsersCount`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getUsersCount(): Observable<number> {
    let status: Observable<number> = this._http.get<number>(`https://localhost:57932/api/Admin/GetUsersCount`)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  addReplyToFeedback(feedbackId: number, adminReply: string): Observable<boolean> {
    let status: Observable<boolean> = this._http.put<boolean>(`https://localhost:57932/api/Admin/AddReplyToFeedback?feedBackId=${feedbackId}&reply=${adminReply}`,null)
      .pipe(catchError(this.errorHandler));
    return status;
  }

  getAllFeedbacks() {
    let status: Observable<IFeedback[]> = this._http.get<IFeedback[]>(`https://localhost:57932/api/Admin/GetAllFeedbacks`)
      .pipe(catchError(this.errorHandler));
    return status;
  }
}
