import { Component, NgModule, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { LoginCredentialsModel } from '../../models/login-credentials.model';
import { UserModel } from '../../models/user.model'; // Import the model
import { ServiceResult } from '../../models/service-result.model';
import { SignalRService } from '../../services/signalr.service'; // Import the SignalR service
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from '../../app-routing.module'; // Import the routing module
import { AppComponent } from '../../app.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [BrowserModule, AppRoutingModule, CommonModule, FormsModule, MatIconModule, BrowserAnimationsModule, MatButtonModule, MatPaginatorModule, MatTableModule, MatInputModule, MatFormFieldModule],
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
          alert('User is already logged in. Redirecting to products page...'); // Debugging line
          console.log('User is already logged in. Redirecting to products page...'); // Debugging line
          this.router.navigate(['products']);
        }
      });
      console.log('User is not logged in'); // Debugging line
    }
  }

  registerPage(): void {
    this.router.navigate(['register']);
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
            this.router.navigate(['products']);
          } else {
            this.errorMessage = user.message || 'Login failed. Please check your credentials.';
          }
          this.router.navigate(['products']);
        },
        error: (error: { message: string; }) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'Login failed. Please check your credentials.';
          console.error('Login error:', error);
          alert('Login failed. Please check your credentials.');
        }
      });
    }
  }
}
