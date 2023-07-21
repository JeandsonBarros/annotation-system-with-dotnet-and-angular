import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { DeleteUserDialogComponent } from 'src/app/components/auth_components/delete-user-dialog/delete-user-dialog.component';
import { UpdateDataUserDialogComponent } from 'src/app/components/auth_components/update-data-user-dialog/update-data-user-dialog.component';
import { UpdatePasswordUserDialogComponent } from 'src/app/components/auth_components/update-password-user-dialog/update-password-user-dialog.component';
import { UserDto } from 'src/app/shared/DTOs/UserDto';
import { User } from 'src/app/shared/Models/User';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

@Component({
  selector: 'app-manage-users',
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.scss']
})
export class ManageUsersComponent implements OnInit {

  users: User[] = [];
  paginationUsers = { pageNumber: 1, totalPages: 0 };
  search = '';

  constructor(private authService: AuthService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.getAllUsers()
  }

  getAllUsers(pageNumber = 1): void {

    this.authService.getAllUsers(pageNumber).subscribe({
      next: (pageUsers) => {

        if (pageNumber === 1) {
          this.users = pageUsers.data
        } else {
          this.users = this.users.concat(pageUsers.data)
        }

        this.paginationUsers = { pageNumber: pageUsers.page, totalPages: pageUsers.totalPages }

      }
    })

  }

  findUserByEmail(email: string, pageNumber = 1): void {

    this.search = email;
    
    if (this.search) {

      this.authService.findUserByEmail(this.search, pageNumber).subscribe({
        next: (pageUsers) => {

          if (pageNumber === 1) {
            this.users = pageUsers.data
          } else {
            this.users = this.users.concat(pageUsers.data)
          }

          this.paginationUsers = { pageNumber: pageUsers.page, totalPages: pageUsers.totalPages }

        }
      })

    } else {
      this.getAllUsers()
    }

  }

  updateDataUser(user: User): void {

    this.dialog.open(UpdateDataUserDialogComponent, {
      data: {
        user: user,
        updateUser: (userDto: UserDto): Observable<User> => this.authService.updateAUser(user.email, userDto)
      }
    });

  }

  updatePasswordUser(user: User): void {

    this.dialog.open(UpdatePasswordUserDialogComponent, {
      data: {
        user: user,
        updateUser: (userDto: UserDto): Observable<User> => this.authService.updateAUser(user.email, userDto)
      }
    });

  }

  deleteUser(userEmail: string): void {

    this.dialog.open(DeleteUserDialogComponent, {
      data: (): Observable<void> => this.authService.deleteAUser(userEmail),
    });

  }

}
