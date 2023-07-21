import { Component, EventEmitter, Input, Output } from '@angular/core';

import { Annotation } from 'src/app/shared/Models/Annotation';
import { AnnotationUpdateParameters } from 'src/app/shared/Models/AnnotationUpdateParameters';
import { AnnotationService } from 'src/app/shared/services/annotation/annotation.service';

@Component({
  selector: 'app-list-annotations',
  templateUrl: './list-annotations.component.html',
  styleUrls: ['./list-annotations.component.scss']
})
export class ListAnnotationsComponent {

  @Input() titleList!: string;
  @Input({ required: true }) annotations!: Annotation[];
  @Output() updateAnnotation = new EventEmitter<AnnotationUpdateParameters>();

  constructor(private annotationService: AnnotationService) { }

  update(annotationUpdateParameters: AnnotationUpdateParameters): void {
    this.updateAnnotation.emit(annotationUpdateParameters);
  }

  deleteAnnotation(annotationId: number): void {

    this.annotationService.deleteAnnotation(annotationId).subscribe({
      next: () => {
        const index = this.annotations.findIndex(annotation => annotation.id == annotationId);
        this.annotations.splice(index, 1);
      },
      error: (error) => {
        console.log(error);
        this.annotationService.showSnackBar(error?.error?.message || "Error deleting annotation");
      }
    })

  }

}

