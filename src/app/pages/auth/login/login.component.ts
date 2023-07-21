import { AuthService } from './../../../shared/services/auth/auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  isLoading = false;
  hidePassword = true;

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required])
  })

  constructor(private authService: AuthService, private router: Router) { }

  login() {

    const email = this.loginForm.get('email')?.value;
    const password = this.loginForm.get('password')?.value;

    if (this.loginForm.valid && email && password) {

      this.isLoading = true;

      this.authService.login(email, password).subscribe({

        next: () => {
          this.router.navigate(['/'])
        },
        error: (error) => {
          this.isLoading = false;
          this.authService.showSnackBar(error?.error?.message || "Login failed")
        }
        
      });

    } else {
      this.authService.showSnackBar("Check the fields")
    }

  }

}
