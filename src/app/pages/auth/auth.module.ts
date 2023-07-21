import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UpdateDataUserDialogComponent } from 'src/app/components/auth_components/update-data-user-dialog/update-data-user-dialog.component';
import { UpdatePasswordUserDialogComponent } from 'src/app/components/auth_components/update-password-user-dialog/update-password-user-dialog.component';
import { DeleteUserDialogComponent } from 'src/app/components/auth_components/delete-user-dialog/delete-user-dialog.component';

import { AuthRoutingModule } from './auth-routing.module';
import { ForgottenPasswordComponent } from './forgotten-password/forgotten-password.component';
import { LoginComponent } from './login/login.component';
import { ManageUsersComponent } from './manage-users/manage-users.component';
import { RegisterComponent } from './register/register.component';

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ForgottenPasswordComponent,
    ManageUsersComponent,
    UpdateDataUserDialogComponent,
    UpdatePasswordUserDialogComponent,
    DeleteUserDialogComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatMenuModule

  ]
})
export class AuthModule { }
