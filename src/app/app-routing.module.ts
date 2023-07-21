import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RouterGuardService } from './shared/services/router-guard/router-guard.service';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/annotations',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./pages/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'annotations',
    loadChildren: () => import('./pages/annotation/annotation.module').then(m => m.AnnotationModule),
    canActivate: [RouterGuardService]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
