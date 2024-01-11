import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {User} from "../_models/user";

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
}
