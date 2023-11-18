import {Component, OnInit} from '@angular/core';
import {Member} from "../../_models/member";
import {ActivatedRoute} from "@angular/router";
import {MembersService} from "../../_services/members.service";

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})
export class MemberDetailsComponent implements OnInit {
  member: Member | undefined;

  constructor(private memberService: MembersService, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    var username = this.route.snapshot.paramMap.get("username");
    if (!username) return;
    this.memberService.getMember(username).subscribe(member => {
      this.member = member;
    })
  }
}
