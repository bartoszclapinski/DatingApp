import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {Member} from "../_models/member";
import {map, of, take} from "rxjs";
import {UserParams} from "../_models/userParams";
import {AccountService} from "./account.service";
import {User} from "../_models/user";
import {getPaginatedResult, getPaginationHeaders} from "./paginationHelper";


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  usersUrl = this.baseUrl + "users";
  userUrl = this.usersUrl + "/";
  members: Member[] = [];
  membersCache = new Map();
  userParams: UserParams | undefined;
  user: User | undefined

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    });
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  getMembers(userParams: UserParams) {
    const response = this.membersCache.get(Object.values(userParams).join("-"));
    if (response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append("minAge", userParams.minAge);
    params = params.append("maxAge", userParams.maxAge);
    params = params.append("gender", userParams.gender);
    params = params.append("orderBy", userParams.orderBy);

    return getPaginatedResult<Member[]>(this.usersUrl, params, this.http).pipe(
      map(response => {
        this.membersCache.set(Object.values(userParams).join("-"), response);
        return response;
      })
    );
  }

  getMember(username: string) {
    const member = [...this.membersCache.values()]
      .reduce((array, element) => array.concat(element.result), [])
      .find((member: Member) => member.userName === username);
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

  addLike(username: string){
    return this.http.post(this.baseUrl + "likes/" + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append("predicate", predicate);
    return getPaginatedResult<Member[]>(this.baseUrl + "likes", params, this.http);
  }

}

