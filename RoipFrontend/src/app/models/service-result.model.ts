// src/app/core/models/service-result.model.ts
export interface ServiceResult<T> {
  success: boolean;
  message: string;
  data: T | null;
  statusCode: number;
  error: string | null;
  roipShekemStoreJWT: string; // Optional
}
