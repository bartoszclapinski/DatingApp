import {Component, OnInit} from '@angular/core';
import {Member} from "../../_models/member";
import {ActivatedRoute} from "@angular/router";
import {MembersService} from "../../_services/members.service";
import {CommonModule} from "@angular/common";
import {TabsModule} from "ngx-bootstrap/tabs";
import {GalleryItem, GalleryModule, ImageItem} from "ng-gallery";

@Component({
  selector: 'app-member-details',
  standalone: true,
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule]
})
export class MemberDetailsComponent implements OnInit {
  member: Member | undefined;
  images: GalleryItem[] = [];

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
      this.getImages();
    })
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member.photos ?? []) {
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}));
    }
  }
}
