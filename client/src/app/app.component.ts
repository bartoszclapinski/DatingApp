import {Component, OnInit} from '@angular/core';
import { HttpClient } from "@angular/common/http";
import {AccountService} from "./_services/account.service";
import {User} from "./_models/user";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating App';
  users: any;

  constructor(private http: HttpClient, private accountService: AccountService) {
  }

  ngOnInit(): void {
    this.getUsers();
    this.setCurrentUser();
  }

  getUsers() {
    this.http.get("https://localhost:5001/api/users").subscribe({
      next: response => this.users = response,
      error: err => console.log(err),
      complete: () => console.log("Completed")
    });
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem("user") || "{}");
    this.accountService.setCurrentUser(user);
  }

}
