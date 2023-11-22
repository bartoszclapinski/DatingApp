import {Component, HostListener, OnInit, ViewChild} from '@angular/core';
import {Member} from "../../_models/member";
import {User} from "../../_models/user";
import {AccountService} from "../../_services/account.service";
import {MembersService} from "../../_services/members.service";
import {take} from "rxjs";
import {ToastrService} from "ngx-toastr";
import {NgForm} from "@angular/forms";

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})

export class MemberEditComponent implements OnInit {
  @ViewChild("editForm") editForm: NgForm | undefined;
  @HostListener("window:beforeunload", ["$event"]) unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }
  member: Member | undefined;
  user: User | null = null;

  constructor(
    private accountService: AccountService,
    private memberService: MembersService,
    private toastr: ToastrService
  ){
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    });
  }

  loadMember() {
    if (!this.user) return;
    this.memberService.getMember(this.user.userName).subscribe(member => {
      this.member = member;
    })
  }

  ngOnInit(): void {
    this.loadMember();
  }

  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe(() => {
      this.toastr.success("Profile updated successfully");
      this.editForm?.reset(this.member);
    });
  }
}
