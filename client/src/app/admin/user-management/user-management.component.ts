import {Component, OnInit} from '@angular/core';
import {AdminService} from "../../_services/admin.service";
import {User} from "../../_models/user";

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit{
  users: User[] = [];

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: users => this.users = users
    });
  }

}
