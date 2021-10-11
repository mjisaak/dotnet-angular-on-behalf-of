import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  private accessToken: string = "";
  public tokenResponse: Observable<string> | null = null;
  public apiResponse: Observable<string> | null = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.tokenResponse = this.http.get<{ access_token: string, id_token: string }[]>('/.auth/me')
      .pipe(
        tap(tokens => {
          console.log(tokens[0])
          this.accessToken = tokens[0].access_token;
        }),
        map(tokens => {
          return JSON.stringify(tokens[0]);
        }),
        catchError((err: HttpErrorResponse) => {
          return of(`Error: ${err.status} = ${err.message}`);
        })
      );
  }

  public callApi(): void {
    this.apiResponse = this.http.get<{}>('https://on-behalf-of-backend-web.azurewebsites.net/WeatherForecast', { headers: { 'Authorization': `Bearer ${this.accessToken}` } })
      .pipe(
        tap(result => {
          console.log(result)
        }),
        map(result => {
          return JSON.stringify(result);
        }),
        catchError((err: HttpErrorResponse) => {
          return of(`Error: ${err.status} = ${err.message}`);
        })
      );
  }
}
