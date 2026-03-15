import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { UrlState } from '../../state/url.state';
import { UrlItem } from '../../models/url-item.model';
import { CreateUrlRequest } from '../../models/create-url-request.model';

@Component({
  selector: 'app-url-manager',
  templateUrl: './url-manager.component.html',
  styleUrls: ['./url-manager.component.css'],
  standalone: false
})
export class UrlManagerComponent implements OnInit {

  urls$!: Observable<UrlItem[]>;

  url = '';
  isPrivate = false;
  searchTerm = '';
  copiedId: number | null = null;


  constructor(private state: UrlState) {}


  ngOnInit(): void {
    this.urls$ = this.state.urls$;
    this.state.load();
  }

  create(){
    const request: CreateUrlRequest = {
      originalUrl: this.url,
      isPrivate: this.isPrivate
    };

    this.state.create(request);

    this.url = '';
    this.isPrivate = false;
  }

  search(){
    this.state.search(this.searchTerm);
  }

  delete(id: number){
    this.state.delete(id);
  }

  copy(url: string, id: number){
    navigator.clipboard.writeText(url);
    this.copiedId = id;
    setTimeout(() => this.copiedId = null, 2000);
  }
}