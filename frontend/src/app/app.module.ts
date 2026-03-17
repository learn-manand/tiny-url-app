import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { App } from './app'
import { UrlManagerComponent } from './component/url-manager/url-manager.component'
import { UrlPatternValidatorDirective } from './validators/url-pattern.validator';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@NgModule({
  declarations: [
    App,
    UrlManagerComponent,
    UrlPatternValidatorDirective
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    MatSnackBarModule
  ],
  bootstrap: [App]
})
export class AppModule {}