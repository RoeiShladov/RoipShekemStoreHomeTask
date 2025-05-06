import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { LoginCredentialsModel } from '../../models/login-credentials.model';
import { UserModel } from '../../models/user.model'; // Import the model
import { ServiceResult } from '../../models/service-result.model';
import { SignalRService } from '../../services/signalr.service'; // Import the SignalR service

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  errorMessage = '';
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private signalRService: SignalRService // Inject the SignalR service
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn$) {
      this.authService.isLoggedIn$.subscribe((isLoggedIn: any) => {
        if (isLoggedIn) {
          this.router.navigate(['/products']);
        }
      });
    }
  }

  login(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      const credentials: LoginCredentialsModel = this.loginForm.value;

      this.authService.login(credentials).subscribe({
        next: (user: ServiceResult<UserModel>) => { // Type the user
          this.isLoading = false;
          if (user.success) {
            this.authService.setToken(user.roipShekemStoreJWT);
            if (user.data) {  // Add this check!
              this.authService.setCurrentUser(user.data);
              this.signalRService.sendData("FetchAuthenticatedUser", user.data);
            }
            this.router.navigate(['/products']);
          } else {
            this.errorMessage = user.message || 'Login failed. Please check your credentials.';
          }
          this.router.navigate(['/products']);
        },
        error: (error: { message: string; }) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'Login failed. Please check your credentials.';
          console.error('Login error:', error);
        }
      });
    }
  }
}
