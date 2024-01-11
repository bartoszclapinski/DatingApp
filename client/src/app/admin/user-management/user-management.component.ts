import {Component, OnInit} from '@angular/core';
import {AdminService} from "../../_services/admin.service";
import {User} from "../../_models/user";
import {BsModalRef, BsModalService, ModalOptions} from "ngx-bootstrap/modal";
import {RolesModalComponent} from "../../modals/roles-modal/roles-modal.component";

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit{
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();

  constructor(private adminService: AdminService, private modalService: BsModalService) {}

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: users => this.users = users
    });
  }

  openRolesModal() {
    const initialState: ModalOptions = {
      initialState: {
        list: ['Data', 'Data2', 'Data3'],
        title: 'Data-Title'
      }
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, initialState);
    this.bsModalRef.content!.closeBtnName = 'Close';
  }

}
