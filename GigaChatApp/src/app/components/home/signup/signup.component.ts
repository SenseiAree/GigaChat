import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import Aos from 'aos';
import { IUser } from 'src/app/interfaces/iuser';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {
  constructor(private _userService: UserService, private formBuilder: FormBuilder, private _router: Router) { }
  signUpForm: FormGroup;

  ngOnInit(): void {

    //Loads Animation On Scroll
    Aos.init();

    //Initializes FormGroup
    this.signUpForm = this.formBuilder.group({
      displayName: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      emailId: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(16)]],
      confirmPassword: ['', [Validators.required]]
    },
      {
        validator: this.confirmPasswordMatch('password', 'confirmPassword')
      }
    )
  }

  confirmPasswordMatch(password: string, confirmPassword: string) {
    return (formGroup: FormGroup) => {
      let confirmPasswordControl = formGroup.controls[confirmPassword];
      let passwordControl = formGroup.controls[password];

      if (confirmPasswordControl.errors && !confirmPasswordControl.errors['confirmedValidator']) {
        return;
      }

      if (passwordControl.value !== confirmPasswordControl.value) {
        confirmPasswordControl.setErrors({ confirmedValidator: true })
      }
      else {
        confirmPasswordControl.setErrors(null);
      }

    }
  }

  signUp(form: FormGroup) {
    var signUpformObj = form.value;
    var user: IUser = {
      userId: 0,
      displayName: signUpformObj.displayName,
      emailId: signUpformObj.emailId,
      password: signUpformObj.password,
      dateOfBirth: signUpformObj.dateOfBirth,
      roleId: 2
    };
    console.log(user);
    this._userService.signUp(user).subscribe(
      (responseData) => {
        if ((responseData != null || responseData != undefined) && responseData.roleId != -1) {
          sessionStorage.setItem("RoleId", responseData.roleId.toString());
          sessionStorage.setItem("UserId", responseData.userId.toString());
          console.log("Signup Successful");
          this._router.navigate(['/chat/profile']);
        } else {

          let messageToAlert = "Registration Failed. Reason: ";
          switch (responseData.userId) {
            case -1:
              messageToAlert += "EmailId already Exists."
              break;
            case -2:
              messageToAlert += "Invalid Length of EmailId."
              break;
            case -3:
              messageToAlert += "Invalid Length of Password."
              break;
            case -4:
              messageToAlert += "Invalid Length of DisplayName."
              break;
            case -5:
              messageToAlert += "User should be at least 13 years old."
              break;
              
          }
          alert(messageToAlert);
        }
      },
      (responseError) => { },
      () => { },
    );
  }

}
