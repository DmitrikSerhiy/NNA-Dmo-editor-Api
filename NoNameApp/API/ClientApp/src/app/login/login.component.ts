import { UserManager } from './../shared/user-manager';
import { AuthService } from './../shared/services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { routerTransition } from '../router.animations';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  animations: [routerTransition()]
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  constructor(
    public router: Router,
    private authService: AuthService,
    private userManager: UserManager) {

    this.loginForm = new FormGroup({
      'email': new FormControl('', [Validators.required, Validators.email]),
      'password': new FormControl('', [Validators.required, Validators.maxLength(8)])
    });
  }

  ngOnInit() {
  }

  submit(email, password) {
    this.authService.authorize(email, password)
      .subscribe((response) => {
        this.userManager.login(response.accessToken, response.email, response.userName);
      });
  }

}
