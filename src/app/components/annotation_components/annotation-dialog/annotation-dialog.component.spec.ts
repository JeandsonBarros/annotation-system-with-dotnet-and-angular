import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnnotationDialogComponent } from './annotation-dialog.component';

describe('AnnotationDialogComponent', () => {
  let component: AnnotationDialogComponent;
  let fixture: ComponentFixture<AnnotationDialogComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AnnotationDialogComponent]
    });
    fixture = TestBed.createComponent(AnnotationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
