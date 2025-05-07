import { Injectable } from '@angular/core';
import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { UserModel } from '../models/user.model'; // Make sure UserModel is correctly imported
import { ServiceResult } from '../models/service-result.model'; // Make sure ServiceResult is correctly imported

@Injectable({
  providedIn: 'root'
})
export class UserResolver implements Resolve<ServiceResult<UserModel> | null> {
  constructor(private authService: AuthService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<ServiceResult<UserModel> | null> {
    return this.authService.getCurrentUser().pipe(
      take(1),
      map(user => {
        const token = this.authService.getToken(); // Get token here
        if (user) {
          const result: ServiceResult<UserModel> = {
            success: true,
            data: user,
            message: 'User data retrieved successfully', 
            statusCode: 200,           
            error: null,                  
            roipShekemStoreJWT: token ?? ''
          };
          return result;
        } else {
          this.router.navigate(['login']);
          const result: ServiceResult<UserModel> = {
            success: false,
            data: null,
            message: 'User not authenticated', 
            statusCode: 401,             
            error: 'Not Authenticated',    
            roipShekemStoreJWT: ''   
          };
          return result;
        }
      }),
      catchError(error => {
        console.error('Error fetching user data:', error);
        const result: ServiceResult<UserModel> = { // Consistent return type
          success: false,
          data: null,
          message: 'Error fetching user data',
          statusCode: 500, 
          error: error.message || 'Server error',
          roipShekemStoreJWT: ''
        };
        return of(result);
      })
    );
  }
}
