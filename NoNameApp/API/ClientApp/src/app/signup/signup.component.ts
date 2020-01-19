import { AuthService } from './../shared/services/auth.service';
import { Component, OnInit } from '@angular/core';
import { routerTransition } from '../router.animations';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss'],
  animations: [routerTransition()]
})
export class SignupComponent implements OnInit {

  constructor(
    public router: Router,
    private authService: AuthService
  ) { }

  ngOnInit() {
  }

  signUp(userName, email, password) {
    this.authService.register(userName, email, password)
    .subscribe((response) => {
      console.log('Successful registration');
      localStorage.setItem('user access token', response.accessToken);
      localStorage.setItem('user email', response.email);
      localStorage.setItem('user name', response.userName);
      this.router.navigateByUrl('/');
    });
  }
}
