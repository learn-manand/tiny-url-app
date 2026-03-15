import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UrlItem } from '../models/url-item.model';
import { CreateUrlRequest } from '../models/create-url-request.model';
import { UrlService } from '../services/url.service';

@Injectable({
  providedIn: 'root'
})
export class UrlState {

  private urlsSubject = new BehaviorSubject<UrlItem[]>([]);
  urls$ = this.urlsSubject.asObservable();

  constructor(private service: UrlService) {}

  load(){
    this.service.getPublicUrls()
      .subscribe(data => this.urlsSubject.next(data));
  }

  create(request: CreateUrlRequest){
    this.service.create(request)
      .subscribe(() => this.load());
  }

  search(term: string){
    this.service.search(term)
      .subscribe(data => this.urlsSubject.next(data));
  }

  delete(id: number){
    this.service.delete(id)
      .subscribe(() => this.load());
  }
}