import {ActivatedRouteSnapshot, DetachedRouteHandle, RouteReuseStrategy} from "@angular/router";

export class CustomRouteReuseStrategy implements RouteReuseStrategy {
  private handlers: { [key: string]: DetachedRouteHandle } = {};

  shouldDetach(route: ActivatedRouteSnapshot): boolean {
    return false;
  }

  store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle): void {

  }

  shouldAttach(route: ActivatedRouteSnapshot): boolean {
    return false;
  }

  retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null {
    return null;
  }

  shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
    return false;
  }
}
