import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {error} from "@angular/compiler-cli/src/transformers/util";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{

  title = 'client';
  users: any;
  constructor(private http: HttpClient) {

  }

  ngOnInit(): void {
    this.http.get('http://localhost:5255/api/user').subscribe({
      next: respones => this.users = respones,
      error: error => console.log(error),
      complete: () => console.log('Request has completed. HoangMinhNg')
    })
  }
}
