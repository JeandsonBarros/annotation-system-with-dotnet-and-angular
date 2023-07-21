import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

@Component({
  selector: 'app-delete-user-dialog',
  templateUrl: './delete-user-dialog.component.html',
  styleUrls: ['./delete-user-dialog.component.scss']
})
export class DeleteUserDialogComponent {
  isLoading = false;

  constructor(
    public dialogRef: MatDialogRef<DeleteUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public deleteFunction: () => Observable<void>,
    private authService: AuthService,
  ) { }

  deleteAccount(): void {

    this.isLoading = true;

    this.deleteFunction().subscribe({
      next: () => {
        this.isLoading = false;
        this.dialogRef.close(true);
      },
      error: (error) => {
        this.isLoading = false;
        this.authService.showSnackBar(error?.error?.message || "Error deleting")
      }
    })

  }

  onNoClick(): void {
    this.dialogRef.close();
  }
  
}
