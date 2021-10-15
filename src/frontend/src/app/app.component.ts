import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { defer, Observable, of, ReplaySubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';


const apiUrl = 'https://on-behalf-of-backend-web-2.azurewebsites.net';

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
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  public Status = Status;

  public tokenResponse: HttpResult<string> = null;
  public backendResponse: HttpResult<string> = null;
  public delegatedResponse: HttpResult<string> = null;

  public targetUrlBackend: string = 'https://graph.microsoft.com/v1.0/me';
  public scope: string = 'https://graph.microsoft.com/User.ReadWrite.All';

  private accessToken: string | null = null;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    const status = new ReplaySubject<Status>();
    var request = this.http
      .get<{ access_token: string; id_token: string }[]>('/.auth/me')
      .pipe(
        tap((tokens) => {
          this.accessToken = tokens[0].access_token;
          status.next(Status.Success);
        }),
        map((tokens) => {
          return JSON.stringify(tokens[0], null, 2);
        }),
        catchError((err: HttpErrorResponse) => {
          return of(`Error: ${err.status} = ${err.message}`);
        })
      );

    this.tokenResponse = {
      data: defer(() => {
        status.next(Status.Loading);
        return request;
      }),
      status,
    };

    this.tokenResponse.data.subscribe();
  }

  public refreshToken(): void {
    var request = this.http.get<void>('/.auth/refresh');
    request.subscribe((_) => {
      this.ngOnInit();
    });
  }

  public checkApiHealth(): void {
    const status = new ReplaySubject<Status>();


    var request = this.http
      .get(`${apiUrl}/health`, {
        headers: {
          Authorization: `Bearer ${this.accessToken}`,
          'Content-Type': 'application/json',
        },
        responseType: 'text',
      })
      .pipe(
        catchError((err: HttpErrorResponse) => {
          console.log('now', err);
          return of(`Error: ${err.status} = ${err.message}`);
        }),
        tap((_) => {
          status.next(Status.Success);
        })
      );

    this.backendResponse = {
      data: defer(() => {
        status.next(Status.Loading);
        return request;
      }),
      status,
    };
  }

  public callThroughBackendApi(): void {
    const status = new ReplaySubject<Status>();
    var request = this.http
      .post<unknown>(
        `${apiUrl}/api/OnBehalfOf`,
        { api_url: this.targetUrlBackend, scopes: [this.scope] },
        {
          headers: {
            Authorization: `Bearer ${this.accessToken}`,
            'Content-Type': 'application/json',
          },
        }
      )
      .pipe(
        map((result) => {
          return JSON.stringify(result);
        }),
        catchError((err: HttpErrorResponse) => {
          return of(`Error: ${err.status} = ${err.message}`);
        }),
        tap((_) => {
          status.next(Status.Success);
        })
      );

    this.delegatedResponse = {
      data: defer(() => {
        status.next(Status.Loading);
        return request;
      }),
      status,
    };
  }
}