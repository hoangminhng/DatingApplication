import {Component, HostListener, OnInit, ViewChild} from '@angular/core';
import {Member} from "../../_model/member";
import {AccountService} from "../../_services/account.service";
import {MembersService} from "../../_services/members.service";
import {take} from "rxjs";
import {User} from "../../_model/user";
import {ToastrService} from "ngx-toastr";
import {NgForm} from "@angular/forms";

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit{
  @ViewChild('editForm') editForm: NgForm | undefined
  @HostListener('window:beforerunload', ['$event']) unloadNotification($event:any){
    $event.returnValue = true;
  }
  member: Member | undefined;
  user: User | null = null;

  constructor(private accountSerive: AccountService, private memberSerive: MembersService, private toaster: ToastrService) {
    this.accountSerive.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user,
    })
  }
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    if (!this.user) return;
    this.memberSerive.getMember(this.user.username).subscribe({
      next: member => this.member = member
    })
  }

  updateMember(){
    this.memberSerive.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        this.toaster.success('Profile update successfully');
        this.editForm?.reset(this.member)
      }
    })

  }
}
