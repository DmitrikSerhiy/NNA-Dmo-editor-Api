import { UserManager } from './user-manager';
import { Observable } from 'rxjs';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivateChild } from '@angular/router';

export class AuthGuard implements CanActivateChild {

    constructor(private userManager: UserManager) { }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | boolean {
        return this.userManager.isAuthorized();
    }
}
