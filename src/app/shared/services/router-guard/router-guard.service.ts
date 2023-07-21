import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Router, UrlTree } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class RouterGuardService {

  constructor(private router: Router) { }
  canActivate():
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    if (!localStorage.getItem('token')) {
      this.router.navigate(['/auth/login']);
      return false;
    }
    return true;
  }
}
