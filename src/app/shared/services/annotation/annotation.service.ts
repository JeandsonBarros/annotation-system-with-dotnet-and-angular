import { Annotation } from './../../Models/Annotation';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Page } from '../../Models/Page';
import { AnnotationDto } from '../../DTOs/AnnotationDto';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class AnnotationService {

  private baseUrl = 'http://localhost:8080/api/annotation'
  headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `${localStorage.getItem('token')}`
  })

  constructor(private httpClient: HttpClient, private _snackBar: MatSnackBar) { }

  showSnackBar(message: string): void {
    this._snackBar.open(message, 'X',
      { horizontalPosition: 'center', verticalPosition: 'top' }
    )
  }

  getAnnotations(page = 1, size = 10): Observable<Page<Annotation[]>> {
    return this.httpClient.get<Page<Annotation[]>>(`${this.baseUrl}?page=${page}&size=${size}`, { headers: this.headers });
  }

  getImportantAnnotations(page = 1, size = 10): Observable<Page<Annotation[]>> {
    return this.httpClient.get<Page<Annotation[]>>(`${this.baseUrl}/important-annotations?page=${page}&size=${size}`, { headers: this.headers });
  }

  getNotImportantAnnotations(page = 1, size = 10): Observable<Page<Annotation[]>> {
    return this.httpClient.get<Page<Annotation[]>>(`${this.baseUrl}/not-important-annotations?page=${page}&size=${size}`, { headers: this.headers });
  }

  findAnnotationByDescription(description: string, page = 1, size = 10): Observable<Page<Annotation[]>> {
    return this.httpClient.get<Page<Annotation[]>>(`${this.baseUrl}/find-by-description/${description}?page=${page}&size=${size}`, { headers: this.headers });
  }

  postAnnotation(annotationDto: AnnotationDto): Observable<Annotation> {
    return this.httpClient.post<Annotation>(`${this.baseUrl}`, annotationDto, { headers: this.headers })
  }

  putAnnotation(annotationId: number, annotationDto: AnnotationDto): Observable<Annotation> {
    return this.httpClient.put<Annotation>(`${this.baseUrl}/${annotationId}`, annotationDto, { headers: this.headers })
  }

  deleteAnnotation(annotationId: number): Observable<void> {
    return this.httpClient.delete<void>(`${this.baseUrl}/${annotationId}`, { headers: this.headers })
  }

}
