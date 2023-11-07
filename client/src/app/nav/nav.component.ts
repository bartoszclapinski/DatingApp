import {Component, OnInit} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {Router} from "@angular/router";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
    title = 'Dating App';
    model: any = {};

  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void { }

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => this.router.navigateByUrl("/members")
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl("/").then(r => console.log(r));
  }
}
