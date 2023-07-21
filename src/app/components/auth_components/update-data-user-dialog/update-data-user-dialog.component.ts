import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { UpdateUserDialogData } from 'src/app/shared/Models/UpdateUserDialogData';

@Component({
  selector: 'app-update-data-user-dialog',
  templateUrl: './update-data-user-dialog.component.html',
  styleUrls: ['./update-data-user-dialog.component.scss']
})
export class UpdateDataUserDialogComponent implements OnInit {

  isLoading = false;

  dataForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    name: new FormControl('', [Validators.required])
  })

  constructor(
    public dialogRef: MatDialogRef<UpdateDataUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public dataDialog: UpdateUserDialogData,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.dataForm.get('email')?.setValue(this.dataDialog.user.email);
    this.dataForm.get('name')?.setValue(this.dataDialog.user.name);
  }

  updateData(): void {

    const email = this.dataForm.get('email')?.value
    const name = this.dataForm.get('name')?.value

    if (this.dataForm.valid && email && name) {

      this.isLoading = true

      this.dataDialog.updateUser({ email, name }).subscribe({
        next: () => {
          this.authService.showSnackBar('Update data successful')
          this.isLoading = false
          this.dialogRef.close()
        },
        error: (error) => {
          this.authService.showSnackBar(error?.error?.message || 'Error updating data')
          this.isLoading = false
        }
      })

    }

  }

}
