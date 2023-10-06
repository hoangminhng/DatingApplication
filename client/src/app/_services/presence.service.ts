import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {ToastrService} from "ngx-toastr";
import {User} from "../_model/user";
import {BehaviorSubject, take} from "rxjs";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUser$ = this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) {
  }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUser$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUserSource.next([...usernames, username])
      })
    });

    this.hubConnection.on('UserIsOffine', username => {
      this.onlineUser$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUserSource.next(usernames.filter(x => x !== username))
      })
    });

    this.hubConnection.on("GetOnlineUsers", username => {
      this.onlineUserSource.next(username);
    })

    this.hubConnection.on("NewMessageReceived", ({username, knownAs}) => {
      this.toastr.info(knownAs + 'has sent you a message! Click to see it')
        .onTap
        .pipe(take(1)).subscribe({
        next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
      })
    })

  }

  stopHubConnection() {
    this.hubConnection?.stop().catch((error: any) => console.log(error));
  }
}
