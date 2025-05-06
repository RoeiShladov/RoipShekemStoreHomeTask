// src/app/core/models/product.model.ts
export interface ProductModel {
  id: number;
  name: string;
  description?: string; // Optional
  price: number;
  quantity: number;
  imageUrl?: string; // Optional
  rowVersion: string; // Data type appropriate for Timestamp (possibly a Base64 string)
}
