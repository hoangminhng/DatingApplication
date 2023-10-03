import {Routes} from "@angular/router";
import {MemberListComponent} from "./member-list/member-list.component";
import {MemberDetailComponent} from "./member-detail/member-detail.component";

export const memberRoutes: Routes = [
  {
    path: 'members',
    component: MemberListComponent,
    children: [
      {path: 'members/:username', component: MemberDetailComponent},
    ]
  }
  ]
