import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { AuthService } from '../../services/auth.service';
import { SignalRService } from '../../services/signalr.service';
import { SignalRUserModel } from '../../models/signalr-user.model'; // Adjust the import path as necessary
import { SearchFilterComponent } from '../../shared/components/search-filter/search-filter.component'; // Adjust the import path as necessary
import { BehaviorSubject } from 'rxjs';

  @Component({
    selector: 'app-live-status',
    templateUrl: './live-status.component.html',
    styleUrls: ['./live-status.component.scss']
  })
  export class LiveStatusComponent implements OnInit, OnDestroy {
    private connectedUsersSubject = new BehaviorSubject<{ [key: string]: SignalRUserModel }>({});
    connectedUsers$ = this.connectedUsersSubject.asObservable();
    private hubUrl: string = 'https://localhost/user-connection-hub';

    constructor(private signalRService: SignalRService) {}

    ngOnInit(): void {
      this.signalRService.startConnection(this.hubUrl).then(() => {
        this.signalRService.hubConnection?.on('UpdateLiveConnectedUsers', (users: { [key: string]: SignalRUserModel }) => {
          this.connectedUsersSubject.next(users);
        });
      }).catch(err => console.error('Error starting SignalR connection:', err));
    }

    ngOnDestroy(): void {
      this.signalRService.stopConnection().catch(err => console.error('Error stopping SignalR connection:', err));
      this.connectedUsersSubject.complete(); // Complete the subject to clean up resources
    }
  }
