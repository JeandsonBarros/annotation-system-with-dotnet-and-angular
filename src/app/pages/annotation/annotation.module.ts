import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AnnotationRoutingModule } from './annotation-routing.module';
import { AnnotationsComponent } from './annotations/annotations.component';
import { AnnotationCardComponent } from 'src/app/components/annotation_components/annotation-card/annotation-card.component';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AnnotationDialogComponent } from '../../components/annotation_components/annotation-dialog/annotation-dialog.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ListAnnotationsComponent } from 'src/app/components/annotation_components/list-annotations/list-annotations.component';
import { SearchAnnotationComponent } from './search-annotation/search-annotation.component';


@NgModule({
  declarations: [
    AnnotationsComponent,
    AnnotationCardComponent,
    AnnotationDialogComponent,
    ListAnnotationsComponent,
    SearchAnnotationComponent,

  ],
  imports: [
    CommonModule,
    AnnotationRoutingModule,
    MatProgressBarModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
   
  ]
})
export class AnnotationModule { }
