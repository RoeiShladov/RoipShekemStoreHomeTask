import { Component, NgModule, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
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

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  imports: [MatCardModule, CommonModule, FormsModule, MatIconModule, BrowserAnimationsModule, MatButtonModule, MatPaginatorModule, MatTableModule, MatInputModule, MatFormFieldModule],
})
export class RegisterComponent implements OnInit {
  newUser = {
    name: '',
    role: '',
    email: '',
    password: '',
    phoneNumber: '',
    address: ''
  };
  errorMessage = '';

  constructor(private userService: UserService, private router: Router) { }

  ngOnInit(): void {
  }

  register(): void {
    this.userService.register(this.newUser).subscribe({
      next: (response) => {
        console.log('Registration successful', response);
        // redirect to login
        alert('Registration successful! Please log in.');
        this.router.navigate(['login']);
      },
      error: (error) => {
        alert('Registration failed. Please try again.');
        console.error('Registration failed', error);
        this.errorMessage = error.error.message || 'Registration failed. Please try again.';
      }
    });
  }
}
