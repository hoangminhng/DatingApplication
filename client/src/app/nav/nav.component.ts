import {Component, OnInit} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {error} from "@angular/compiler-cli/src/transformers/util";
import {Observable, of} from "rxjs";
import {User} from "../_model/user";
import {Router} from "@angular/router";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
  model:any = {};

  ngOnInit(): void {
  }
  constructor(public accountService: AccountService, private router: Router
              , private toastr: ToastrService) {  }

  login(){
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/lists');
      },
      error: error => this.toastr.error(error.error),
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
