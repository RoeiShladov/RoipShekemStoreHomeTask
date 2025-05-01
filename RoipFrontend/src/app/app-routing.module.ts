import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [{ path: 'login', component: LoginComponent },
  { path: 'products', component: ProductListComponent },
  { path: 'admin/products', component: AdminProductComponent, canActivate: [AdminGuard] },
  { path: 'admin/status', component: ConnectionStatusComponent, canActivate: [AdminGuard] }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})


export class AppRoutingModule { }
