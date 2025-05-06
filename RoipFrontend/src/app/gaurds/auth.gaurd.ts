import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {

    constructor(private authService: AuthService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (this.authService.isAuthenticated()) {
            return true;
        } else {
          this.router.navigate(['/login']); //, { queryParams: { returnUrl: state.url } }); 
            return false;
        }
    }
}

//if not authenticated(anonymous user) nevigate to login page, 'state.url' is the page the user was trying to access,
//send it to the login page so that after login, user can be redirected to the page he was trying to access.
