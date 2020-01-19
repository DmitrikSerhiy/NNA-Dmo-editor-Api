import { AuthService } from './../shared/services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { routerTransition } from '../router.animations';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  animations: [routerTransition()]
})
export class LoginComponent implements OnInit {
  constructor(
    public router: Router,
    private authService: AuthService
  ) {}

  ngOnInit() {
  }

  authorize(email, password) {
    this.authService.authorize(email, password)
      .subscribe((response) => {
        console.log('Successful authentication');
        localStorage.setItem('user access token', response.accessToken);
        localStorage.setItem('user email', response.email);
        localStorage.setItem('user name', response.userName);
        this.router.navigateByUrl('/');
      });
  }
}
