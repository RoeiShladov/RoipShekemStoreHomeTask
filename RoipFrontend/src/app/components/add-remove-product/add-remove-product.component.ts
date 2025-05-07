import { Component, NgModule } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { ProductModel } from '../../models/product.model';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ProductListComponent } from '../product-list/product-list.component'; // Import ProductListComponent
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SignalRUserModel } from '../../models/signalr-user.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-product',
  standalone: true,
  templateUrl: './add-remove-product.component.html',
  styleUrls: ['./add-remove-product.component.scss'],
  imports: [MatCardModule, CommonModule, FormsModule, MatIconModule, BrowserAnimationsModule, MatButtonModule, MatPaginatorModule, MatTableModule, MatInputModule, MatFormFieldModule],
})

export class AddProductComponent {
  newProduct: ProductModel = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    quantity: 0,
    imageUrl: '',
    rowVersion: ''
  };
  action: string | null = null;
  addProductForm = document.getElementById('addProductForm');
  removeProductForm = document.getElementById('removeProductForm');
  user!: SignalRUserModel;
  productName!: string;
  constructor(
    private productService: ProductService,
    private router: Router,
    private authService: AuthService,
    private productListComponent: ProductListComponent // Inject ProductListComponent
  ) { }


  ngOnInit(): void {
    if (this.authService.isAuthenticated() && this.authService.isAdmin()) {

        this.authService['currentUser$'].subscribe((user) => {
          if (user) {
            this.user.email = user.email;
            this.user.name = user.name;
            this.user.role = user.role;
          }
        });

        this.action = this.productListComponent.getSelectedAction();
      //if user pressed add button, show add product form and hide remove product form
        if (this.action === 'add') {
          if (this.addProductForm) this.addProductForm.style.display = 'block';
          if (this.removeProductForm) this.removeProductForm.style.display = 'none';
        }
        else
          //if user pressed remove button, show remove product form and hide add product form
          if (this.action === 'delete') {
            if (this.addProductForm) this.addProductForm.style.display = 'none';
            if (this.removeProductForm) this.removeProductForm.style.display = 'block';
          }
          else {
            alert('Invalid action mode');
            console.error('Invalid action mode');
            this.router.navigate(['products']);
          }
      }
      else {
      this.authService.logout()
      this.router.navigate(['products']);

      }    
  }

  addProduct(): void {
    const jwt = this.authService.getToken();
    if (jwt) {
      this.productService.addProduct(jwt, this.newProduct).subscribe(result => {
        if (result.success) {
          alert('Product successfully added!');
          this.router.navigate(['products']);
        } else {
          alert('Error adding product: ' + result.message);
          console.error('Error adding product:', result.message);
        }
      });
    }
  }

  deleteProduct(): void {
    const jwt = this.authService.getToken();
    if (jwt) {
      this.productService.deleteProduct(jwt, this.productName).subscribe(result => {
        if (result.success) {
          alert('Product successfully deleted!');
          this.router.navigate(['products']);
        } else {
          alert('Error deleting product: ' + result.message);
          console.error('Error deleting product:', result.message);
        }
      });
    }
  }
}
