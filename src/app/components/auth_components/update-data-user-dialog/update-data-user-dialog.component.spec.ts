import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateDataUserDialogComponent } from './update-data-user-dialog.component';

describe('UpdateDataUserDialogComponent', () => {
  let component: UpdateDataUserDialogComponent;
  let fixture: ComponentFixture<UpdateDataUserDialogComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [UpdateDataUserDialogComponent]
    });
    fixture = TestBed.createComponent(UpdateDataUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
