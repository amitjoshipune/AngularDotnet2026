import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Item, ItemCreateRequest, ItemUpdateRequest } from '../models/item.models';

@Injectable({ providedIn: 'root' })
export class ItemService {
  constructor(private readonly http: HttpClient) {}

  getItems(search = ''): Observable<Item[]> {
    let params = new HttpParams();
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<Item[]>(`${environment.apiUrl}/items`, { params });
  }

  getItem(id: number): Observable<Item> {
    return this.http.get<Item>(`${environment.apiUrl}/items/${id}`);
  }

  createItem(payload: ItemCreateRequest): Observable<Item> {
    return this.http.post<Item>(`${environment.apiUrl}/items`, payload);
  }

  updateItem(payload: ItemUpdateRequest): Observable<Item> {
    return this.http.put<Item>(`${environment.apiUrl}/items/${payload.id}`, payload);
  }

  deleteItem(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${environment.apiUrl}/items/${id}`);
  }
}
