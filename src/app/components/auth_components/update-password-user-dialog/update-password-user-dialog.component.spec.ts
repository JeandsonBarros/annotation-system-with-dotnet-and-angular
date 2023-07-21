import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdatePasswordUserDialogComponent } from './update-password-user-dialog.component';

describe('UpdatePasswordUserDialogComponent', () => {
  let component: UpdatePasswordUserDialogComponent;
  let fixture: ComponentFixture<UpdatePasswordUserDialogComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [UpdatePasswordUserDialogComponent]
    });
    fixture = TestBed.createComponent(UpdatePasswordUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
