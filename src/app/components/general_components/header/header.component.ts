import { Component, OnInit } from '@angular/core';
import { NavigationStart, Router } from '@angular/router';
import { User } from 'src/app/shared/Models/User';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { UpdateDataUserDialogComponent } from '../../auth_components/update-data-user-dialog/update-data-user-dialog.component';
import { UserDto } from 'src/app/shared/DTOs/UserDto';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { UpdatePasswordUserDialogComponent } from '../../auth_components/update-password-user-dialog/update-password-user-dialog.component';
import { DeleteUserDialogComponent } from '../../auth_components/delete-user-dialog/delete-user-dialog.component';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  html = document.querySelector('html');
  isDark = false;
  isLoggedIn = false;
  isVisibleSmallSearch = false;
  search = '';
  userLogged!: User

  constructor(
    private router: Router,
    private authService: AuthService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {

    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        /* console.log(event.url); */
        
        if (localStorage.getItem('token')) {

          this.authService.getAccountData().subscribe(response => {
            this.userLogged = response
          })

          this.isLoggedIn = true

        } else {
          this.isLoggedIn = false
        }

      }
    });

    const prefersColorScheme = window.matchMedia('(prefers-color-scheme: dark)');
    if (this.html && prefersColorScheme.matches) {
      this.html.classList.add('dark')
      this.isDark = true;
    }
    
  }

  toggleTheme(): void {
    if (this.html) this.html.classList.toggle('dark')
    this.isDark = !this.isDark
  }

  searchAnnotation(event: Event): void {

    this.search = (event.target as HTMLInputElement).value;

    if (this.search != "") {
      this.router.navigate(['annotations/search/' + this.search])
    }
    else {
      this.router.navigate([''])
    }

  }

  updateDataUser() {

    this.dialog.open(UpdateDataUserDialogComponent, {
      data: {
        user: this.userLogged,
        updateUser: (userDto: UserDto): Observable<User> => this.authService.updateAccount(userDto)
      }
    });

  }

  updatePasswordUser() {

    this.dialog.open(UpdatePasswordUserDialogComponent, {
      data: {
        user: this.userLogged,
        updateUser: (userDto: UserDto): Observable<User> => this.authService.updateAccount(userDto)
      }
    });

  }

  deleteUser(): void {

    this.dialog.open(DeleteUserDialogComponent, {
      data: this.authService.deleteAccount,
    });

  }

  logout() {
    localStorage.removeItem('token')
    this.router.navigate(['/auth/login']);
  }

}
