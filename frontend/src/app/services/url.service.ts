import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UrlItem } from '../models/url-item.model';
import { environment } from '../../environments/environment';
import { CreateUrlRequest } from '../models/create-url-request.model';

@Injectable({
  providedIn: 'root'
})
export class UrlService {

  private baseUrl = environment.apiUrl;

  private readonly endpoints = {
    add: '/api/add',
    public: '/api/public',
    delete: '/api/delete'
  } as const;

  constructor(private http: HttpClient) {}

  create(request: CreateUrlRequest){
    return this.http.post<UrlItem>(`${this.baseUrl}${this.endpoints.add}`, request);
  }

  getPublicUrls(){
    return this.http.get<UrlItem[]>(`${this.baseUrl}${this.endpoints.public}`);
  }

  delete(code: string){
    return this.http.delete(`${this.baseUrl}${this.endpoints.delete}/${code}`);
  }
}