import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
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
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('Registration failed', error);
        this.errorMessage = error.error.message || 'Registration failed. Please try again.';
      }
    });
  }
}
