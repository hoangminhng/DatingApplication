import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {User} from "../_model/user";
import {BehaviorSubject, map, Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = "http://localhost:5255/api/";
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model: any){
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((respone: User)=>{
        const user = respone;
        if (user){
          localStorage.setItem('user', JSON.stringify(user)) //The localStorage object helps us store data
          this.currentUserSource.next(user);
        }
      } )
    )
  }


  setCurrentUser(user: User){
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}