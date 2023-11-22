import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { Router, NavigationEnd } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  invalidLoginMessage: string = "Invalid Email or Password";
  invalidLogin: boolean;
  constructor(private _router: Router, private formBuilder: FormBuilder, private _userService: UserService) { }
  ngOnInit() {
    this.invalidLogin = false;
    this.loginForm = this.formBuilder.group({
      emailId: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(16)]]
    });
  }
  login(formGroup: FormGroup) {
    var emailId = formGroup.value.emailId;
    var password = formGroup.value.password;

    this._userService.login(emailId, password).subscribe(
      (responseData) => {
        if ((responseData != null || responseData != undefined) && (responseData.roleId != -1 || responseData.userId != -1 )) {
          sessionStorage.setItem("RoleId", responseData.roleId.toString());
          sessionStorage.setItem("UserId", responseData.userId.toString());
          console.log("Login Successful");

          if (responseData.roleId == 1) {
            this._router.navigate(['/chat/messages']);
          } else if (responseData.roleId == 2) {
            this._router.navigate(['/admin']);
          }
        }
        else {
          console.log("Invalid Username or Password")
          this.invalidLogin = true;
          this.invalidLoginMessage = "Login Failed. Reason: Invalid Username or Password"
        }


      },
      (responseError) => {
        this.invalidLogin = true;
        this.invalidLoginMessage = `Login Failed. Reason: ${"Status Code: " + (<string>responseError).split(" ")[5]}`;
        console.log(responseError)
      },
      () => { }
    );
  }
}
