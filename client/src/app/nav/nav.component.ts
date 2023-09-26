import {Component, OnInit} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {error} from "@angular/compiler-cli/src/transformers/util";
import {Observable, of} from "rxjs";
import {User} from "../_model/user";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
  model:any = {};

  ngOnInit(): void {
  }
  constructor(public accountService: AccountService) {
  }

  login(){
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
      },
      error: error => console.log(error),
    })
  }

  logout(){
    this.accountService.logout();
  }
}
