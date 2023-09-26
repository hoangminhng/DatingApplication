import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{
  registerMode = false;
  users:any;
  ngOnInit(): void {
    this.getUser()
  }

  constructor(private http:HttpClient) {
  }

  registerToggle(){
    this.registerMode = !this.registerMode;
  }

  getUser(){
    this.http.get('http://localhost:5255/api/user').subscribe({
      next: respones => this.users = respones,
      error: error => console.log(error),
      complete: () => console.log('Request has completed. HoangMinhNg')
    })
  }

  cancelRegisterMode(event: boolean){
    this.registerMode = event
  }
}
