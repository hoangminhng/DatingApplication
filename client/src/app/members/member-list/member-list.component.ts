import {Component, OnInit} from '@angular/core';
import {Member} from "../../_model/member";
import {MembersService} from "../../_services/members.service";
import {Observable} from "rxjs";
import {Pagination} from "../../_model/pagination";

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit{
  // members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  pageNumber = 1;
  pageSize = 5;
  constructor(private memberServices: MembersService) {
  }

  ngOnInit(): void {
    // this.members$ = this.memberServices.getMembers();
    this.loadMembers();
  }

  loadMembers(){
    this.memberServices.getMembers(this.pageNumber, this.pageSize).subscribe({
      next: response => {
        if (response.result && response.pagination){
          this.members = response.result;
          this.pagination = response.pagination;
        }
      }
    })
  }

  pageChange(event: any){
    if (this.pageNumber !== event.page){
      this.pageNumber = event.page;
      this.loadMembers();
    }

  }

}
