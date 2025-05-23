import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './components/product-list/product-list.component';
import { AddProductComponent } from './components/add-remove-product/add-remove-product.component';
import { LiveStatusComponent } from './components/live-status/live-status.component';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { AuthGuard } from '../app/gaurds/auth.gaurd';
import { AdminGuard } from '../app/gaurds/admin.gaurd';
import { UserResolver } from './resolvers/user.resolver';
import { MatIconModule } from '@angular/material/icon';  
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';


const routes: Routes = [
 { path: 'health-check', component: LoginComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'products-list', component: ProductListComponent, canActivate: [AuthGuard, AdminGuard], resolve: { user: UserResolver }  },
  { path: 'add-remove-product', component: AddProductComponent, canActivate: [AdminGuard], resolve: { user: UserResolver } },
  { path: 'live-status', component: LiveStatusComponent, canActivate: [AdminGuard], resolve: { user: UserResolver } }, 
  { path: '', redirectTo: '/login', pathMatch: 'full' }, // Default route
  { path: '**', redirectTo: '' }  // Default route for unknown routes
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
