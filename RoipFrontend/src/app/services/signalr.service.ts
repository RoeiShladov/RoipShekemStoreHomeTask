import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { UserModel } from '../models/user.model'; // Adjust the import path as necessary }
import { SignalRUserModel } from '../models/signalr-user.model'; // Adjust the import path as necessary

export interface ConnectedUsers { //define the type
  [key: string]: SignalRUserModel;
}

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  public hubConnection: HubConnection | undefined;
  private connectedUsersSubject = new Subject<ConnectedUsers>();  // Use the interface
  public connectedUsers$ = this.connectedUsersSubject.asObservable();


  sendData(methodName: string, data: UserModel): Promise<void> {
    if (this.hubConnection) {
      return this.hubConnection.invoke(methodName, data.email, data.name, data.role)
        .then(() => console.log(`Data sent successfully using method: ${methodName}`))
        .catch((err: Error) => {
          console.error(`Error sending data using method ${methodName}:`, err);
          throw err;
        });
    } else {
      console.error('Hub connection is not established.');
      return Promise.reject('Hub connection is not established.');
    }
  }

  getConnectedUsers(): Subject<any> {
    return this.connectedUsersSubject;
  }

  startConnection(hubUrl: string): Promise<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(hubUrl)
      .build();
    // Send data to the hub
    
    if (this.hubConnection) {
      this.hubConnection.on('UpdateLiveConnectedUsers', (users: ConnectedUsers) => {
        console.log('Connected users updated:', users);
        this.connectedUsersSubject.next(users);                   
      });
    }

    const start = (): Promise<void> => {
      return this.hubConnection!.start()
        .then(() => console.log('Hub connection started'))
        .catch((err: Error) => {
          console.error('Error while starting hub connection:', err);
          console.log('Retrying connection in 3 seconds...');
          return new Promise((resolve) => setTimeout(resolve, 3000)).then(start);
        });
    };
    return start();
  }
  
  stopConnection(): Promise<void> {

    const stop = (): Promise<void> => {
      return this.hubConnection!.start()
        .then(() => console.log('Hub connection stopped'))
        .catch((err: Error) => {
          console.error('Error while stopping hub connection:', err);
          console.log('Retrying disconnection in 3 seconds...');
          return new Promise((resolve) => setTimeout(resolve, 3000)).then(stop);
        });
    };
    return stop();
  }
  
  //When user closes the browser or navigates away,
  //this will automatically call the OnDisconnectedAsync method in the hub
  onDisconnected(): void {
    this.hubConnection?.off('OnDisconnectedAsync', () => {
      console.log('Disconnected from hub');
    });
  }

  //When the user connects to the hub, this will automatically
  //call the OnConnectedAsync method in the hub
  onConnected(): void {
    this.hubConnection?.on('OnConnectedAsync', () => {
      console.log('Connected to hub');
    });
  }
}
