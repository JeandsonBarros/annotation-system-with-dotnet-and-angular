import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UpdateUserDialogData } from 'src/app/shared/Models/UpdateUserDialogData';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

@Component({
  selector: 'app-update-password-user-dialog',
  templateUrl: './update-password-user-dialog.component.html',
  styleUrls: ['./update-password-user-dialog.component.scss']
})
export class UpdatePasswordUserDialogComponent {
  isLoading = false;
  hidePassword = true;
  hideConfirmPassword = true;

  passwordForm = new FormGroup({
    password: new FormControl('', [Validators.required]),
    confirmPassword: new FormControl('', [Validators.required]),
  })

  constructor(
    public dialogRef: MatDialogRef<UpdatePasswordUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public dataDialog: UpdateUserDialogData,
    private authService: AuthService
  ) {}

  passwordsAreTheSame(): string {

    if (this.passwordForm.get('password')?.value && this.passwordForm.get('confirmPassword')?.value != this.passwordForm.get('password')?.value) {
      this.passwordForm.get('confirmPassword')?.setErrors({ customerror: 'not match' })
      return 'Passwords do not match';
    } else {
      return ''
    }

  }

  updatePassword(): void {

    const password = this.passwordForm.get('password')?.value
   
    if (this.passwordForm.valid && password) {

      this.isLoading = true

      this.dataDialog.updateUser({ password }).subscribe({
        next: () => {
          this.authService.showSnackBar('Update password successful')
          this.isLoading = false
          this.dialogRef.close()
        },
        error: (error) => {
          this.authService.showSnackBar(error?.error?.message || 'Error updating password')
          this.isLoading = false
        }
      })

    }
  }

}
