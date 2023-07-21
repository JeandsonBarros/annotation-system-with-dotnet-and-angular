import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListAnnotationsComponent } from './list-annotations.component';

describe('ListAnnotationsComponent', () => {
  let component: ListAnnotationsComponent;
  let fixture: ComponentFixture<ListAnnotationsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ListAnnotationsComponent]
    });
    fixture = TestBed.createComponent(ListAnnotationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
