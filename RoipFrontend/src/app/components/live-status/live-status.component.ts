import { Component, OnInit, OnDestroy } from '@angular/core';
import { SignalRService } from '../../services/signalr.service';
import { SignalRUserModel } from '../../models/signalr-user.model';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SearchFilterComponent } from '../../shared/components/search-filter/search-filter.component';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-live-status',
  standalone: true,
  templateUrl: './live-status.component.html',
  styleUrls: ['./live-status.component.scss'],
  imports: [SearchFilterComponent, NgIf, MatCardModule, CommonModule, FormsModule, MatIconModule, BrowserAnimationsModule, MatButtonModule, MatPaginatorModule, MatTableModule, MatInputModule, MatFormFieldModule],
})


export class LiveStatusComponent implements OnInit, OnDestroy {
  connectedUsers: SignalRUserModel[] = []; // Change to an array
  private connectedUsersSubscription: Subscription | undefined;
  isAdmin = false;

  constructor(private router: Router,
private signalRService: SignalRService, private authService: AuthService) { }

  ngOnInit(): void {
      this.authService.currentUser$.subscribe(user => {
      this.isAdmin = user?.role === 'Admin';
    });

    if (this.isAdmin) {
      this.signalRService.startConnection('https://localhost/user-connection-hub')
        .then(() => {
          this.connectedUsersSubscription = this.signalRService.connectedUsers$.subscribe(users => {
            // Convert the object to an array
            this.connectedUsers = Object.values(users);
          });
        })
        .catch(err => console.error('Error starting SignalR connection:', err));
    }
    else {
      this.router.navigate(['products']);
    }
  }

  ngOnDestroy(): void {
    if (this.connectedUsersSubscription) {
      this.connectedUsersSubscription.unsubscribe();
    }
    this.signalRService.stopConnection().catch(err => console.error('Error stopping SignalR connection:', err));
  }

  onSearchChanged(filterText: string): void {
    // Implement your search logic here if needed
    // This example just logs the filter text
    console.log('Search text:', filterText);
  }
}
