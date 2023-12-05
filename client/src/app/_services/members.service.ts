import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {Member} from "../_models/member";
import {map, of} from "rxjs";
import {PaginatedResult} from "../_models/pagination";

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  usersUrl = this.baseUrl + "users";
  userUrl = this.usersUrl + "/";
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>;

  constructor(private http: HttpClient) { }

  getMembers(page?: number, itemsPerPage?: number) {
    let params = new HttpParams();
    if (page && itemsPerPage) {
      params = params.append("pageNumber", page);
      params = params.append("pageSize", itemsPerPage);
    }

    return this.http.get<Member[]>(this.usersUrl, {observe: 'response', params}).pipe(
      map(response => {
        if (response.body){
          this.paginatedResult.result = response.body;
        }
        const pagination = response.headers.get("Pagination");
        if (pagination) {
          this.paginatedResult.pagination = JSON.parse(pagination);
        }
        return this.paginatedResult;
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find(x => x.userName === username);
    if (member) return of(member);
    return this.http.get<Member>(this.userUrl + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.usersUrl, member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member};
      })
    );
  }

  setMainPhoto(photoId: any){
    return this.http.put(this.usersUrl + "/set-main-photo/" + photoId, {});
  }

  deletePhoto(photoId: any) {
    return this.http.delete(this.usersUrl + "/delete-photo/" + photoId);
  }

}
