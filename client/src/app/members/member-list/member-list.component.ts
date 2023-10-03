import {Component, OnInit} from '@angular/core';
import {Member} from "../../_model/member";
import {MembersService} from "../../_services/members.service";
import {Observable, take} from "rxjs";
import {Pagination} from "../../_model/pagination";
import {UserParams} from "../../_model/userParams";
import {User} from "../../_model/user";
import {AccountService} from "../../_services/account.service";

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];

  constructor(private memberServices: MembersService) {
    this.userParams = memberServices.getUserParams();
  }

  ngOnInit(): void {
    // this.members$ = this.memberServices.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    if (!this.userParams) return;
    if (this.userParams) {
      this.memberServices.setUserParams(this.userParams);
      this.memberServices.getMembers(this.userParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }
  }

  resetFilter() {
    this.userParams = this.memberServices.resetUserParams();
    this.loadMembers();
  }

  pageChange(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.memberServices.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
