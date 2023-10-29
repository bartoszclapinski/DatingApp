import {Component, OnInit} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {Observable, of} from "rxjs";
import {User} from "../_models/user";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
    title = 'Dating App';
    model: any = {};

  constructor(public accountService: AccountService) { }

  ngOnInit(): void { }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
    });
  }

  logout() {
    this.accountService.logout();
  }
}
