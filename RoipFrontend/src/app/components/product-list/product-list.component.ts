// src/app/components/product-list/product-list.component.ts
import { Component, NgModule, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { ProductModel } from '../../models/product.model';
import { AuthService } from '../../services/auth.service';
import { PageEvent } from '@angular/material/paginator';
import { SearchFilterComponent } from '../../shared/components/search-filter/search-filter.component';
import { UserResolver } from '../../resolvers/user.resolver';
import { UserModel } from '../../models/user.model';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, map, tap } from 'rxjs';
import { MatOptionModule } from '@angular/material/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSelectModule } from '@angular/material/select';
import { SignalRUserModel } from '../../models/signalr-user.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-list',
  standalone: true,
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
  imports: [MatSelectModule, MatOptionModule, MatCardModule, CommonModule, FormsModule, MatIconModule, BrowserAnimationsModule, MatButtonModule, MatPaginatorModule, MatTableModule, MatInputModule, MatFormFieldModule],
})

export class ProductListComponent implements OnInit {
  products: ProductModel[] = [];
  displayedColumns: string[] = ['productName', 'description', 'price', 'quantity', 'imageUrl', 'actions'];
  isAdmin: boolean = false;
  totalProducts: number = 0;
  pageNumber: number = 1;
  pageSize: number = 10;
  filterTextSubject: any;
  minPrice: number | undefined;
  maxPrice: number | undefined;
  qty: number = 0;
  jwt: string | null = null; // Initialize jwt to null
  quantityOptions: number[] = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
  filterText: string = ''; // Initialize filterText to an empty string
  public addOrDelete: string | null = null; // Initialize addOrDelete to null
  user!: SignalRUserModel;


  // Optional: Use a getter if you need additional logic
  getSelectedAction(): string {
    if (this.addOrDelete)
      return this.addOrDelete;
    return '';
  }
  constructor(private productService: ProductService, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.authService['currentUser$'].subscribe((user) => {
        if (user) {
          this.isAdmin = user?.role === 'Admin';
          this.loadProducts();
          this.user.email = user?.email;
          this.user.name = user?.name;
          this.user.role = user?.role;
        }              
      })
    }
    else {
      this.authService.logout()      
      }
    }

  loadProducts(): void {
    const jwt = this.authService.getToken();
    if (jwt) {
      this.productService.getAllProducts(jwt, this.pageNumber, this.pageSize).subscribe(result => {
        if (result.success && result.data) {
          this.products = result.data;
          this.totalProducts = this.products.length; // Calculate total count based on the array length
        } else {
          alert('Error fetching products: ' + result.message);
          console.error('Error fetching products:', result.message);
        }
      });
    }
  }

  liveStatusNavigatePage(): void {
    this.router.navigate(['live-status']);
  }

  removeNavigateNewPage(): void {
    this.addOrDelete = 'delete';
    this.router.navigate(['add-remove-product']);
  }

  addNavigateNewPage(): void {
    this.addOrDelete = 'add';
      this.router.navigate(['add-remove-product']);
    }

  onSearchChanged(): void {
    this.jwt = this.authService.getToken()
    if (this.jwt) {
      this.productService.searchProduct(this.jwt, this.filterText, this.minPrice, this.maxPrice).subscribe(result => {
        if (result.success && result.data) {
          this.products = result.data;
        } else {
          alert('Error searching products: ' + result.message);
          console.error('Error searching products:', result.message);
        }
      })
    } else {
      alert('JWT token is null or empty');
      console.error('JWT token is null or empty');
      this.authService.logout();
    }
  }

  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadProducts();
  }

  onPriceChange(minPrice: number | undefined, maxPrice: number | undefined): void {
    this.minPrice = minPrice;
    this.maxPrice = maxPrice;
    this.searchProducts(this.filterText);
  }

  buyProduct(productName: string): void {
    // Implementation for buying a product
    // You might want to open a dialog to get the quantity
    const jwt = this.authService.getToken();
    if (jwt) {
      if (this.qty) {
        this.productService.buyProduct(jwt, productName, this.qty).subscribe(result => { // Default quantity 1
          if (!result.success) {
            alert('buying product failed: ' + result.message);
            console.error('buying product failed: ', result.message);            
          }
          this.loadProducts();
          });
        }
      else {
        this.productService.buyProduct(jwt, productName, 0).subscribe(result => { // Default quantity 1
          if (!result.success) {
            alert('buying product failed: ' + result.message);
            console.error('buying product failed: ', result.message);
          }
          this.loadProducts();       
        });
      }            
    }
  }

  searchProducts(filterText: string): void {
    const jwt = this.authService.getToken();
    if (jwt) {

      // Perform filtering on current products list, but we want to get the newest updated products in store
      //, so let's call the API to get the newest products.
      //this.products = this.products.filter(product => {
      //  const matchesFilterText = product.name.toLowerCase().includes(filterText.toLowerCase());
      //  const matchesMinPrice = this.minPrice === undefined || product.price >= this.minPrice;
      //  const matchesMaxPrice = this.maxPrice === undefined || product.price <= this.maxPrice;
      //  return matchesFilterText && matchesMinPrice && matchesMaxPrice;
      //});

      this.productService.searchProduct(jwt, filterText, this.minPrice, this.maxPrice).subscribe(result => {
        if (result.success && result.data) {
          this.products = result.data;
        } else {
          console.error('Error searching products:', result.message);
        }
      });
    }
  }
}
