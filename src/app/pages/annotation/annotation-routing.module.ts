import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AnnotationsComponent } from './annotations/annotations.component';
import { SearchAnnotationComponent } from './search-annotation/search-annotation.component';

const routes: Routes = [
  {
    path: '',
    component: AnnotationsComponent

  },
  {
    path: 'search/:description',
    component: SearchAnnotationComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AnnotationRoutingModule { }
