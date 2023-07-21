import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AnnotationDialogComponent } from 'src/app/components/annotation_components/annotation-dialog/annotation-dialog.component';
import { Annotation } from 'src/app/shared/Models/Annotation';

import { AnnotationService } from './../../../shared/services/annotation/annotation.service';
import { AnnotationDto } from 'src/app/shared/DTOs/AnnotationDto';
import { AnnotationUpdateParameters } from 'src/app/shared/Models/AnnotationUpdateParameters';



@Component({
  selector: 'app-annotations',
  templateUrl: './annotations.component.html',
  styleUrls: ['./annotations.component.scss']
})
export class AnnotationsComponent implements OnInit {

  isLoading = true

  importantAnnotations!: Annotation[]
  paginationImportantAnnotations = { pageNumber: 1, totalPages: 0 }

  notImportantAnnotations!: Annotation[]
  paginationNotImportantAnnotations = { pageNumber: 1, totalPages: 0 }

  constructor(private annotationService: AnnotationService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.getImportantAnnotations();
    this.getNotImportantAnnotations();
  }

  getImportantAnnotations(pageNumber = 1): void {

    this.annotationService.getImportantAnnotations(pageNumber).subscribe({

      next: (pageAnnotation) => {

        if (pageNumber === 1) {
          this.importantAnnotations = pageAnnotation.data
        } else {
          this.importantAnnotations = this.importantAnnotations.concat(pageAnnotation.data);
        }

        this.paginationImportantAnnotations = { pageNumber: pageNumber, totalPages: pageAnnotation.totalPages }
        this.isLoading = false

      },

      error: (error) => {
        console.log(error);
        this.isLoading = false
        this.annotationService.showSnackBar(error?.error?.message || "Error getting important notes");
      }

    });

  }

  getNotImportantAnnotations(pageNumber = 1): void {

    this.annotationService.getNotImportantAnnotations(pageNumber).subscribe({

      next: (pageAnnotation) => {

        if (pageNumber === 1) {
          this.notImportantAnnotations = pageAnnotation.data
        } else {
          this.notImportantAnnotations = this.notImportantAnnotations.concat(pageAnnotation.data);
        }

        this.paginationNotImportantAnnotations = { pageNumber: pageNumber, totalPages: pageAnnotation.totalPages }
        this.isLoading = false

      },

      error: (error) => {
        console.log(error);
        this.isLoading = false
        this.annotationService.showSnackBar(error?.error?.message || "Error getting unimportant notes");
      }

    });

  }

  openDialogAddAnnotation(): void {

    this.dialog.open(AnnotationDialogComponent,
      {
        data: { actionAnnotation: (annotationDto: AnnotationDto) => this.addAnnotation(annotationDto) }
      });

  }

  addAnnotation(annotationDto: AnnotationDto): void {

    this.isLoading = true

    this.annotationService.postAnnotation(annotationDto).subscribe({
      next: (annotationResp) => {
        annotationResp.isImportant
          ? this.importantAnnotations.push(annotationResp)
          : this.notImportantAnnotations.push(annotationResp)
          this.isLoading = false
      },
      error: (error) => {
        console.log(error);
        this.annotationService.showSnackBar(error?.error?.message || "Error saving annotation");
        this.isLoading = false
      }
    })

  }

  updateAnnotation(annotationUpdateParameters: AnnotationUpdateParameters): void {

    const { annotationId, annotationDto } = annotationUpdateParameters

    this.isLoading = true

    this.annotationService.putAnnotation(annotationId, annotationDto).subscribe({
      next: (annotationResp) => {

        if (annotationResp.isImportant) {

          const indexInNotImportant = this.notImportantAnnotations.findIndex(annotation => annotation.id === annotationId)
          if (indexInNotImportant > -1) {
            this.notImportantAnnotations.splice(indexInNotImportant, 1)
            this.importantAnnotations.push(annotationResp)
          } else {
            const index = this.importantAnnotations.findIndex(annotation => annotation.id == annotationId)
            this.importantAnnotations[index] = annotationResp
          }

        } else {

          const indexInImportant = this.importantAnnotations.findIndex(annotation => annotation.id === annotationId)
          if (indexInImportant > -1) {
            this.importantAnnotations.splice(indexInImportant, 1)
            this.notImportantAnnotations.push(annotationResp)
          } else {
            const index = this.notImportantAnnotations.findIndex(annotation => annotation.id == annotationId)
            this.notImportantAnnotations[index] = annotationResp
          }

        }

        this.isLoading = false

      },
      error: (error) => {
        this.annotationService.showSnackBar(error?.error?.message || "Error updating annotation");
        this.isLoading = false
      }
    })

  }

}
