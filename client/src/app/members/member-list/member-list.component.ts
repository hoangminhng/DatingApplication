import {Component, OnInit} from '@angular/core';
import {Member} from "../../_model/member";
import {MembersService} from "../../_services/members.service";

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit{
  members: Member[] = [];
  constructor(private memberServices: MembersService) {
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    this.memberServices.getMembers().subscribe({
      next: members => this.members = members
    })
  }

}
