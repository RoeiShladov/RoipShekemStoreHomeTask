// src/app/core/services/product.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductModel } from '../models/product.model';
import { ServiceResult } from '../models/service-result.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly apiUrl = '/api/products';

  constructor(private http: HttpClient) { }

  getAllProducts(jwt: string, pageNumber: number, pageSize: number): Observable<ServiceResult<ProductModel[]>> {
    let params = new HttpParams()
      .set('jwt', jwt)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<ServiceResult<ProductModel[]>>(`${this.apiUrl}/get-all-products`, { params: { jwt } });
  }

  addProduct(jwt: string, product: ProductModel): Observable<ServiceResult<string>> {
    return this.http.post<ServiceResult<string>>(`${this.apiUrl}/add-product`, { params: { jwt, product } });
  }

  deleteProduct(jwt: string, productName: string): Observable<ServiceResult<ProductModel>> {
    return this.http.delete<ServiceResult<ProductModel>>(`${this.apiUrl}/delete-product`, { params: { jwt, productName } });
  }

  buyProduct(jwt: string, productName: string, quantity: number): Observable<ServiceResult<ProductModel>> {
    return this.http.post<ServiceResult<ProductModel>>(`${this.apiUrl}/buy-product`, { params: { jwt, productName, quantity } });
  }

  searchProduct(jwt: string, filterText: string, minPrice?: number, maxPrice?: number): Observable<ServiceResult<ProductModel[]>> {
    let params = new HttpParams().set('jwt', jwt).set('filterText', filterText);
    if (minPrice) params = params.set('minPrice', minPrice.toString());
    if (maxPrice) params = params.set('maxPrice', maxPrice.toString());    
    return this.http.get<ServiceResult<ProductModel[]>>(`${this.apiUrl}/search-filter`, { params });
  }
}
