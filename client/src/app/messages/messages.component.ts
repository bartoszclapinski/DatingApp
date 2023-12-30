import {Component, Input, OnInit} from '@angular/core';
import {Member} from "../_models/member";
import {Message} from "../_modules/message";
import {Pagination} from "../_models/pagination";
import {MessageService} from "../_services/message.service";

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages?: Message[];
  pagination: Pagination = {} as Pagination;
  container: string = "Unread";
  pageNumber: number = 1;
  pageSize: number = 5;
  loading: boolean = false;

  constructor(private messageService: MessageService) { }


  ngOnInit(): void {
   this.loadMessages();
  }

  loadMessages() {
    if (this.container) {
      this.loading = true;
      this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe({
        next: response => {
          this.messages = response.result;
          if (response.pagination) this.pagination = response.pagination;
          this.loading = false;
        }
      });
    }
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        this.messages?.splice(this.messages.findIndex(m => m.id === id), 1);
      }
    });
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
