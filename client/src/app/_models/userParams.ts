import {User} from "./user";

export class UserParams {
  gender: string;
  minAge: number = 18;
  maxAge: number = 150;
  pageNumber: number = 1;
  pageSize: number = 5;

  constructor(user: User) {
    this.gender = user.gender === "female" ? "male" : "female";
  }
}
