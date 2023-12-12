import {Component, OnInit} from '@angular/core';
import {Member} from "../_models/member";
import {MembersService} from "../_services/members.service";
import {Pagination} from "../_models/pagination";

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit{
  members: Member[] | undefined;
  predicate: string = "liked";
  pageNumber: number = 1;
  pageSize: number = 5;
  pagination: Pagination = {} as Pagination;

  constructor(private memberService: MembersService) {
  }
  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe({
      next: response => {
        this.members = response.result;
        if (response.pagination) this.pagination = response.pagination;
      }
    });
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }
}
