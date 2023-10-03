import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {MemberListComponent} from "./members/member-list/member-list.component";
import {MemberDetailComponent} from "./members/member-detail/member-detail.component";
import {ListComponent} from "./list/list.component";
import {MessagesComponent} from "./messages/messages.component";
import {authGuard} from "./_guards/auth.guard";
import {MemberEditComponent} from "./members/member-edit/member-edit.component";
import {preventUnsaveChangesGuard} from "./_guards/prevent-unsave-changes.guard";

const routes: Routes = [
  {path: '', component: HomeComponent},
  {
    path: 'members',
    loadChildren: () => import('./members/member.module').then(m => m.MembersModule),
  },
  {
    path: 'lists',
    loadChildren: () => import('./list/list.module').then(m => m.ListModule),
  },
  // {path: '',
  //   runGuardsAndResolvers: 'always',
  //   canActivate: [authGuard],
  //   children: [
  //     {path: 'members', component: MemberListComponent},
  //     {path: 'members/:username', component: MemberDetailComponent},
  //     {path: 'member/edit', component: MemberEditComponent, canDeactivate: [preventUnsaveChangesGuard]},
  //     {path: 'lists', component: ListComponent},
  //     {path: 'messages', component: MessagesComponent},
  //   ]
  // },
  {path: '**', component: HomeComponent, pathMatch:'full'},

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
