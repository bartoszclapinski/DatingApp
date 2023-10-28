import { Component } from '@angular/core';
import {AccountService} from "../_services/account.service";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {
    title = 'Dating App';
    model: any = {};
    loggedIn: boolean = false;
  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
    this.getCurrentUser();
  }

  getCurrentUser() {
    this.accountService.currentUser$.subscribe({
      next: user => this.loggedIn = !!user,
      error: err => console.log(err),
    });
  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      this.loggedIn = true;
    });
  }

  logout() {
    this.accountService.logout();
    this.loggedIn = false;
  }
}
