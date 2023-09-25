import { Component } from '@angular/core';
import {AccountService} from "../_services/account.service";
import {error} from "@angular/compiler-cli/src/transformers/util";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {
  model:any = {};
  loggeddIn = false;
  constructor(private accountService: AccountService) {
  }

  login(){
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
        this.loggeddIn = true;
      },
      error: error => console.log(error),
    })
  }

}
