import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {ToastrService} from "ngx-toastr";
import {User} from "../_model/user";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent  implements OnInit{
  @Output() cancelRegister = new EventEmitter();
  model: any = {}
  @Input() users: any;
  constructor(private aaccountService:AccountService, private toastr:ToastrService, private http: HttpClient) {
  }
  ngOnInit(): void {
  }

  register(){
    this.aaccountService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: err =>{
        this.toastr.error(err.error),
        console.log(err)
      }
    })
  }

  cancel(){
    this.cancelRegister.emit(false)
  }
}
