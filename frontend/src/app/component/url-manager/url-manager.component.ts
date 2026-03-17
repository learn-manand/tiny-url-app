import { Component, OnInit } from '@angular/core';
import { Observable, BehaviorSubject, combineLatest, map } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
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

  allUrls$!: Observable<UrlItem[]>;
  filteredUrls$!: Observable<UrlItem[]>;

  url = '';
  isPrivate = false;
  searchTerm = '';
  private searchTerm$ = new BehaviorSubject<string>('');


  constructor(private state: UrlState, private snackBar: MatSnackBar) {}


  ngOnInit(): void {
    this.allUrls$ = this.state.urls$;
    
    this.filteredUrls$ = combineLatest([
      this.allUrls$,
      this.searchTerm$
    ]).pipe(
    map(([urls, term]) => {
        if (!term) return urls;

        term = term.toLowerCase();

        return urls.filter(u =>
          u.originalUrl.toLowerCase().includes(term) ||
          u.shortUrl.toLowerCase().includes(term)
        );
      })
    );

    this.state.load();

    window.addEventListener('focus', () => {
      this.state.load();
    });
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
    this.searchTerm$.next(this.searchTerm);
  }

  delete(code: string){
    this.state.delete(code);
  }

  copy(url: string){
    navigator.clipboard.writeText(url);

    this.snackBar.open('Copied to clipboard!', 'Close', {
    duration: 2000,
    horizontalPosition: 'center',
    verticalPosition: 'bottom'
  });
  }
}