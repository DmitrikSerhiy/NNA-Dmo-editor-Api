import { UserManager } from './../shared/user-manager';
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
    private authService: AuthService,
    private userManager: UserManager
  ) {}

  ngOnInit() {
  }

  authorize(email, password) {
    this.authService.authorize(email, password)
      .subscribe((response) => {
        this.userManager.login(response.accessToken, response.email, response.userName);
      });
  }

}
