import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { AnnotationDto } from 'src/app/shared/DTOs/AnnotationDto';
import { Annotation } from 'src/app/shared/Models/Annotation';
import { AnnotationService } from 'src/app/shared/services/annotation/annotation.service';

interface AnnotationDialogData {
  annotation?: Annotation;
  actionAnnotation: (annotationDto: AnnotationDto) => void;
}

@Component({
  selector: 'app-annotation-dialog',
  templateUrl: './annotation-dialog.component.html',
  styleUrls: ['./annotation-dialog.component.scss']
})
export class AnnotationDialogComponent implements OnInit {

  annotation: Annotation = { id: NaN, title: '', description: '', isImportant: false }

  constructor(
    public dialogRef: MatDialogRef<AnnotationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public dialogData: AnnotationDialogData,
    private annotationService: AnnotationService
  ) { }

  ngOnInit(): void {
    if (this.dialogData?.annotation) this.annotation = { ...this.dialogData.annotation }
  }

  saveAnnotation(): void {

    if (!this.annotation.description) {
      return this.annotationService.showSnackBar("Description is required")
    }

    const annotationDto = {
      title: this.annotation.title,
      description: this.annotation.description,
      isImportant: this.annotation.isImportant
    }

    this.dialogData.actionAnnotation(annotationDto)
    this.dialogRef.close(annotationDto)

  }

  onNoClick(): void {
    this.dialogRef.close();
  }

}
