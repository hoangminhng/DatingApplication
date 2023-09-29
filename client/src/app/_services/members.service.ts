import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {Member} from "../_model/member";
import {map, of} from "rxjs";
import {PaginationResult} from "../_model/pagination";

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  paginatedResult: PaginationResult<Member[]> = new PaginationResult<Member[]>;
  constructor(private http: HttpClient) { }

  getMembers(page?:number, itemPerPage?: number){
    let params = new HttpParams();
    if (page && itemPerPage){
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemPerPage);
    }
    // if (this.members.length > 0)  return of(this.members)

    return this.http.get<Member[]>(this.baseUrl + 'user', {observe: 'response', params}).pipe(
      // map(members => {
      //   this.members = members;
      //   return members;
      // })
      map(respone => {
        if (respone.body){
          this.paginatedResult.result = respone.body;
        }
        const pagination = respone.headers.get('Pagination');
        if (pagination){
          this.paginatedResult.pagination = JSON.parse(pagination);
        }
        return this.paginatedResult
      })
    )
    // return this.http.get<Member[]>(this.baseUrl + 'user')
  }

  getMember(username: string){
    const member = this.members.find(x => x.userName == username);
    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'user/' + username)
  }

  updateMember(member:Member){
    return this.http.put(this.baseUrl + 'user', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member}
      })
    );
  }

  setMainPhoto(PhotoId: number){
    return this.http.put(this.baseUrl + 'user/set-main-photo/' + PhotoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'user/delete-photo/' + photoId)
  }
}
