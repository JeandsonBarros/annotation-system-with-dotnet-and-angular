import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { map, Observable } from 'rxjs';

import { UserDto } from '../../DTOs/UserDto';
import { User } from '../../Models/User';
import { Message } from './../../Models/Message';
import { Page } from '../../Models/Page';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseUrl = 'http://localhost:8080/api/auth'
  headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': `${localStorage.getItem('token')}`
  })

  constructor(
    private httpClient: HttpClient,
    private snackBar: MatSnackBar
  ) { }

  showSnackBar(message: string): void {
    this.snackBar.open(message, 'X',
      {
        horizontalPosition: 'center',
        verticalPosition: 'top',
        duration: 10000
      }
    )
  }

  login(email: string, password: string): Observable<Message> {

    return this.httpClient.post<Message>(`${this.baseUrl}/login`, { email: email, password: password }).pipe(
      map(data => {

        localStorage.setItem('token', data.message);

        this.headers = new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization': `${data.message}`
        });

        data.message = 'Logged in successfully';

        return data;

      })
    )

  }

  getAccountData(): Observable<User> {
    return this.httpClient.get<any>(`${this.baseUrl}/account-data`, { headers: this.headers });
  }

  register(userDto: UserDto): Observable<User> {
    return this.httpClient.post<User>(`${this.baseUrl}/register`, userDto)
  }

  updateAccount(userDto: UserDto): Observable<User> {
    return this.httpClient.patch<User>(`${this.baseUrl}/account-update`, userDto, { headers: this.headers })
  }

  deleteAccount(): Observable<void> {
    return this.httpClient.delete<void>(`${this.baseUrl}/delete-account`, { headers: this.headers })
  }

  sendEmailCodeForForgottenPassword(email: string): Observable<Message> {
    return this.httpClient.post<Message>(`${this.baseUrl}/forgotten-password/send-email-code`, { email })
  }

  updateForgottenPassword(email: string, newPassword: string, code: number): Observable<Message> {
    return this.httpClient.put<Message>(`${this.baseUrl}/forgotten-password/change-password`, { email, newPassword, code })
  }

  /* ======== Admin functions =========== */

  getAllUsers(page = 1, size=20): Observable<Page<User[]>> {
    return this.httpClient.get<any>(`${this.baseUrl}/list-all-users?page=${page}&size=${size}`, { headers: this.headers });
  }

  findUserByEmail(userEmail: string, page = 1, size=20): Observable<Page<User[]>> {
    return this.httpClient.get<any>(`${this.baseUrl}/find-user-by-email/${userEmail}?page=${page}&size=${size}`, { headers: this.headers });
  }

  updateAUser(userEmail: string, userDto: UserDto): Observable<User>{
    return this.httpClient.patch<User>(`${this.baseUrl}/update-a-user/${userEmail}`, userDto, { headers: this.headers });
  }

  deleteAUser(userEmail: string): Observable<void> {
    return this.httpClient.delete<void>(`${this.baseUrl}/delete-a-user/${userEmail}`, { headers: this.headers });
  }

}
