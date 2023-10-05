import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterModule, Routes} from "@angular/router";
import {MemberListComponent} from "./member-list/member-list.component";
import {MemberDetailComponent} from "./member-detail/member-detail.component";
import {preventUnsaveChangesGuard} from "../_guards/prevent-unsave-changes.guard";
import {MemberEditComponent} from "./member-edit/member-edit.component";
import {memberDetailedResolver} from "../_resolvers/member-detailed.resolver";

const routes: Routes = [
  {path: '', component: MemberListComponent},
  {path: 'edit', component: MemberEditComponent, canDeactivate: [preventUnsaveChangesGuard]},
  {path: ':username', component: MemberDetailComponent, resolve: {member: memberDetailedResolver}},

]

@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
    exports: [
        RouterModule,
    ]
})
export class MembersModule { }
