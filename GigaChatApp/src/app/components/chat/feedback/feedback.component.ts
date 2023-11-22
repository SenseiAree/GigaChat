import { Component } from '@angular/core';
import { IFeedback } from 'src/app/interfaces/ifeedback';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.css']
})
export class FeedbackComponent {
  stars: number[] = [1, 2, 3, 4, 5];
  providedRating: number;
  providedFeedback: string;
  userId: number;
  userFeedbackAvailable: boolean = false;
  userFeedback: IFeedback[] = null;

  constructor(private _userService: UserService) { }
  ngOnInit() {
    this.userId = sessionStorage.getItem("UserId") != null ? parseInt(sessionStorage.getItem("UserId")) : undefined;
    this.getFeedback();
  }
  getFeedback() {
    this._userService.getFeedback(this.userId).subscribe(
      result => {
        console.log(result)
        if (result.length > 0) {
          this.userFeedback = result;
          this.userFeedbackAvailable = true;
          this.countStar(this.userFeedback[0].rating)
        }
        else {
          this.userFeedback = null;
          this.userFeedbackAvailable = false;
        }
      }
    )
  }

  submitFeedback() {
    this._userService.addFeedback(this.userId, this.providedRating, this.providedFeedback).subscribe(
      result => {
        alert("Feedback submitted succesfully.");
        this.getFeedback();
      },
      error => {
        console.log("feedback Failed");
      }
    )
  }


  countStar(star) {
    this.providedRating = star;
    let i = 0;
    for (i = 1; i <= star; i++) {
      var selectedStar = document.getElementById(i.toString())
      selectedStar.classList.remove("empty-star");
      selectedStar.classList.add("yellow-star");
    }
    for (; i <= 5; i++) {
      var selectedStar = document.getElementById(i.toString())
      selectedStar.classList.remove("yellow-star");
      selectedStar.classList.add("empty-star");

    }


  }
}
