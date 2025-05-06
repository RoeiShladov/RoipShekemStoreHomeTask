// src/app/core/models/user.model.ts
export interface UserModel {
  name: string;
  email: string;
  password?: string; // Password is often write-only, not received back
  phoneNumber: string;
  address: string;
  role: string;     // May be returned from the server
}
