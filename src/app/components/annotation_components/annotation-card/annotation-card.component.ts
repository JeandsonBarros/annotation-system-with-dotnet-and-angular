import { Annotation } from './../../../shared/Models/Annotation';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AnnotationDialogComponent } from '../annotation-dialog/annotation-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { AnnotationService } from 'src/app/shared/services/annotation/annotation.service';
import { AnnotationDto } from 'src/app/shared/DTOs/AnnotationDto';
import { AnnotationUpdateParameters } from 'src/app/shared/Models/AnnotationUpdateParameters';

@Component({
  selector: 'app-annotation-card',
  templateUrl: './annotation-card.component.html',
  styleUrls: ['./annotation-card.component.scss']
})
export class AnnotationCardComponent {

  @Input({ required: true }) annotation!: Annotation;
  @Output() updateAnnotation = new EventEmitter<AnnotationUpdateParameters>();
  @Output() deleteAnnotation = new EventEmitter<number>();

  constructor(public dialog: MatDialog) { }

  openDialog(): void {

    this.dialog.open(AnnotationDialogComponent, {
      data: {
        annotation: this.annotation,
        actionAnnotation: (annotationDto: AnnotationDto) => this.updateAnnotation.emit({annotationId: this.annotation.id, annotationDto: annotationDto})
      },
    });

  }

  deleteAnnotatonClick(): void{
    this.deleteAnnotation.emit(this.annotation.id)
  }

}
