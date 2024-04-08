import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {User} from "../_models/user";
import {Photo} from "../_models/photo";

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + "admin/users-with-roles");
  }

  updateUserRoles(userName: string, roles: string[]) {
    return this.http.post(this.baseUrl + "admin/edit-roles/" + userName + "?roles=" + roles, {});
  }

  getPhotosForApproval() {
    return this.http.get<Photo[]>(this.baseUrl + "admin/photos-to-moderate");
  }

  approvePhoto(photoId: any){
    return this.http.post(this.baseUrl + "admin/approve-photo/" + photoId, {});
  }

  rejectPhoto(photoId: any){
    return this.http.post(this.baseUrl + "admin/reject-photo/" + photoId, {});
  }
}
