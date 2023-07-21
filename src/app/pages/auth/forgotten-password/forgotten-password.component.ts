import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

@Component({
  selector: 'app-forgotten-password',
  templateUrl: './forgotten-password.component.html',
  styleUrls: ['./forgotten-password.component.scss']
})
export class ForgottenPasswordComponent {

  isLoadingSend = false;
  isLoadingUpdate = false;
  isSentCode = false;
  hidePassword = true;
  hideConfirmPassword = true;

  updatePasswordForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
    confirmPassword: new FormControl('', [Validators.required]),
    code: new FormControl(NaN, [Validators.required])
  })

  passwordsAreTheSame(): string {

    if (this.updatePasswordForm.get('password')?.value && this.updatePasswordForm.get('confirmPassword')?.value != this.updatePasswordForm.get('password')?.value) {
      this.updatePasswordForm.get('confirmPassword')?.setErrors({ customerror: 'not match' })
      return 'Passwords do not match';
    } else {
      return ''
    }

  }

  constructor(private authService: AuthService, private router: Router) {}

  sendEmailCodeForForgottenPassword(): void {

    const email = this.updatePasswordForm.get('email')

    if (email?.value && email?.valid) {

      this.isLoadingSend = true

      this.authService.sendEmailCodeForForgottenPassword(email.value).subscribe({

        next: (response) => {
          this.authService.showSnackBar(response.message)
          this.isSentCode = true
          this.isLoadingSend = false
        },

        error: (err: any) => {
          this.isLoadingSend = false
          this.authService.showSnackBar(err?.error?.message || "Error sending email")
        }
        
      })

    } else {
      this.authService.showSnackBar("Check the fields")
    }

  }

  updatePassword(): void {

    const email = this.updatePasswordForm.get('email')?.value;
    const password = this.updatePasswordForm.get('password')?.value;
    const code = this.updatePasswordForm.get('code')?.value;

    if (email && code && password && this.updatePasswordForm.valid) {

      this.isLoadingUpdate = true;
      
      this.authService.updateForgottenPassword( email, password, code ).subscribe({

        next: () => {
          this.isLoadingUpdate = false;
          this.authService.showSnackBar("Update password completed successfully")
          this.router.navigate(['/auth/login'])
        },

        error: (error) => {
          this.isLoadingUpdate = false;
          this.authService.showSnackBar(error?.error?.message || "Update failed")
        }

      })

    } else {
      this.authService.showSnackBar("Check the fields")
    }

  }

}
