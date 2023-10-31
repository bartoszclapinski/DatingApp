import {Component, OnInit} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {Observable, of} from "rxjs";
import {User} from "../_models/user";
import {Router} from "@angular/router";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
    title = 'Dating App';
    model: any = {};

  constructor(public accountService: AccountService, private router: Router) { }

  ngOnInit(): void { }

  login() {
    this.accountService.login(this.model).subscribe(() => {
      this.router.navigateByUrl("/members").then(r => console.log(r));
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl("/").then(r => console.log(r));
  }
}
