import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { defer, Observable, of, ReplaySubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

const apiUrl = 'https://on-behalf-of-backend-web.azurewebsites.net';

type HttpResult<T> = {
  data: Observable<T>;
  status: ReplaySubject<Status>;
} | null;

enum Status {
  Loading,
  Success,
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public Status = Status;

  public tokenResponse: HttpResult<string> = null;
  public backendResponse: HttpResult<string> = null;
  public graphResponse: HttpResult<string> = null;
  public delegatedResponse: HttpResult<string> = null;

  public targetUrl: string = 'https://graph.microsoft.com/v1.0/me';
  public targetUrlBackend: string = "https://graph.microsoft.com/v1.0/me";

  private idToken: string | null = null;
  private accessToken: string | null = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    const status = new ReplaySubject<Status>();
    var request = this.http.get<{ access_token: string, id_token: string }[]>('/.auth/me')
      .pipe(
        tap(tokens => {
          this.idToken = tokens[0].id_token;
          this.accessToken = tokens[0].access_token;
          status.next(Status.Success);
        }),
        map(tokens => {
          return JSON.stringify(tokens[0]);
        }),
        catchError((err: HttpErrorResponse) => {
          return of(`Error: ${err.status} = ${err.message}`);
        }),
      );

    this.tokenResponse = {
      data: defer(() => {
        status.next(Status.Loading);
        return request;
      }), status
    };
  }

  public callBackendApi(): void {
    const status = new ReplaySubject<Status>();
    var request = this.http.get<unknown>(`${apiUrl}/WeatherForecast`,
      { headers: { 'Authorization': `Bearer ${this.idToken}` } }
    )
      .pipe(
        map(result => {
          return JSON.stringify(result);
        }),
        catchError((err: HttpErrorResponse) => {
          return of(`Error: ${err.status} = ${err.message}`);
        }),
        tap(_ => {
          status.next(Status.Success);
        })
      );

    this.backendResponse = {
      data: defer(() => {
        status.next(Status.Loading);
        return request;
      }), status
    };
  }

  public callGraphApi(): void {
    const status = new ReplaySubject<Status>();
    var request = this.http.get<unknown>(this.targetUrl,
      { headers: { 'Authorization': `Bearer ${this.accessToken}` } }
    )
      .pipe(
        map(result => {
          return JSON.stringify(result);
        }),
        catchError((err: HttpErrorResponse) => {
          return of(`Error: ${err.status} = ${err.message}`);
        }),
        tap(_ => {
          status.next(Status.Success);
        })
      );

    this.graphResponse = {
      data: defer(() => {
        status.next(Status.Loading);
        return request;
      }), status
    };
  }

  public callThroughBackendApi(): void {
    const status = new ReplaySubject<Status>(Status.Loading);
    var request = this.http.post<unknown>(`${apiUrl}/Delegated`,
      { api_url: this.targetUrlBackend, access_token: this.accessToken },
      { headers: { 'Authorization': `Bearer ${this.idToken}`, 'Content-Type': 'application/json' } }
    )
      .pipe(
        map(result => {
          return JSON.stringify(result);
        }),
        catchError((err: HttpErrorResponse) => {
          return of(`Error: ${err.status} = ${err.message}`);
        }),
        tap(_ => {
          status.next(Status.Success);
        })
      );

    this.delegatedResponse = {
      data: defer(() => {
        status.next(Status.Loading);
        return request;
      }), status
    };
  }
}