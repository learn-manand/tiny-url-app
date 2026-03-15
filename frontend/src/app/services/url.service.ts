import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UrlItem } from '../models/url-item.model';
import { environment } from '../../environments/environment';
import { CreateUrlRequest } from '../models/create-url-request.model';

@Injectable({
  providedIn: 'root'
})
export class UrlService {

  private baseUrl = `${environment.apiUrl}/urls`;

  constructor(private http: HttpClient) {}

  create(request: CreateUrlRequest){
    return this.http.post<UrlItem>(this.baseUrl, request);
  }

  getPublicUrls(){
    return this.http.get<UrlItem[]>(`${this.baseUrl}/public`);
  }

  search(searchTerm: string){
    return this.http.get<UrlItem[]>(
      `${this.baseUrl}/search?term=${searchTerm}`
    );
  }

  delete(id: number){
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}