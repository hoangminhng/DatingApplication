import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import {FormsModule} from "@angular/forms";
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListComponent } from './list/list.component';
import { MessagesComponent } from './messages/messages.component';
import {SharedModule} from "./_modules/shared.module";
import { MemberCardComponent } from './members/member-card/member-card.component';
import {MatTableModule} from "@angular/material/table";
import {MatButtonModule} from "@angular/material/button";
import {MatPaginatorModule} from "@angular/material/paginator";
import {JwtInterceptor} from "./_interceptor/jwt.interceptor";
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import {GalleryComponent} from "ng-gallery";
import {LoadingInterceptor} from "./_interceptor/loading.interceptor";

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    ListComponent,
    MessagesComponent,
    MemberCardComponent,
    MemberEditComponent,
  ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        BrowserAnimationsModule,
        FormsModule,
        SharedModule,
        MatTableModule,
        MatButtonModule,
        MatPaginatorModule,
        GalleryComponent,
    ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
