import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

const apiUrl = 'https://on-behalf-of-backend-web.azurewebsites.net';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  private idToken: string = "";
  private accessToken: string = "";
  public tokenResponse: Observable<string> | null = null;
  public backendResponse: Observable<string> | null = null;
  public graphResponse: Observable<string> | null = null;
  public delegatedResponse: Observable<string> | null = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.tokenResponse = this.http.get<{ access_token: string, id_token: string }[]>('/.auth/me')
      .pipe(
        tap(tokens => {
          console.log(tokens[0])
          this.idToken = tokens[0].id_token;
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

  public callBackendApi(): void {
    this.backendResponse = this.http.get<{}>(`${apiUrl}/WeatherForecast`, { headers: { 'Authorization': `Bearer ${this.idToken}` } })
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

  public callGraphApi(): void {
    this.graphResponse = this.http.get<{}>('https://graph.microsoft.com/v1.0/me', { headers: { 'Authorization': `Bearer ${this.accessToken}` } })
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

  public callGraphThroughBackendApi(): void {
    this.delegatedResponse = this.http.post<{}>(`${apiUrl}/Delegated`, 
      { access_token: this.accessToken }, 
      { headers: { 'Authorization': `Bearer ${this.idToken}`, 'Content-Type': 'application/json' } }
    )
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

/*
"login": {
  "loginParameters": [
    "scope=openid profile email https://graph.microsoft.com/User.Read.All"
  ],
  "disableWWWAuthenticate": false
},
*/