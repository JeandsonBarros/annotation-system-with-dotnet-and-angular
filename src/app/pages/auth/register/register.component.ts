import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  isLoading = false;
  hidePassword = true;
  hideConfirmPassword = true;

  registerForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
    confirmPassword: new FormControl('', [Validators.required]),
    name: new FormControl('', [Validators.required])
  })

  constructor(private authService: AuthService, private router: Router) { }

  passwordsAreTheSame(): string {

    if (this.registerForm.get('password')?.value && this.registerForm.get('confirmPassword')?.value != this.registerForm.get('password')?.value) {
      this.registerForm.get('confirmPassword')?.setErrors({ customerror: 'not match' })
      return 'Passwords do not match';
    } else {
      return ''
    }

  }

  register() {

    const email = this.registerForm.get('email')?.value;
    const password = this.registerForm.get('password')?.value;
    const name = this.registerForm.get('name')?.value;
    
    if (email && name && password && this.registerForm.valid) {

      this.isLoading = true;
      
      this.authService.register({ email, name, password }).subscribe({

        next: () => {
          this.isLoading = false;
          this.authService.showSnackBar("Registration completed successfully")
          this.router.navigate(['/auth/login'])
        },
        error: (error) => {
          this.isLoading = false;
          console.log("register error", error);
          this.authService.showSnackBar(error?.error?.message || "Register failed")
        }

      })

    } else {
      this.authService.showSnackBar("Check the fields")
    }

  }

}
