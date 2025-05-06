import { NgModule } from '@angular/core';  
import { BrowserModule } from '@angular/platform-browser';  
import { FormsModule } from '@angular/forms';  
import { MatCardModule } from '@angular/material/card';  
import { MatFormFieldModule } from '@angular/material/form-field';  
import { MatInputModule } from '@angular/material/input';  
import { MatButtonModule } from '@angular/material/button';  
import { MatIconModule } from '@angular/material/icon';  
import { MatTableModule } from '@angular/material/table';  
import { MatPaginatorModule } from '@angular/material/paginator';  
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';  
import { JwtHelperService, JWT_OPTIONS } from '@auth0/angular-jwt';  
import { AppRoutingModule } from './app-routing.module';  
import { AppComponent } from './app.component';  
import { AuthService } from './services/auth.service';  
import { ProductService } from './services/product.service';  
import { AuthGuard } from './gaurds/auth.gaurd';  
import { AdminGuard } from './gaurds/admin.gaurd';  
import { HTTP_INTERCEPTORS } from '@angular/common/http';  
import { JwtInterceptor } from './interceptors/jwt.interceptor';  
import { SignalRService } from './services/signalr.service';  
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';  
import { ErrorInterceptor } from './interceptors/error.interceptor';  
import { UserResolver } from './resolvers/user.resolver';  
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';  

@NgModule({  
 declarations: [  
   AppComponent,  
 ],  
 imports: [  
   BrowserModule,  
   FormsModule,  
   MatCardModule, // Ensure MatCardModule is imported here  
   MatFormFieldModule,  
   MatInputModule,  
   MatButtonModule,  
   MatIconModule,  
   MatTableModule,  
   MatPaginatorModule,  
   BrowserAnimationsModule,  
   AppRoutingModule,  
   NgbModule,  
 ],  
 providers: [  
   AuthGuard,  
   AdminGuard,  
   AuthService,  
   ProductService,  
   JwtHelperService,  
   SignalRService,  
   UserResolver,  
   { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },  
   { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },  
   { provide: JWT_OPTIONS, useValue: JWT_OPTIONS }  
 ],  
 schemas: [CUSTOM_ELEMENTS_SCHEMA],  
 bootstrap: [AppComponent]  
})  
export class AppModule { }
