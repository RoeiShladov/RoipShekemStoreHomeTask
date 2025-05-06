// src/app/components/product-list/product-list.component.ts
import { Component, OnInit } from '@angular/core';
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

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})

export class ProductListComponent implements OnInit {
  products: ProductModel[] = [];
  displayedColumns: string[] = ['productName', 'description', 'price', 'quantity', 'imageUrl', 'actions'];
  isAdmin: boolean = false;
  totalProducts: number = 0;
  pageNumber: number = 1;
  pageSize: number = 10;
  user: UserModel | null = null;
  filterText: string = '';
  filterTextSubject: any;
  minPrice: number | undefined;
  maxPrice: number | undefined;
  qty: number | undefined;
  public addOrDelete: string | null = null; // Initialize addOrDelete to null
 

  // Optional: Use a getter if you need additional logic
  getSelectedAction(): string {
    if (this.addOrDelete)
      return this.addOrDelete;
    return '';
  }
  constructor(private productService: ProductService, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {    
    this.authService['currentUser$'].subscribe((user) => {
      this.isAdmin = user?.role === 'Admin';
      this.loadProducts();
    });
  }

  loadProducts(): void {
    const jwt = this.authService.getToken();
    if (jwt) {
      this.productService.getAllProducts(jwt, this.pageNumber, this.pageSize).subscribe(result => {
        if (result.success && result.data) {
          this.products = result.data;
          this.totalProducts = this.products.length; // Calculate total count based on the array length
        } else {
          console.error('Error fetching products:', result.message);
        }
      });
    }
  }

  liveStatusNavigatePage(): void {
    this.router.navigate(['/live-status']);
  }

  removeNavigateNewPage(): void {
    this.addOrDelete = 'delete';
    this.router.navigate(['/add-remove-product']);
  }

  addNavigateNewPage(): void {
    this.addOrDelete = 'add';
      this.router.navigate(['/add-remove-product']);
    }


  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadProducts();
  }

  buyProduct(productName: string): void {
    // Implementation for buying a product
    // You might want to open a dialog to get the quantity
    const jwt = this.authService.getToken();
    if (jwt) {
      if (this.qty) {
        this.productService.buyProduct(jwt, productName, this.qty).subscribe(result => { // Default quantity 1
          if (result.success) {
            this.loadProducts();
          } else {
            console.error('Error buying product:', result.message);
          }
          });
        }
      else {
        this.productService.buyProduct(jwt, productName, 0).subscribe(result => { // Default quantity 1
          if (result.success) {
            this.loadProducts();
          } else {
            console.error('Error buying product:', result.message);
          }
        });
      }            
    }
  }

  searchProducts(filterText: string): void {
    const jwt = this.authService.getToken();
    if (jwt) {

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
