import {Component, Input, OnInit} from '@angular/core';
import {Message} from "../../_modules/message";
import {MessageService} from "../../_services/message.service";
import {CommonModule} from "@angular/common";
import {TimeagoModule} from "ngx-timeago";

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  imports: [CommonModule, TimeagoModule],
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit{
  @Input() username: string = "";
  messages: Message[] = [];

  constructor(private messageService: MessageService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService.getMessageThread(this.username).subscribe({
      next: messages => this.messages = messages
    });
  }
}
