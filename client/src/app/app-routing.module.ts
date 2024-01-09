import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {MemberListComponent} from "./members/member-list/member-list.component";
import {MessagesComponent} from "./messages/messages.component";
import {AuthGuard} from "./_guards/auth.guard";
import {TestErrorComponent} from "./errors/test-error/test-error.component";
import {NotFoundComponent} from "./errors/not-found/not-found.component";
import {ServerErrorComponent} from "./errors/server-error/server-error.component";
import {MemberDetailsComponent} from "./members/member-details/member-details.component";
import {MemberEditComponent} from "./members/member-edit/member-edit.component";
import {preventUnsavedChangesGuard} from "./_guards/prevent-unsaved-changes.guard";
import {ListsComponent} from "./lists/lists.component";
import {resolve} from "@angular/compiler-cli";
import {memberDetailedResolver} from "./_resolvers/member-detailed.resolver";
import {AdminPanelComponent} from "./admin/admin-panel/admin-panel.component";

const routes: Routes = [
  {path: "", component: HomeComponent},
  {path: '', runGuardsAndResolvers: "always", canActivate: [AuthGuard], children: [
      {path: "members", component: MemberListComponent},
      {path: "members/:username", component: MemberDetailsComponent, resolve: {member: memberDetailedResolver}},
      {path: "member/edit", component: MemberEditComponent, canDeactivate: [preventUnsavedChangesGuard]},
      {path: "lists", component: ListsComponent},
      {path: "messages", component: MessagesComponent},
      {path: "admin", component: AdminPanelComponent}
    ]},
  {path: "errors", component: TestErrorComponent},
  {path: "not-found", component: NotFoundComponent},
  {path: "server-error", component: ServerErrorComponent},
  {path: "**", component: HomeComponent, pathMatch: "full"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
