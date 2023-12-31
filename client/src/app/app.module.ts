import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {NavComponent} from './nav/nav.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {HomeComponent} from './home/home.component';
import {RegisterComponent} from './register/register.component';
import {MemberListComponent} from './members/member-list/member-list.component';
import {MemberDetailComponent} from './members/member-detail/member-detail.component';
import {ListComponent} from './list/list.component';
import {MessagesComponent} from './messages/messages.component';
import {SharedModule} from "./_modules/shared.module";
import {MemberCardComponent} from './members/member-card/member-card.component';
import {MatTableModule} from "@angular/material/table";
import {MatButtonModule} from "@angular/material/button";
import {MatPaginatorModule} from "@angular/material/paginator";
import {JwtInterceptor} from "./_interceptor/jwt.interceptor";
import {MemberEditComponent} from './members/member-edit/member-edit.component';
import {GalleryComponent} from "ng-gallery";
import {LoadingInterceptor} from "./_interceptor/loading.interceptor";
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DatePickerComponent } from './_forms/date-picker/date-picker.component';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import {RouteReuseStrategy} from "@angular/router";
import {CustomRouteReuse} from "./_services/customRouteReuse";
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';
import { ModalModule } from 'ngx-bootstrap/modal';

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
    TextInputComponent,
    DatePickerComponent,
    PhotoEditorComponent,
    ConfirmDialogComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MatTableModule,
    MatButtonModule,
    MatPaginatorModule,
    GalleryComponent,
    ModalModule.forRoot(),
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
    {provide: RouteReuseStrategy, useClass: CustomRouteReuse}
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
