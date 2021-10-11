import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public token: Observable<{ access_token: string, id_token: string }[]> | null = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.token = this.http.get<{ access_token: string, id_token: string }[]>('/.auth/me')
      .pipe(
        tap(tokens => {
          console.log(tokens[0])
        })
      );
  }
}
