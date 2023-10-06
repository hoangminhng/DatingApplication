import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {User} from "../_model/user";
import {BehaviorSubject, map, Observable} from "rxjs";
import {environment} from "../../environments/environment";
import {PresenceService} from "./presence.service";

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  login(model: any){
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((respone: User)=>{
        const user = respone;
        if (user){
          localStorage.setItem('user', JSON.stringify(user)) //The localStorage object helps us store data
          this.setCurrentUser(user);
        }
      } )
    )
  }

  register(model:any){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user){
          this.setCurrentUser(user)
        }
        return user;
      })
    )
  }

  setCurrentUser(user: User){
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    this.presenceService.createHubConnection(user)
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection()
  }
}
