import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor() { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.error instanceof ErrorEvent) {
          // Client-side errors
          const errorMessage = `Client Error: ${error.error.message}`;
          this.openErrorWindow(errorMessage);
        } else {
          // Server-side errors
          const errorMessage = `Server Error, The Error Code: ${error.status}\nMessage: ${error.error.message || error.message}`;
          this.openErrorWindow(errorMessage);
        }
        return throwError(error);
      })
    );
  }

  private openErrorWindow(message: string): void {
    const errorWindow = window.open('', '_blank', 'width=600,height=400');
    if (errorWindow) {
      errorWindow.document.write(`
        <html>
          <head>
            <title>Error</title>
            <style>
              body {
                font-family: Arial, sans-serif;
                margin: 20px;
                padding: 0;
                background-color: #f8d7da;
                color: #721c24;
              }
              h1 {
                font-size: 24px;
              }
              p {
                font-size: 16px;
              }
            </style>
          </head>
          <body>
            <h1>An Error Occurred</h1>
            <p>${message}</p>
          </body>
        </html>
      `);
      errorWindow.document.close();
    }
  }
}
