import { Component, OnInit, WritableSignal, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { AnnotationDialogComponent } from 'src/app/components/annotation_components/annotation-dialog/annotation-dialog.component';
import { AnnotationDto } from 'src/app/shared/DTOs/AnnotationDto';
import { Annotation } from 'src/app/shared/Models/Annotation';
import { AnnotationUpdateParameters } from 'src/app/shared/Models/AnnotationUpdateParameters';
import { AnnotationService } from 'src/app/shared/services/annotation/annotation.service';

@Component({
  selector: 'app-search-annotation',
  templateUrl: './search-annotation.component.html',
  styleUrls: ['./search-annotation.component.scss']
})
export class SearchAnnotationComponent implements OnInit {

  isLoading = true;
  descriptionSearch = '';

  annotations!: Annotation[]
  paginationAnnotations = { pageNumber: 1, totalPages: 0 };

  constructor(
    private annotationService: AnnotationService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {

    this.activatedRoute.params.subscribe(params => {
      if (params['description'] != '') {
        this.descriptionSearch = params['description'];
        this.getAnnotations();
      } else {
        this.router.navigate(['']);
      }
    })

  }

  getAnnotations(pageNumber = 1): void {

    this.annotationService.findAnnotationByDescription(this.descriptionSearch, pageNumber).subscribe({
      next: (pageAnnotation) => {

        if (pageNumber === 1) {
          this.annotations = pageAnnotation.data
        } else {
          this.annotations = this.annotations.concat(pageAnnotation.data);
        }

        this.paginationAnnotations = { pageNumber: pageNumber, totalPages: pageAnnotation.totalPages }

        this.isLoading = false

      },
      error: (error) => {
        console.log(error);
        this.isLoading = false
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

    this.annotationService.postAnnotation(annotationDto).subscribe({
      next: (annotationResp) => {
        if(annotationResp.description.includes(this.descriptionSearch))
          this.annotations.push(annotationResp)
      },
      error: (error) => {
        console.log(error);
        this.annotationService.showSnackBar(error?.error?.message || "Error saving annotation");
      }
    })

  }

  updateAnnotation(annotationUpdateParameters: AnnotationUpdateParameters): void {

    const { annotationId, annotationDto } = annotationUpdateParameters

    this.annotationService.putAnnotation(annotationId, annotationDto).subscribe({
      next: (annotationResp) => {
        const index = this.annotations.findIndex(annotation => annotation.id == annotationId)
        this.annotations[index] = annotationResp
      },
      error: (error) => {
        this.annotationService.showSnackBar(error?.error?.message || "Error updating annotation");
      }
    })

  }

}
