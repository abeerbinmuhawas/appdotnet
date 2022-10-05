import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registermode = false;
  users: any;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }



  registertoggel() {
    this.registermode = !this.registermode;
  }



  getusers() {
    this.http.get('https://localhost:5001/api/users').subscribe(users => this.users = users);
  }

  cancelRegisterMode(event: boolean) {
    this.registermode = event;
  }

}

