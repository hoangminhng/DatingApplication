import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {error} from "@angular/compiler-cli/src/transformers/util";
import {AccountService} from "./_services/account.service";
import {User} from "./_model/user";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{

  title = 'client';
  users: any;
  constructor(private http: HttpClient, private accountService: AccountService) {

  }

  ngOnInit(): void {
    this.getUser();
    this.setCurrentUser();
  }

  getUser(){
    this.http.get('http://localhost:5255/api/user').subscribe({
      next: respones => this.users = respones,
      error: error => console.log(error),
      complete: () => console.log('Request has completed. HoangMinhNg')
    })
  }
  setCurrentUser(){
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user: User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
}
