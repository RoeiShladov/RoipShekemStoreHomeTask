import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, map, tap } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt'; // Install: npm install @auth0/angular-jwt
import { LoginCredentialsModel } from '../models/login-credentials.model';
import { ServiceResult } from '../models/service-result.model';
import { environment } from '../../enviroments/enviroments';
import { UserModel } from '../models/user.model'; // Import the UserModel
import { Router } from '@angular/router'; // Import Router }

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl + '/users';  
  private readonly tokenKey = 'roipShekemStoreJWT'; // Key for localStorage
  private readonly currentUserSubject = new BehaviorSubject<UserModel | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  public isLoggedIn$ = this.currentUser$.pipe(map(user => !!user));

  constructor(
    private http: HttpClient,
    private jwtHelper: JwtHelperService,
    private router: Router // Inject Router
  ) { }

  login(credentials: LoginCredentialsModel): Observable<ServiceResult<UserModel>> {
    return this.http.post<ServiceResult<UserModel>>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => {
          if (response.success && response.data && response.roipShekemStoreJWT) {
            this.setToken(response.roipShekemStoreJWT);
            this.setCurrentUser(response.data);
          } else {
            this.removeToken();
            this.currentUserSubject.next(null); // Clear user
          }
        })
      );
  }

  logout(): void {
    this.removeToken();
    this.currentUserSubject.next(null);
    this.router.navigate(['login']);
  }

  getToken(): string | null{
    return localStorage.getItem(this.tokenKey);
  }

  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  removeToken(): void {
    localStorage.removeItem(this.tokenKey);
  }

  getCurrentUser(): Observable<UserModel | null> {
    return this.currentUser$;
  }

  isAdmin(): boolean {
    const user = this.currentUserSubject.getValue();
    return user ? user.role === 'Admin' : false;
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired();
  }

  setCurrentUser(user: UserModel): void {
    this.currentUserSubject.next(user);
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) {
      return true;
    }
    return this.jwtHelper.isTokenExpired(token);
  }
}
