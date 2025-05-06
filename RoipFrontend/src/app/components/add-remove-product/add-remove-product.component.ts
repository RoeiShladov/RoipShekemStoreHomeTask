import { Component } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { ProductModel } from '../../models/product.model';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ProductListComponent } from '../product-list/product-list.component'; // Import ProductListComponent

@Component({
  selector: 'app-add-product',
  templateUrl: './add-remove-product.component.html',
  styleUrls: ['./add-remove-product.component.scss']
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
  constructor(
    private productService: ProductService,
    private router: Router,
    private authService: AuthService,
    private productListComponent: ProductListComponent // Inject ProductListComponent
  ) { }


  ngOnInit(): void {

    this.action = this.productListComponent.getSelectedAction();
      
    if (this.action === 'add') {
    if (this.addProductForm) this.addProductForm.style.display = 'block';
    if (this.removeProductForm) this.removeProductForm.style.display = 'none';
    }
    else
      if (this.action === 'delete') {
      if (this.addProductForm) this.addProductForm.style.display = 'none';
      if (this.removeProductForm) this.removeProductForm.style.display = 'block';
    }
      else
      {
      console.error('Invalid action mode');
      if (this.addProductForm) this.addProductForm.style.display = 'none';
      if (this.removeProductForm) this.removeProductForm.style.display = 'none';
    }    
  }

  addProduct(): void {
    const jwt = this.authService.getToken();
    if (jwt) {
      this.productService.addProduct(jwt, this.newProduct).subscribe(result => {
        if (result.success) {
          alert('Product successfully added!');
          this.router.navigate(['/products']);
        } else {
          console.error('Error adding product:', result.message);
        }
      });
    }
  }

  deleteProduct(productName: string): void {
    const jwt = this.authService.getToken();
    if (jwt) {
      this.productService.deleteProduct(jwt, productName).subscribe(result => {
        if (result.success) {
          alert('Product successfully deleted!');
          this.router.navigate(['/products']);
        } else {
          console.error('Error deleting product:', result.message);
        }
      });
    }
  }
}
