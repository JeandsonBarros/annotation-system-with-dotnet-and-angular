import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchAnnotationComponent } from './search-annotation.component';

describe('SearchAnnotationComponent', () => {
  let component: SearchAnnotationComponent;
  let fixture: ComponentFixture<SearchAnnotationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SearchAnnotationComponent]
    });
    fixture = TestBed.createComponent(SearchAnnotationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
