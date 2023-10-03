import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {ToastrService} from "ngx-toastr";
import {User} from "../_model/user";
import {HttpClient} from "@angular/common/http";
import {AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators} from "@angular/forms";
import {Router} from "@angular/router";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent  implements OnInit{
  @Output() cancelRegister = new EventEmitter();
  @Input() users: any;
  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validatorErrors: string[] | undefined;
  passwordPattern: RegExp = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{4,10}$/;

  constructor(private accountService:AccountService, private toastr:ToastrService,
              private fb: FormBuilder, private router:Router) {
  }
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm(){
    this.registerForm = this.fb.group({
      gender:['male'],
      username:['', Validators.required],
      knownAs:['', Validators.required],
      dateOfBirth:['', Validators.required],
      city:['', Validators.required],
      country:['', Validators.required],
      password:['', [Validators.required, Validators.pattern(this.passwordPattern)]],
      confirmPassword:['', [Validators.required, this.matchValue('password')]],
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: (value) => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity()
      }
    });
  }

  matchValue(matchTo: string): ValidatorFn{
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  register(){
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value, dateOfBirth:dob};
    console.log(values);
    this.accountService.register(values).subscribe({
      next: response  => {
        this.router.navigateByUrl('/members');
      },
      error: err =>{
        if (err.status === 400) {
          this.validatorErrors = ['Username is already existed'];
        }
      }
    })
  }

  cancel(){
    this.cancelRegister.emit(false)
  }

  private getDateOnly(dob:string| undefined){
    if (!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString().slice(0,10);
  }
}
